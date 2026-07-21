namespace StreamNova.Models;

public sealed class ViewingProgress
{
    public Guid UserId { get; set; }
    public int MovieId { get; set; }
    public int ProgressSeconds { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
