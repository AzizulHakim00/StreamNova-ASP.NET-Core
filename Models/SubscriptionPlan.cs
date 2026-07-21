namespace StreamNova.Models;

public sealed class SubscriptionPlan
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public int MaxScreens { get; set; }
    public string VideoQuality { get; set; } = "Full HD";
    public bool DownloadsEnabled { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Features { get; set; } = [];
}

public sealed class UserSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string Status { get; set; } = SubscriptionStatuses.Active;
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime RenewsAtUtc { get; set; } = DateTime.UtcNow.AddMonths(1);
    public DateTime? CancelledAtUtc { get; set; }
}

public static class SubscriptionStatuses
{
    public const string Active = "Active";
    public const string Cancelled = "Cancelled";
}
