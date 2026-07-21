namespace StreamNova.ViewModels;

public sealed class MovieReviewViewModel
{
    public Guid Id { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsOwner { get; set; }
}
