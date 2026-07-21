using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class AdminDashboardViewModel
{
    public int TotalMovies { get; set; }
    public int TotalUsers { get; set; }
    public int WatchlistAdds { get; set; }
    public int ActiveProgressItems { get; set; }
    public int TotalReviews { get; set; }
    public IReadOnlyList<Movie> LatestMovies { get; set; } = [];
}
