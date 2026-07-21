using StreamNova.Models;
using StreamNova.Services;
using StreamNova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public sealed class AdminController : Controller
{
    private readonly MovieService _movieService;
    private readonly UserService _userService;
    private readonly UserMovieService _userMovieService;
    private readonly ReviewService _reviewService;
    private readonly SubscriptionService _subscriptionService;
    private readonly SupportService _supportService;

    public AdminController(
        MovieService movieService,
        UserService userService,
        UserMovieService userMovieService,
        ReviewService reviewService,
        SubscriptionService subscriptionService,
        SupportService supportService)
    {
        _movieService = movieService;
        _userService = userService;
        _userMovieService = userMovieService;
        _reviewService = reviewService;
        _subscriptionService = subscriptionService;
        _supportService = supportService;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _movieService.GetAllAsync();
        var model = new AdminDashboardViewModel
        {
            TotalMovies = movies.Count,
            TotalUsers = await _userService.CountAsync(),
            WatchlistAdds = await _userMovieService.WatchlistCountAsync(),
            ActiveProgressItems = await _userMovieService.ProgressCountAsync(),
            TotalReviews = await _reviewService.CountAsync(),
            ActiveSubscriptions = await _subscriptionService.ActiveCountAsync(),
            MonthlyRecurringRevenue = await _subscriptionService.MonthlyRecurringRevenueAsync(),
            OpenSupportTickets = await _supportService.OpenCountAsync(),
            LatestMovies = movies.OrderByDescending(movie => movie.Id).Take(5).ToList()
        };
        return View(model);
    }

    public async Task<IActionResult> Movies() => View(await _movieService.GetAllAsync());

    public async Task<IActionResult> Users() => View(await _userService.GetAdminUsersAsync());

    [HttpGet]
    public IActionResult Create() => View("MovieForm", new Movie
    {
        Year = DateTime.UtcNow.Year,
        DurationMinutes = 120,
        MaturityRating = "PG-13",
        Score = 7,
        PosterPath = "/images/hero.svg",
        BackdropPath = "/images/hero.svg"
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return View("MovieForm", movie);
        }

        await _movieService.SaveAsync(movie);
        TempData["Success"] = "Movie added successfully.";
        return RedirectToAction(nameof(Movies));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _movieService.GetAsync(id);
        return movie is null ? NotFound() : View("MovieForm", movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return View("MovieForm", movie);
        }

        await _movieService.SaveAsync(movie);
        TempData["Success"] = "Movie updated successfully.";
        return RedirectToAction(nameof(Movies));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _movieService.DeleteAsync(id);
        TempData["Success"] = "Movie deleted.";
        return RedirectToAction(nameof(Movies));
    }
}
