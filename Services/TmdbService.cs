using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class TmdbService
{
    private const string ImageBase = "https://image.tmdb.org/t/p";
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TmdbService> _logger;
    private readonly string? _accessToken;

    public TmdbService(
        HttpClient httpClient,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<TmdbService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _accessToken = configuration["Tmdb:ReadAccessToken"]
            ?? Environment.GetEnvironmentVariable("TMDB_READ_ACCESS_TOKEN")
            ?? Environment.GetEnvironmentVariable("TMDB_API_TOKEN");
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_accessToken);

    public async Task<DiscoveryHomeViewModel> GetHomeAsync(string? region, CancellationToken cancellationToken = default)
    {
        var model = new DiscoveryHomeViewModel
        {
            IsConfigured = IsConfigured,
            Region = NormalizeRegion(region)
        };

        if (!IsConfigured)
        {
            model.ErrorMessage = "TMDB discovery is ready but needs a TMDB Read Access Token.";
            return model;
        }

        try
        {
            var trendingTask = GetJsonAsync("trending/all/week?language=en-US", cancellationToken);
            var moviesTask = GetJsonAsync("movie/popular?language=en-US&page=1", cancellationToken);
            var tvTask = GetJsonAsync("tv/popular?language=en-US&page=1", cancellationToken);
            var animeTask = GetJsonAsync("discover/tv?language=en-US&page=1&sort_by=popularity.desc&include_adult=false&with_genres=16&with_origin_country=JP", cancellationToken);

            await Task.WhenAll(trendingTask, moviesTask, tvTask, animeTask);

            model.Trending = ParseTitleList(await trendingTask, null, false);
            model.Movies = ParseTitleList(await moviesTask, "movie", false);
            model.TvShows = ParseTitleList(await tvTask, "tv", false);
            model.Anime = ParseTitleList(await animeTask, "tv", true);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "TMDB landing catalog could not be loaded.");
            model.ErrorMessage = "The live catalog could not be loaded right now. Please try again shortly.";
        }

        return model;
    }

    public async Task<DiscoverySearchViewModel> SearchAsync(
        string? query,
        string? type,
        string? region,
        int page,
        CancellationToken cancellationToken = default)
    {
        var normalizedType = NormalizeType(type, allowAll: true);
        var model = new DiscoverySearchViewModel
        {
            IsConfigured = IsConfigured,
            Query = query?.Trim() ?? string.Empty,
            Type = normalizedType,
            Region = NormalizeRegion(region),
            Page = Math.Max(1, page)
        };

        if (!IsConfigured)
        {
            model.ErrorMessage = "Add a TMDB Read Access Token to activate search.";
            return model;
        }

        if (string.IsNullOrWhiteSpace(model.Query))
        {
            return model;
        }

        try
        {
            var encoded = Uri.EscapeDataString(model.Query);
            var endpoint = normalizedType switch
            {
                "movie" => $"search/movie?query={encoded}&include_adult=false&language=en-US&page={model.Page}",
                "tv" or "anime" => $"search/tv?query={encoded}&include_adult=false&language=en-US&page={model.Page}",
                _ => $"search/multi?query={encoded}&include_adult=false&language=en-US&page={model.Page}"
            };

            var root = await GetJsonAsync(endpoint, cancellationToken);
            model.Results = ParseTitleList(
                root,
                normalizedType is "movie" or "tv" or "anime" ? (normalizedType == "anime" ? "tv" : normalizedType) : null,
                normalizedType == "anime");
            model.TotalPages = Math.Clamp(GetInt(root, "total_pages"), 1, 500);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "TMDB search failed for {Query}.", model.Query);
            model.ErrorMessage = "Search is temporarily unavailable.";
        }

        return model;
    }

    public async Task<DiscoveryDetailsViewModel?> GetDetailsAsync(
        string? type,
        int id,
        string? region,
        CancellationToken cancellationToken = default)
    {
        var mediaType = NormalizeType(type, allowAll: false);
        if (id <= 0 || mediaType is not ("movie" or "tv"))
        {
            return null;
        }

        var model = new DiscoveryDetailsViewModel
        {
            IsConfigured = IsConfigured,
            Id = id,
            MediaType = mediaType,
            Region = NormalizeRegion(region),
            TmdbUrl = $"https://www.themoviedb.org/{mediaType}/{id}"
        };

        if (!IsConfigured)
        {
            model.ErrorMessage = "Add a TMDB Read Access Token to load this title.";
            return model;
        }

        try
        {
            var detailsTask = GetJsonAsync($"{mediaType}/{id}?append_to_response=videos,credits&language=en-US", cancellationToken);
            var providersTask = GetJsonAsync($"{mediaType}/{id}/watch/providers", cancellationToken);
            await Task.WhenAll(detailsTask, providersTask);

            var root = await detailsTask;
            var providersRoot = await providersTask;

            model.Title = FirstNonEmpty(GetString(root, "title"), GetString(root, "name"), "Untitled");
            model.OriginalTitle = FirstNonEmpty(GetString(root, "original_title"), GetString(root, "original_name"), model.Title);
            model.Overview = GetString(root, "overview") ?? string.Empty;
            model.PosterUrl = BuildImageUrl(GetString(root, "poster_path"), "w500");
            model.BackdropUrl = BuildImageUrl(GetString(root, "backdrop_path"), "original");
            model.ReleaseYear = ReadYear(FirstNonEmpty(GetString(root, "release_date"), GetString(root, "first_air_date")));
            model.Status = GetString(root, "status") ?? string.Empty;
            model.Language = GetString(root, "original_language")?.ToUpperInvariant() ?? string.Empty;
            model.Rating = Math.Round(GetDouble(root, "vote_average"), 1);
            model.VoteCount = GetInt(root, "vote_count");
            model.HomepageUrl = GetString(root, "homepage");
            model.Genres = ReadStringArray(root, "genres", "name");
            model.IsAnime = mediaType == "tv"
                && string.Equals(GetString(root, "original_language"), "ja", StringComparison.OrdinalIgnoreCase)
                && model.Genres.Any(genre => genre.Equals("Animation", StringComparison.OrdinalIgnoreCase));

            if (mediaType == "movie")
            {
                model.RuntimeMinutes = GetInt(root, "runtime");
            }
            else if (root.TryGetProperty("episode_run_time", out var runtimeArray)
                && runtimeArray.ValueKind == JsonValueKind.Array
                && runtimeArray.GetArrayLength() > 0)
            {
                model.RuntimeMinutes = runtimeArray[0].GetInt32();
            }

            ReadTrailer(root, model);
            ReadProviders(providersRoot, model);
            ReadSeasons(root, model);
            ReadCast(root, model);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "TMDB details failed for {MediaType} {Id}.", mediaType, id);
            model.ErrorMessage = "This title could not be loaded right now.";
        }

        return model;
    }

    private async Task<JsonElement> GetJsonAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(relativeUrl, out JsonElement cached))
        {
            return cached;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"TMDB returned HTTP {(int)response.StatusCode}.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement.Clone();
        _cache.Set(relativeUrl, root, TimeSpan.FromMinutes(15));
        return root;
    }

    private static List<DiscoveryTitleViewModel> ParseTitleList(JsonElement root, string? forcedMediaType, bool animeOnly)
    {
        var items = new List<DiscoveryTitleViewModel>();
        if (!root.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
        {
            return items;
        }

        foreach (var item in results.EnumerateArray())
        {
            if (GetBoolean(item, "adult"))
            {
                continue;
            }

            var mediaType = forcedMediaType ?? GetString(item, "media_type") ?? (item.TryGetProperty("title", out _) ? "movie" : "tv");
            if (mediaType is not ("movie" or "tv"))
            {
                continue;
            }

            var isAnime = mediaType == "tv"
                && string.Equals(GetString(item, "original_language"), "ja", StringComparison.OrdinalIgnoreCase)
                && HasGenre(item, 16);
            if (animeOnly && !isAnime)
            {
                continue;
            }

            var title = FirstNonEmpty(GetString(item, "title"), GetString(item, "name"), "Untitled");
            items.Add(new DiscoveryTitleViewModel
            {
                Id = GetInt(item, "id"),
                MediaType = mediaType,
                Title = title,
                Overview = GetString(item, "overview") ?? string.Empty,
                PosterUrl = BuildImageUrl(GetString(item, "poster_path"), "w500"),
                BackdropUrl = BuildImageUrl(GetString(item, "backdrop_path"), "w1280"),
                ReleaseYear = ReadYear(FirstNonEmpty(GetString(item, "release_date"), GetString(item, "first_air_date"))),
                Rating = Math.Round(GetDouble(item, "vote_average"), 1),
                IsAnime = isAnime
            });
        }

        return items.Take(20).ToList();
    }

    private static void ReadTrailer(JsonElement root, DiscoveryDetailsViewModel model)
    {
        if (!root.TryGetProperty("videos", out var videos)
            || !videos.TryGetProperty("results", out var results)
            || results.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        JsonElement? selected = null;
        foreach (var video in results.EnumerateArray())
        {
            if (!string.Equals(GetString(video, "site"), "YouTube", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var type = GetString(video, "type");
            var official = GetBoolean(video, "official");
            if (string.Equals(type, "Trailer", StringComparison.OrdinalIgnoreCase) && official)
            {
                selected = video;
                break;
            }

            if (selected is null && string.Equals(type, "Trailer", StringComparison.OrdinalIgnoreCase))
            {
                selected = video;
            }
            else if (selected is null && string.Equals(type, "Teaser", StringComparison.OrdinalIgnoreCase))
            {
                selected = video;
            }
        }

        if (selected is not JsonElement trailer)
        {
            return;
        }

        var key = GetString(trailer, "key");
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        model.TrailerName = GetString(trailer, "name") ?? "Official trailer";
        model.TrailerEmbedUrl = $"https://www.youtube-nocookie.com/embed/{Uri.EscapeDataString(key)}?rel=0&modestbranding=1";
    }

    private static void ReadProviders(JsonElement root, DiscoveryDetailsViewModel model)
    {
        if (!root.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        if (!results.TryGetProperty(model.Region, out var regionNode))
        {
            return;
        }

        model.ProviderLink = GetString(regionNode, "link");
        model.StreamProviders = ParseProviders(regionNode, "flatrate");
        model.RentProviders = ParseProviders(regionNode, "rent");
        model.BuyProviders = ParseProviders(regionNode, "buy");
    }

    private static List<DiscoveryProviderViewModel> ParseProviders(JsonElement regionNode, string propertyName)
    {
        var providers = new List<DiscoveryProviderViewModel>();
        if (!regionNode.TryGetProperty(propertyName, out var items) || items.ValueKind != JsonValueKind.Array)
        {
            return providers;
        }

        foreach (var provider in items.EnumerateArray())
        {
            providers.Add(new DiscoveryProviderViewModel
            {
                ProviderId = GetInt(provider, "provider_id"),
                Name = GetString(provider, "provider_name") ?? "Provider",
                LogoUrl = BuildImageUrl(GetString(provider, "logo_path"), "w92")
            });
        }

        return providers
            .GroupBy(provider => provider.ProviderId)
            .Select(group => group.First())
            .Take(12)
            .ToList();
    }

    private static void ReadSeasons(JsonElement root, DiscoveryDetailsViewModel model)
    {
        if (model.MediaType != "tv"
            || !root.TryGetProperty("seasons", out var seasons)
            || seasons.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var season in seasons.EnumerateArray())
        {
            var seasonNumber = GetInt(season, "season_number");
            if (seasonNumber < 0)
            {
                continue;
            }

            model.Seasons.Add(new DiscoverySeasonViewModel
            {
                SeasonNumber = seasonNumber,
                Name = GetString(season, "name") ?? $"Season {seasonNumber}",
                EpisodeCount = GetInt(season, "episode_count"),
                AirYear = ReadYear(GetString(season, "air_date")),
                PosterUrl = BuildImageUrl(GetString(season, "poster_path"), "w300"),
                Overview = GetString(season, "overview") ?? string.Empty
            });
        }
    }

    private static void ReadCast(JsonElement root, DiscoveryDetailsViewModel model)
    {
        if (!root.TryGetProperty("credits", out var credits)
            || !credits.TryGetProperty("cast", out var cast)
            || cast.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var person in cast.EnumerateArray().Take(12))
        {
            model.Cast.Add(new DiscoveryPersonViewModel
            {
                Name = GetString(person, "name") ?? "Unknown",
                Character = GetString(person, "character") ?? string.Empty,
                ProfileUrl = BuildImageUrl(GetString(person, "profile_path"), "w185")
            });
        }
    }

    private static List<string> ReadStringArray(JsonElement root, string arrayProperty, string valueProperty)
    {
        var values = new List<string>();
        if (!root.TryGetProperty(arrayProperty, out var array) || array.ValueKind != JsonValueKind.Array)
        {
            return values;
        }

        foreach (var item in array.EnumerateArray())
        {
            var value = GetString(item, valueProperty);
            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(value);
            }
        }

        return values;
    }

    private static bool HasGenre(JsonElement item, int genreId)
    {
        if (!item.TryGetProperty("genre_ids", out var genres) || genres.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        return genres.EnumerateArray().Any(value => value.ValueKind == JsonValueKind.Number && value.GetInt32() == genreId);
    }

    private static string BuildImageUrl(string? path, string size) =>
        string.IsNullOrWhiteSpace(path)
            ? "/images/catalog-placeholder.svg"
            : $"{ImageBase}/{size}{path}";

    private static string ReadYear(string? date) =>
        !string.IsNullOrWhiteSpace(date) && date.Length >= 4 ? date[..4] : "—";

    private static string NormalizeRegion(string? region)
    {
        var value = region?.Trim().ToUpperInvariant();
        return value is { Length: 2 } && value.All(char.IsLetter) ? value : "BD";
    }

    private static string NormalizeType(string? type, bool allowAll)
    {
        var value = type?.Trim().ToLowerInvariant();
        if (value is "movie" or "tv" or "anime")
        {
            return value;
        }

        return allowAll ? "all" : "movie";
    }

    private static string FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static string? GetString(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object
        && element.TryGetProperty(propertyName, out var value)
        && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static int GetInt(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object
        && element.TryGetProperty(propertyName, out var value)
        && value.ValueKind == JsonValueKind.Number
        && value.TryGetInt32(out var number)
            ? number
            : 0;

    private static double GetDouble(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object
        && element.TryGetProperty(propertyName, out var value)
        && value.ValueKind == JsonValueKind.Number
        && value.TryGetDouble(out var number)
            ? number
            : 0;

    private static bool GetBoolean(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object
        && element.TryGetProperty(propertyName, out var value)
        && value.ValueKind is JsonValueKind.True or JsonValueKind.False
        && value.GetBoolean();
}
