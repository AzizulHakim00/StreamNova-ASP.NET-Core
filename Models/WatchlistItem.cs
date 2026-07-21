namespace StreamNova.Models;

public sealed class WatchlistItem
{
    public Guid UserId { get; set; }
    public int MovieId { get; set; }
    public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
}
