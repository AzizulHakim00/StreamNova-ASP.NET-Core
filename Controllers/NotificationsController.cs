using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNova.Services;
using StreamNova.ViewModels;

namespace StreamNova.Controllers;

[Authorize]
public sealed class NotificationsController : Controller
{
    private readonly NotificationService _notificationService;

    public NotificationsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return View(new NotificationCenterViewModel
        {
            Notifications = await _notificationService.GetForUserAsync(userId),
            UnreadCount = await _notificationService.UnreadCountAsync(userId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Read(Guid id, string? returnUrl = null)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _notificationService.MarkReadAsync(userId, id);
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _notificationService.MarkAllReadAsync(userId);
        return RedirectToAction(nameof(Index));
    }

    private bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
