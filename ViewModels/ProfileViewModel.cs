using System.ComponentModel.DataAnnotations;

namespace StreamNova.ViewModels;

public sealed class ProfileViewModel
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime MemberSinceUtc { get; set; }
    public int WatchlistCount { get; set; }
    public int ContinueWatchingCount { get; set; }
    public int ReviewCount { get; set; }
}
