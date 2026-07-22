namespace StreamNova.ViewModels;

public sealed class DiscoveryHomeViewModel
{
    public bool IsConfigured { get; set; }
    public string Region { get; set; } = "BD";
    public string? ErrorMessage { get; set; }
    public List<DiscoveryTitleViewModel> Trending { get; set; } = [];
    public List<DiscoveryTitleViewModel> Movies { get; set; } = [];
    public List<DiscoveryTitleViewModel> TvShows { get; set; } = [];
    public List<DiscoveryTitleViewModel> Anime { get; set; } = [];
    public List<DiscoveryTitleViewModel> NowPlaying { get; set; } = [];
    public List<DiscoveryTitleViewModel> TopRated { get; set; } = [];
    public List<DiscoveryTitleViewModel> Upcoming { get; set; } = [];
    public List<DiscoveryTitleViewModel> AiringToday { get; set; } = [];
    public List<DiscoveryTitleViewModel> KoreanDrama { get; set; } = [];
    public List<DiscoveryTitleViewModel> Documentaries { get; set; } = [];
}

public sealed class DiscoverySearchViewModel
{
    public bool IsConfigured { get; set; }
    public string Query { get; set; } = string.Empty;
    public string Type { get; set; } = "all";
    public string Region { get; set; } = "BD";
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string? ErrorMessage { get; set; }
    public List<DiscoveryTitleViewModel> Results { get; set; } = [];
}

public sealed class DiscoveryTitleViewModel
{
    public int Id { get; set; }
    public string MediaType { get; set; } = "movie";
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = "/images/catalog-placeholder.svg";
    public string BackdropUrl { get; set; } = "/images/catalog-placeholder.svg";
    public string ReleaseYear { get; set; } = "—";
    public double Rating { get; set; }
    public bool IsAnime { get; set; }
}

public sealed class DiscoveryDetailsViewModel
{
    public bool IsConfigured { get; set; }
    public string Region { get; set; } = "BD";
    public string? ErrorMessage { get; set; }
    public int Id { get; set; }
    public string MediaType { get; set; } = "movie";
    public string Title { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = "/images/catalog-placeholder.svg";
    public string BackdropUrl { get; set; } = "/images/catalog-placeholder.svg";
    public string ReleaseYear { get; set; } = "—";
    public string Status { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public double Rating { get; set; }
    public int VoteCount { get; set; }
    public bool IsAnime { get; set; }
    public string TmdbUrl { get; set; } = string.Empty;
    public string? HomepageUrl { get; set; }
    public string? TrailerName { get; set; }
    public string? TrailerEmbedUrl { get; set; }
    public string? ProviderLink { get; set; }
    public List<string> Genres { get; set; } = [];
    public List<DiscoveryProviderViewModel> StreamProviders { get; set; } = [];
    public List<DiscoveryProviderViewModel> RentProviders { get; set; } = [];
    public List<DiscoveryProviderViewModel> BuyProviders { get; set; } = [];
    public List<DiscoverySeasonViewModel> Seasons { get; set; } = [];
    public List<DiscoveryPersonViewModel> Cast { get; set; } = [];
}

public sealed class DiscoveryProviderViewModel
{
    public int ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}

public sealed class DiscoverySeasonViewModel
{
    public int SeasonNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
    public string AirYear { get; set; } = "—";
    public string PosterUrl { get; set; } = "/images/catalog-placeholder.svg";
    public string Overview { get; set; } = string.Empty;
}

public sealed class DiscoveryPersonViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = "/images/catalog-placeholder.svg";
}
