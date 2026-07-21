using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNova.Services;
using StreamNova.ViewModels;

namespace StreamNova.Controllers;

[Authorize]
public sealed class SubscriptionController : Controller
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var plans = await _subscriptionService.GetPlansAsync();
        var subscription = await _subscriptionService.GetForUserAsync(userId);
        return View(new SubscriptionPageViewModel
        {
            Plans = plans,
            CurrentSubscription = subscription,
            CurrentPlan = subscription is null
                ? null
                : plans.FirstOrDefault(plan => plan.Id == subscription.PlanId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Choose(string planId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        TempData[await _subscriptionService.ChoosePlanAsync(userId, planId) ? "Success" : "Error"] =
            "Subscription updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        TempData[await _subscriptionService.CancelAsync(userId) ? "Success" : "Error"] =
            "Subscription cancellation processed.";
        return RedirectToAction(nameof(Index));
    }

    private bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
