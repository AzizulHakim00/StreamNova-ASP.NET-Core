using Microsoft.AspNetCore.Mvc;
using StreamNova.Services;

namespace StreamNova.Controllers;

[Route("discover")]
public sealed class DiscoverController : Controller
{
    private readonly TmdbService _tmdbService;
    private readonly TmdbShelfService _tmdbShelfService;

    public DiscoverController(TmdbService tmdbService, TmdbShelfService tmdbShelfService)
    {
        _tmdbService = tmdbService;
        _tmdbShelfService = tmdbShelfService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? region, CancellationToken cancellationToken)
    {
        var model = await _tmdbService.GetHomeAsync(region, cancellationToken);
        await _tmdbShelfService.PopulateAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        string? q,
        string? type,
        string? region,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var model = await _tmdbService.SearchAsync(q, type, region, page, cancellationToken);
        return View(model);
    }

    [HttpGet("{type}/{id:int}")]
    public async Task<IActionResult> Details(
        string type,
        int id,
        string? region,
        CancellationToken cancellationToken)
    {
        var model = await _tmdbService.GetDetailsAsync(type, id, region, cancellationToken);
        return model is null ? NotFound() : View(model);
    }
}
