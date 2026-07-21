using System.Security.Claims;
using StreamNova.Services;
using StreamNova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

public sealed class BrowseController : Controller
{
    private readonly MovieService _movieService;
    private readonly UserMovieService _userMovieService;
    private readonly ReviewService _reviewService;

    public BrowseController(
        MovieService movieService,
        UserMovieService userMovieService,
        ReviewService reviewService)
    {
        _movieService = movieService;
        _userMovieService = userMovieService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index(string? q, string? genre)
    {
        var model = new BrowseViewModel
        {
            Movies = await _movieService.SearchAsync(q, genre),
            Genres = await _movieService.GetGenresAsync(),
            Query = q,
            Genre = genre
        };
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        Guid? currentUserId = TryGetUserId(out var parsedUserId) ? parsedUserId : null;
        var all = await _movieService.GetAllAsync();
        var summary = await _reviewService.GetSummaryAsync(id);
        var model = new MovieDetailsViewModel
        {
            Movie = movie,
            Similar = all.Where(item => item.Id != id && item.Genre == movie.Genre).Take(6).ToList(),
            AverageReviewRating = summary.Average,
            ReviewCount = summary.Count,
            Reviews = await _reviewService.GetForMovieAsync(id, currentUserId)
        };

        if (currentUserId.HasValue)
        {
            model.InWatchlist = await _userMovieService.IsInWatchlistAsync(currentUserId.Value, id);
            var progress = await _userMovieService.GetProgressAsync(currentUserId.Value, id);
            model.ResumeSeconds = progress?.ProgressSeconds ?? 0;
            var userReview = await _reviewService.GetUserReviewAsync(currentUserId.Value, id);
            if (userReview is not null)
            {
                model.CurrentUserReviewId = userReview.Id;
                model.ReviewInput = new ReviewInputViewModel
                {
                    Rating = userReview.Rating,
                    Comment = userReview.Comment
                };
            }
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Watch(int id)
    {
        var movie = await _movieService.GetAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        var resumeSeconds = 0;
        if (TryGetUserId(out var userId))
        {
            var progress = await _userMovieService.GetProgressAsync(userId, id);
            resumeSeconds = progress?.ProgressSeconds ?? 0;
        }

        return View(new WatchViewModel { Movie = movie, ResumeSeconds = resumeSeconds });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProgress(int id, int progressSeconds, int durationSeconds)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _userMovieService.SaveProgressAsync(userId, id, progressSeconds, durationSeconds);
        return Json(new { saved = true });
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
