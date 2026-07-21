using StreamNova.Models;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class SupportService
{
    private readonly JsonDatabase _database;

    public SupportService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<SupportTicket> CreateAsync(Guid userId, SupportTicketInputViewModel input) =>
        _database.UpdateAsync(state =>
        {
            var ticket = new SupportTicket
            {
                UserId = userId,
                Subject = input.Subject.Trim(),
                Category = input.Category,
                Description = input.Description.Trim(),
                Priority = input.Priority
            };
            state.SupportTickets.Add(ticket);
            state.Notifications.Add(new UserNotification
            {
                UserId = userId,
                Title = "Support request received",
                Message = $"Ticket {ticket.Id.ToString()[..8].ToUpperInvariant()} has been opened.",
                Type = "Support",
                ActionUrl = "/Support"
            });
            return ticket;
        });

    public Task<IReadOnlyList<SupportTicket>> GetForUserAsync(Guid userId) =>
        _database.ReadAsync(state => (IReadOnlyList<SupportTicket>)state.SupportTickets
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.UpdatedAtUtc)
            .ToList());

    public Task<IReadOnlyList<AdminSupportTicketViewModel>> GetForAdminAsync() =>
        _database.ReadAsync(state => (IReadOnlyList<AdminSupportTicketViewModel>)state.SupportTickets
            .OrderBy(item => item.Status == TicketStatuses.Resolved)
            .ThenByDescending(item => item.Priority == TicketPriorities.High)
            .ThenByDescending(item => item.UpdatedAtUtc)
            .Select(ticket =>
            {
                var user = state.Users.FirstOrDefault(item => item.Id == ticket.UserId);
                return new AdminSupportTicketViewModel
                {
                    Ticket = ticket,
                    UserName = user?.Name ?? "Unknown user",
                    UserEmail = user?.Email ?? string.Empty
                };
            })
            .ToList());

    public Task<bool> ResolveAsync(Guid ticketId, string response) =>
        _database.UpdateAsync(state =>
        {
            var ticket = state.SupportTickets.FirstOrDefault(item => item.Id == ticketId);
            if (ticket is null)
            {
                return false;
            }

            ticket.Status = TicketStatuses.Resolved;
            ticket.AdminResponse = response.Trim();
            ticket.UpdatedAtUtc = DateTime.UtcNow;
            state.Notifications.Add(new UserNotification
            {
                UserId = ticket.UserId,
                Title = "Support ticket resolved",
                Message = $"Your request “{ticket.Subject}” has a new response.",
                Type = "Support",
                ActionUrl = "/Support"
            });
            return true;
        });

    public Task<int> OpenCountAsync() =>
        _database.ReadAsync(state => state.SupportTickets.Count(item => item.Status != TicketStatuses.Resolved));
}
