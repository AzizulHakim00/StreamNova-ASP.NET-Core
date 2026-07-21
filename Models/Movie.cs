using System.ComponentModel.DataAnnotations;

namespace StreamNova.Models;

public sealed class Movie
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string Synopsis { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(1, 500)]
    public int DurationMinutes { get; set; }

    [Required, StringLength(20)]
    public string MaturityRating { get; set; } = "PG-13";

    [Required, StringLength(300)]
    public string PosterPath { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string BackdropPath { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? VideoUrl { get; set; }

    [Range(0, 10)]
    public double Score { get; set; }

    public bool Featured { get; set; }
    public bool Trending { get; set; }
    public bool NewRelease { get; set; }
}
