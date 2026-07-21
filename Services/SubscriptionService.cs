using StreamNova.Models;

namespace StreamNova.Services;

public sealed class SubscriptionService
{
    private readonly JsonDatabase _database;

    public SubscriptionService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyList<SubscriptionPlan>> GetPlansAsync() =>
        _database.UpdateAsync(state =>
        {
            EnsurePlans(state);
            return (IReadOnlyList<SubscriptionPlan>)state.SubscriptionPlans
                .OrderBy(plan => plan.MonthlyPrice)
                .ToList();
        });

    public Task<UserSubscription?> GetForUserAsync(Guid userId) =>
        _database.ReadAsync(state => state.Subscriptions
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.StartedAtUtc)
            .FirstOrDefault());

    public Task<bool> ChoosePlanAsync(Guid userId, string planId) =>
        _database.UpdateAsync(state =>
        {
            EnsurePlans(state);
            var plan = state.SubscriptionPlans.FirstOrDefault(item =>
                item.Id.Equals(planId, StringComparison.OrdinalIgnoreCase));
            if (plan is null)
            {
                return false;
            }

            var subscription = state.Subscriptions
                .Where(item => item.UserId == userId)
                .OrderByDescending(item => item.StartedAtUtc)
                .FirstOrDefault();

            if (subscription is null)
            {
                subscription = new UserSubscription { UserId = userId };
                state.Subscriptions.Add(subscription);
            }

            subscription.PlanId = plan.Id;
            subscription.Status = SubscriptionStatuses.Active;
            subscription.StartedAtUtc = DateTime.UtcNow;
            subscription.RenewsAtUtc = DateTime.UtcNow.AddMonths(1);
            subscription.CancelledAtUtc = null;

            state.Notifications.Add(new UserNotification
            {
                UserId = userId,
                Title = $"{plan.Name} plan activated",
                Message = $"Your StreamNova subscription is active and renews on {subscription.RenewsAtUtc:dd MMM yyyy}.",
                Type = "Subscription",
                ActionUrl = "/Subscription"
            });
            return true;
        });

    public Task<bool> CancelAsync(Guid userId) =>
        _database.UpdateAsync(state =>
        {
            var subscription = state.Subscriptions
                .Where(item => item.UserId == userId && item.Status == SubscriptionStatuses.Active)
                .OrderByDescending(item => item.StartedAtUtc)
                .FirstOrDefault();
            if (subscription is null)
            {
                return false;
            }

            subscription.Status = SubscriptionStatuses.Cancelled;
            subscription.CancelledAtUtc = DateTime.UtcNow;
            state.Notifications.Add(new UserNotification
            {
                UserId = userId,
                Title = "Subscription cancelled",
                Message = $"Access remains available until {subscription.RenewsAtUtc:dd MMM yyyy}.",
                Type = "Subscription",
                ActionUrl = "/Subscription"
            });
            return true;
        });

    public Task<int> ActiveCountAsync() =>
        _database.ReadAsync(state => state.Subscriptions.Count(item => item.Status == SubscriptionStatuses.Active));

    public Task<decimal> MonthlyRecurringRevenueAsync() =>
        _database.ReadAsync(state =>
        {
            EnsurePlans(state);
            return state.Subscriptions
                .Where(item => item.Status == SubscriptionStatuses.Active)
                .Join(state.SubscriptionPlans,
                    subscription => subscription.PlanId,
                    plan => plan.Id,
                    (_, plan) => plan.MonthlyPrice)
                .Sum();
        });

    private static void EnsurePlans(DatabaseState state)
    {
        if (state.SubscriptionPlans.Count > 0)
        {
            return;
        }

        state.SubscriptionPlans.AddRange(
        [
            new SubscriptionPlan
            {
                Id = "mobile",
                Name = "Mobile",
                Description = "Affordable streaming for one phone or tablet.",
                MonthlyPrice = 4.99m,
                MaxScreens = 1,
                VideoQuality = "HD",
                Features = ["One screen", "Mobile and tablet", "Ad-free streaming"]
            },
            new SubscriptionPlan
            {
                Id = "standard",
                Name = "Standard",
                Description = "The best balance for individuals and couples.",
                MonthlyPrice = 8.99m,
                MaxScreens = 2,
                VideoQuality = "Full HD",
                DownloadsEnabled = true,
                IsPopular = true,
                Features = ["Two screens", "Full HD", "Offline downloads", "Ad-free streaming"]
            },
            new SubscriptionPlan
            {
                Id = "premium",
                Name = "Premium",
                Description = "Maximum quality and more screens for families.",
                MonthlyPrice = 13.99m,
                MaxScreens = 4,
                VideoQuality = "4K + HDR",
                DownloadsEnabled = true,
                Features = ["Four screens", "4K and HDR", "Offline downloads", "Spatial audio"]
            }
        ]);
    }
}
