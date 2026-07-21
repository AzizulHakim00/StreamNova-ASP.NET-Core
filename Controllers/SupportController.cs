using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNova.Models;
using StreamNova.Services;
using StreamNova.ViewModels;

namespace StreamNova.Controllers;

[Authorize]
public sealed class SupportController : Controller
{
    private readonly SupportService _supportService;

    public SupportController(SupportService supportService)
    {
        _supportService = supportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return View(new SupportCenterViewModel
        {
            Tickets = await _supportService.GetForUserAsync(userId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupportCenterViewModel model)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            model.Tickets = await _supportService.GetForUserAsync(userId);
            return View("Index", model);
        }

        await _supportService.CreateAsync(userId, model.Input);
        TempData["Success"] = "Your support ticket was created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Admin() => View(await _supportService.GetForAdminAsync());

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(Guid id, string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            TempData["Error"] = "Add a response before resolving the ticket.";
            return RedirectToAction(nameof(Admin));
        }

        TempData[await _supportService.ResolveAsync(id, response) ? "Success" : "Error"] =
            "Ticket status updated.";
        return RedirectToAction(nameof(Admin));
    }

    private bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
