using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class HomeViewModel
{
    public Movie Featured { get; set; } = new();
    public IReadOnlyList<Movie> Trending { get; set; } = [];
    public IReadOnlyList<Movie> NewReleases { get; set; } = [];
    public IReadOnlyList<Movie> Drama { get; set; } = [];
    public IReadOnlyList<ContinueWatchingItemViewModel> ContinueWatching { get; set; } = [];
}
