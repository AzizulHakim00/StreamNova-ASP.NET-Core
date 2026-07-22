using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class WatchViewModel
{
    public Movie Movie { get; set; } = new();
    public Movie? PreviousMovie { get; set; }
    public Movie? NextMovie { get; set; }
    public int ResumeSeconds { get; set; }
    public string PlaybackKind { get; set; } = "none";
    public string? PlaybackUrl { get; set; }
    public string? PlaybackMimeType { get; set; }
}
