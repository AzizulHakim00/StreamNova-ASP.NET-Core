using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class WatchViewModel
{
    public Movie Movie { get; set; } = new();
    public int ResumeSeconds { get; set; }
}
