using System.Security.Claims;
using StreamNova.Services;
using StreamNova.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

public sealed class HomeController : Controller
{
    private readonly MovieService _movieService;
    private readonly UserMovieService _userMovieService;
    private readonly RecommendationService _recommendationService;

    public HomeController(
        MovieService movieService,
        UserMovieService userMovieService,
        RecommendationService recommendationService)
    {
        _movieService = movieService;
        _userMovieService = userMovieService;
        _recommendationService = recommendationService;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _movieService.GetAllAsync();
        if (movies.Count == 0)
        {
            return View(new HomeViewModel());
        }

        var model = new HomeViewModel
        {
            Featured = movies.FirstOrDefault(movie => movie.Featured) ?? movies[0],
            Trending = movies.Where(movie => movie.Trending).Take(10).ToList(),
            NewReleases = movies.Where(movie => movie.NewRelease).Take(10).ToList(),
            Drama = movies.Where(movie => movie.Genre is "Thriller" or "Sci-Fi").Take(10).ToList()
        };

        if (TryGetUserId(out var userId))
        {
            model.ContinueWatching = await _userMovieService.GetContinueWatchingAsync(userId);
            model.Recommended = await _recommendationService.GetForUserAsync(userId);
        }
        else
        {
            model.Recommended = movies
                .OrderByDescending(movie => movie.Score)
                .Take(10)
                .ToList();
        }

        return View(model);
    }

    public IActionResult Privacy() => View();

    [Route("Home/Error")]
    public IActionResult Error() => View();

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
