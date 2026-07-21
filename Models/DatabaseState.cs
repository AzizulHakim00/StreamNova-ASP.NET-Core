namespace StreamNova.Models;

public sealed class DatabaseState
{
    public List<AppUser> Users { get; set; } = [];
    public List<Movie> Movies { get; set; } = [];
    public List<WatchlistItem> Watchlist { get; set; } = [];
    public List<ViewingProgress> Progress { get; set; } = [];
    public List<MovieReview> Reviews { get; set; } = [];
}
