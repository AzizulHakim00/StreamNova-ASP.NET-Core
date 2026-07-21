using StreamNova.Models;

namespace StreamNova.Services;

public static class PlayableCatalogMigration
{
    public const int SchemaVersion = 2;

    public static void Apply(DatabaseState state)
    {
        if (state.SchemaVersion >= SchemaVersion)
        {
            return;
        }

        var nextId = state.Movies.Count == 0 ? 1 : state.Movies.Max(movie => movie.Id) + 1;
        var additions = BuildOpenMovies();

        for (var index = additions.Count - 1; index >= 0; index--)
        {
            var template = additions[index];
            var existing = state.Movies.FirstOrDefault(movie =>
                movie.Title.Equals(template.Title, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                template.Id = nextId++;
                state.Movies.Insert(0, template);
                continue;
            }

            existing.VideoUrl ??= template.VideoUrl;
            existing.PlaybackLabel ??= template.PlaybackLabel;
            existing.SourceCredit ??= template.SourceCredit;
            existing.SourcePageUrl ??= template.SourcePageUrl;
            existing.LicenseLabel ??= template.LicenseLabel;
        }

        state.SchemaVersion = SchemaVersion;
    }

    public static List<Movie> BuildOpenMovies() =>
    [
        new Movie
        {
            Title = "Big Buck Bunny",
            Synopsis = "A gentle giant of a rabbit turns the tables on three mischievous forest bullies in this Blender open movie.",
            Genre = "Animation",
            Year = 2008,
            DurationMinutes = 10,
            MaturityRating = "G",
            PosterPath = "/images/big-buck-bunny.svg",
            BackdropPath = "/images/big-buck-bunny.svg",
            VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation",
            SourcePageUrl = "https://studio.blender.org/projects/big-buck-bunny/gallery/",
            LicenseLabel = "Creative Commons / Blender Open Movie",
            Score = 8.3,
            Featured = true,
            Trending = true,
            NewRelease = true
        },
        new Movie
        {
            Title = "Elephants Dream",
            Synopsis = "Two characters explore a strange machine-filled world in the first Blender open movie.",
            Genre = "Animation",
            Year = 2006,
            DurationMinutes = 11,
            MaturityRating = "PG",
            PosterPath = "/images/elephants-dream.svg",
            BackdropPath = "/images/elephants-dream.svg",
            VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation",
            SourcePageUrl = "https://studio.blender.org/projects/elephants-dream/gallery/",
            LicenseLabel = "Creative Commons / Blender Open Movie",
            Score = 7.6,
            Trending = true
        },
        new Movie
        {
            Title = "Sintel",
            Synopsis = "A determined young woman crosses a dangerous fantasy world while searching for a dragon she once rescued.",
            Genre = "Fantasy",
            Year = 2010,
            DurationMinutes = 15,
            MaturityRating = "PG",
            PosterPath = "/images/sintel.svg",
            BackdropPath = "/images/sintel.svg",
            VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/Sintel.mp4",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation",
            SourcePageUrl = "https://studio.blender.org/projects/sintel/all-artwork/",
            LicenseLabel = "Creative Commons / Blender Open Movie",
            Score = 8.1,
            Trending = true
        },
        new Movie
        {
            Title = "Tears of Steel",
            Synopsis = "Scientists and warriors reunite in Amsterdam to prevent a destructive robot conflict in this live-action open movie.",
            Genre = "Sci-Fi",
            Year = 2012,
            DurationMinutes = 12,
            MaturityRating = "PG-13",
            PosterPath = "/images/tears-of-steel.svg",
            BackdropPath = "/images/tears-of-steel.svg",
            VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/TearsOfSteel.mp4",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation",
            SourcePageUrl = "https://mango.blender.org/about/",
            LicenseLabel = "Creative Commons Attribution 3.0",
            Score = 7.8,
            NewRelease = true
        }
    ];
}