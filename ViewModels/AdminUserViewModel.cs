namespace StreamNova.ViewModels;

public sealed class AdminUserViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public int WatchlistCount { get; set; }
    public int ReviewCount { get; set; }
}
