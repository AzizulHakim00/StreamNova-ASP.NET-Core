using System.ComponentModel.DataAnnotations;
using StreamNova.Models;

namespace StreamNova.ViewModels;

public sealed class SubscriptionPageViewModel
{
    public IReadOnlyList<SubscriptionPlan> Plans { get; set; } = [];
    public UserSubscription? CurrentSubscription { get; set; }
    public SubscriptionPlan? CurrentPlan { get; set; }
}

public sealed class SupportTicketInputViewModel
{
    [Required, StringLength(120, MinimumLength = 4)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = "General";

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = TicketPriorities.Normal;
}

public sealed class SupportCenterViewModel
{
    public SupportTicketInputViewModel Input { get; set; } = new();
    public IReadOnlyList<SupportTicket> Tickets { get; set; } = [];
}

public sealed class AdminSupportTicketViewModel
{
    public SupportTicket Ticket { get; set; } = new();
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}

public sealed class NotificationCenterViewModel
{
    public IReadOnlyList<UserNotification> Notifications { get; set; } = [];
    public int UnreadCount { get; set; }
}
