using StreamNova.Models;

namespace StreamNova.Services;

public sealed class NotificationService
{
    private readonly JsonDatabase _database;

    public NotificationService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyList<UserNotification>> GetForUserAsync(Guid userId) =>
        _database.ReadAsync(state => (IReadOnlyList<UserNotification>)state.Notifications
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.CreatedAtUtc)
            .ToList());

    public Task<int> UnreadCountAsync(Guid userId) =>
        _database.ReadAsync(state => state.Notifications.Count(item => item.UserId == userId && !item.IsRead));

    public Task<bool> MarkReadAsync(Guid userId, Guid notificationId) =>
        _database.UpdateAsync(state =>
        {
            var notification = state.Notifications.FirstOrDefault(item =>
                item.Id == notificationId && item.UserId == userId);
            if (notification is null)
            {
                return false;
            }

            notification.IsRead = true;
            return true;
        });

    public Task<int> MarkAllReadAsync(Guid userId) =>
        _database.UpdateAsync(state =>
        {
            var notifications = state.Notifications
                .Where(item => item.UserId == userId && !item.IsRead)
                .ToList();
            notifications.ForEach(item => item.IsRead = true);
            return notifications.Count;
        });

    public Task CreateAsync(Guid userId, string title, string message, string type = "Info", string? actionUrl = null) =>
        _database.UpdateAsync(state =>
        {
            state.Notifications.Add(new UserNotification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl
            });
            return true;
        });
}
