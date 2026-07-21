using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class BrowseViewModel
{
    public IReadOnlyList<Movie> Movies { get; set; } = [];
    public IReadOnlyList<string> Genres { get; set; } = [];
    public string? Query { get; set; }
    public string? Genre { get; set; }
}
