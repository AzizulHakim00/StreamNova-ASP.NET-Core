using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class ContinueWatchingItemViewModel
{
    public Movie Movie { get; set; } = new();
    public int ProgressSeconds { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public int Percentage => DurationSeconds <= 0
        ? 0
        : Math.Clamp((int)Math.Round(ProgressSeconds * 100d / DurationSeconds), 0, 100);
}
