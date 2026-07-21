namespace StreamNova.Models;

public sealed class SupportTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = TicketPriorities.Normal;
    public string Status { get; set; } = TicketStatuses.Open;
    public string? AdminResponse { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public static class TicketStatuses
{
    public const string Open = "Open";
    public const string InProgress = "In progress";
    public const string Resolved = "Resolved";
}

public static class TicketPriorities
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";
}
