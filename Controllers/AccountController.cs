using System.Security.Claims;
using StreamNova.Models;
using StreamNova.Services;
using StreamNova.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StreamNova.Controllers;

public sealed class AccountController : Controller
{
    private readonly UserService _userService;
    private readonly UserMovieService _userMovieService;
    private readonly ReviewService _reviewService;

    public AccountController(
        UserService userService,
        UserMovieService userMovieService,
        ReviewService reviewService)
    {
        _userService = userService;
        _userMovieService = userMovieService;
        _reviewService = reviewService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var user = await _userService.ValidateAsync(model.Email, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return user.Role == UserRoles.Admin
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.RegisterAsync(model.Name, model.Email, model.Password);
        if (!result.Success || result.User is null)
        {
            ModelState.AddModelError(nameof(model.Email), result.Error);
            return View(model);
        }

        await SignInAsync(result.User, true);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var user = await _userService.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        return View(new ProfileViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            MemberSinceUtc = user.CreatedAtUtc,
            WatchlistCount = await _userMovieService.WatchlistCountForUserAsync(userId),
            ContinueWatchingCount = await _userMovieService.ContinueWatchingCountForUserAsync(userId),
            ReviewCount = await _reviewService.CountForUserAsync(userId)
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            TempData["ProfileError"] = "Enter a display name with at least two characters.";
            return RedirectToAction(nameof(Profile));
        }

        var user = await _userService.UpdateNameAsync(userId, model.Name);
        if (user is null)
        {
            return NotFound();
        }

        await SignInAsync(user, true);
        TempData["ProfileSuccess"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            TempData["PasswordError"] = "Use at least eight characters and make sure the confirmation matches.";
            return RedirectToAction(nameof(Profile));
        }

        var changed = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
        TempData[changed ? "ProfileSuccess" : "PasswordError"] = changed
            ? "Password changed successfully."
            : "The current password is incorrect.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(AppUser user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            properties);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
