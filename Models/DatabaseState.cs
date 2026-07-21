namespace StreamNova.Models;

public sealed class DatabaseState
{
    public int SchemaVersion { get; set; }
    public List<AppUser> Users { get; set; } = [];
    public List<Movie> Movies { get; set; } = [];
    public List<WatchlistItem> Watchlist { get; set; } = [];
    public List<ViewingProgress> Progress { get; set; } = [];
    public List<MovieReview> Reviews { get; set; } = [];
    public List<SubscriptionPlan> SubscriptionPlans { get; set; } = [];
    public List<UserSubscription> Subscriptions { get; set; } = [];
    public List<UserNotification> Notifications { get; set; } = [];
    public List<SupportTicket> SupportTickets { get; set; } = [];
}