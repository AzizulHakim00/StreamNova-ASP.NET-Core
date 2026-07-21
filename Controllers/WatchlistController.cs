using System.Security.Claims;
using StreamNova.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

[Authorize]
public sealed class WatchlistController : Controller
{
    private readonly UserMovieService _userMovieService;

    public WatchlistController(UserMovieService userMovieService)
    {
        _userMovieService = userMovieService;
    }

    public async Task<IActionResult> Index()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return View(await _userMovieService.GetWatchlistAsync(userId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int movieId, string? returnUrl = null)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _userMovieService.ToggleWatchlistAsync(userId, movieId);
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction("Details", "Browse", new { id = movieId });
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
