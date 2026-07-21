using System.ComponentModel.DataAnnotations;

namespace StreamNova.ViewModels;

public sealed class ReviewInputViewModel
{
    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required, StringLength(800, MinimumLength = 3)]
    public string Comment { get; set; } = string.Empty;
}
