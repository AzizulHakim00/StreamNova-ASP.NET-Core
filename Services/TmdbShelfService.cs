using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class TmdbShelfService
{
    private const string ImageBase = "https://image.tmdb.org/t/p";
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TmdbShelfService> _logger;
    private readonly string? _accessToken;

    public TmdbShelfService(
        HttpClient httpClient,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<TmdbShelfService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _accessToken = configuration["Tmdb:ReadAccessToken"]
            ?? Environment.GetEnvironmentVariable("TMDB_READ_ACCESS_TOKEN")
            ?? Environment.GetEnvironmentVariable("TMDB_API_TOKEN");
    }

    public async Task PopulateAsync(DiscoveryHomeViewModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            return;
        }

        try
        {
            var region = Uri.EscapeDataString(model.Region);
            var nowPlayingTask = GetJsonAsync($"movie/now_playing?language=en-US&page=1&region={region}", cancellationToken);
            var topRatedTask = GetJsonAsync("movie/top_rated?language=en-US&page=1", cancellationToken);
            var upcomingTask = GetJsonAsync($"movie/upcoming?language=en-US&page=1&region={region}", cancellationToken);
            var airingTask = GetJsonAsync("tv/airing_today?language=en-US&page=1", cancellationToken);
            var koreanTask = GetJsonAsync("discover/tv?language=en-US&page=1&include_adult=false&sort_by=popularity.desc&with_origin_country=KR", cancellationToken);
            var documentaryTask = GetJsonAsync("discover/movie?language=en-US&page=1&include_adult=false&sort_by=popularity.desc&with_genres=99", cancellationToken);

            await Task.WhenAll(nowPlayingTask, topRatedTask, upcomingTask, airingTask, koreanTask, documentaryTask);

            model.NowPlaying = ParseTitles(await nowPlayingTask, "movie");
            model.TopRated = ParseTitles(await topRatedTask, "movie");
            model.Upcoming = ParseTitles(await upcomingTask, "movie");
            model.AiringToday = ParseTitles(await airingTask, "tv");
            model.KoreanDrama = ParseTitles(await koreanTask, "tv");
            model.Documentaries = ParseTitles(await documentaryTask, "movie");
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "The expanded TMDB shelves could not be loaded.");
        }
    }

    private async Task<JsonElement> GetJsonAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        var cacheKey = $"expanded:{relativeUrl}";
        if (_cache.TryGetValue(cacheKey, out JsonElement cached))
        {
            return cached;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement.Clone();
        _cache.Set(cacheKey, root, TimeSpan.FromMinutes(20));
        return root;
    }

    private static List<DiscoveryTitleViewModel> ParseTitles(JsonElement root, string mediaType)
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

            var title = FirstNonEmpty(GetString(item, "title"), GetString(item, "name"), "Untitled");
            var originalLanguage = GetString(item, "original_language") ?? string.Empty;
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
                IsAnime = mediaType == "tv" && originalLanguage.Equals("ja", StringComparison.OrdinalIgnoreCase) && HasGenre(item, 16)
            });
        }

        return items
            .Where(item => item.Id > 0)
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .Take(20)
            .ToList();
    }

    private static bool HasGenre(JsonElement item, int genreId) =>
        item.TryGetProperty("genre_ids", out var genres)
        && genres.ValueKind == JsonValueKind.Array
        && genres.EnumerateArray().Any(value => value.ValueKind == JsonValueKind.Number && value.GetInt32() == genreId);

    private static string BuildImageUrl(string? path, string size) =>
        string.IsNullOrWhiteSpace(path) ? "/images/catalog-placeholder.svg" : $"{ImageBase}/{size}{path}";

    private static string ReadYear(string? date) =>
        !string.IsNullOrWhiteSpace(date) && date.Length >= 4 ? date[..4] : "—";

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
