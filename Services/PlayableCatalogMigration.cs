using StreamNova.Models;

namespace StreamNova.Services;

public static class PlayableCatalogMigration
{
    public const int SchemaVersion = 3;

    public static void Apply(DatabaseState state)
    {
        if (state.SchemaVersion < 2)
        {
            EnsureOpenMovies(state);
        }

        if (state.SchemaVersion < 3)
        {
            EnsureOfficialPreviews(state);
        }

        state.SchemaVersion = SchemaVersion;
    }

    private static void EnsureOpenMovies(DatabaseState state)
    {
        var nextId = state.Movies.Count == 0 ? 1 : state.Movies.Max(movie => movie.Id) + 1;
        var additions = BuildOpenMovies();

        for (var index = additions.Count - 1; index >= 0; index--)
        {
            var template = additions[index];
            var existing = FindMovie(state, template.Title);

            if (existing is null)
            {
                template.Id = nextId++;
                state.Movies.Insert(0, template);
                continue;
            }

            ApplyPlaybackMetadata(existing, template);
        }
    }

    private static void EnsureOfficialPreviews(DatabaseState state)
    {
        foreach (var preview in BuildOfficialPreviews())
        {
            var movie = FindMovie(state, preview.Title);
            if (movie is null)
            {
                continue;
            }

            ApplyPlaybackMetadata(movie, preview);
        }
    }

    private static Movie? FindMovie(DatabaseState state, string title) =>
        state.Movies.FirstOrDefault(movie =>
            movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    private static void ApplyPlaybackMetadata(Movie target, Movie source)
    {
        if (string.IsNullOrWhiteSpace(target.VideoUrl))
        {
            target.VideoUrl = source.VideoUrl;
        }

        if (string.IsNullOrWhiteSpace(target.PlaybackLabel))
        {
            target.PlaybackLabel = source.PlaybackLabel;
        }

        if (string.IsNullOrWhiteSpace(target.SourceCredit))
        {
            target.SourceCredit = source.SourceCredit;
        }

        if (string.IsNullOrWhiteSpace(target.SourcePageUrl))
        {
            target.SourcePageUrl = source.SourcePageUrl;
        }

        if (string.IsNullOrWhiteSpace(target.LicenseLabel))
        {
            target.LicenseLabel = source.LicenseLabel;
        }
    }

    private static List<Movie> BuildOfficialPreviews() =>
    [
        new Movie
        {
            Title = "Interstellar",
            VideoUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Warner Bros. UK & Ireland / Paramount Pictures",
            SourcePageUrl = "https://www.paramountpictures.com/movies/interstellar",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Blade Runner 2049",
            VideoUrl = "https://www.youtube.com/watch?v=gCcx85zbxz4",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Warner Bros.",
            SourcePageUrl = "https://www.youtube.com/watch?v=gCcx85zbxz4",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Shutter Island",
            VideoUrl = "https://www.youtube.com/watch?v=v8yrZSkKxTA",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Rotten Tomatoes Classic Trailers / Paramount Pictures",
            SourcePageUrl = "https://www.paramountpictures.com/movies/shutter-island",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Fast X",
            VideoUrl = "https://www.youtube.com/watch?v=SAhlmquynBY",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Universal Pictures Canada",
            SourcePageUrl = "https://www.fastxmovie.com/",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Barbie",
            VideoUrl = "https://www.youtube.com/watch?v=pBk4NYhWNMM",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Warner Bros.",
            SourcePageUrl = "https://www.barbie-themovie.com/",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Spider-Man",
            VideoUrl = "https://www.youtube.com/watch?v=t06RUxPbp_c",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Sony Pictures Entertainment",
            SourcePageUrl = "https://www.sonypictures.com/movies/spiderman",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        }
    ];

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
