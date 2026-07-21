using System.Security.Claims;
using StreamNova.Models;
using StreamNova.Services;
using StreamNova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

[Authorize]
public sealed class ReviewsController : Controller
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int movieId, ReviewInputViewModel model)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            TempData["ReviewError"] = "Choose a rating and write at least three characters.";
            return RedirectToAction("Details", "Browse", new { id = movieId, review = "invalid" });
        }

        await _reviewService.SaveAsync(userId, movieId, model.Rating, model.Comment);
        TempData["ReviewSuccess"] = "Your review was saved.";
        return RedirectToAction("Details", "Browse", new { id = movieId, review = "saved" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, int movieId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _reviewService.DeleteAsync(userId, id, User.IsInRole(UserRoles.Admin));
        TempData["ReviewSuccess"] = "Review removed.";
        return RedirectToAction("Details", "Browse", new { id = movieId });
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
