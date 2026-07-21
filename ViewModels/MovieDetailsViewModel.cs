using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class MovieDetailsViewModel
{
    public Movie Movie { get; set; } = new();
    public IReadOnlyList<Movie> Similar { get; set; } = [];
    public bool InWatchlist { get; set; }
    public int ResumeSeconds { get; set; }
    public double AverageReviewRating { get; set; }
    public int ReviewCount { get; set; }
    public IReadOnlyList<MovieReviewViewModel> Reviews { get; set; } = [];
    public ReviewInputViewModel ReviewInput { get; set; } = new();
    public Guid? CurrentUserReviewId { get; set; }
}
