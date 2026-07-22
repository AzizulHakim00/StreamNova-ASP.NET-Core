using StreamNova.Models;

namespace StreamNova.Services;

public static class PlayableCatalogMigration
{
    public const int SchemaVersion = 4;

    public static void Apply(DatabaseState state)
    {
        if (state.SchemaVersion < 2)
        {
            EnsureOpenMovies(state, BuildOriginalOpenMovies());
        }

        if (state.SchemaVersion < 3)
        {
            EnsureOfficialPreviews(state);
        }

        if (state.SchemaVersion < 4)
        {
            EnsureOpenMovies(state, BuildExpandedOpenMovies());
        }

        state.SchemaVersion = SchemaVersion;
    }

    private static void EnsureOpenMovies(DatabaseState state, IReadOnlyList<Movie> additions)
    {
        var nextId = state.Movies.Count == 0 ? 1 : state.Movies.Max(movie => movie.Id) + 1;

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
            if (movie is not null)
            {
                ApplyPlaybackMetadata(movie, preview);
            }
        }
    }

    private static Movie? FindMovie(DatabaseState state, string title) =>
        state.Movies.FirstOrDefault(movie =>
            movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    private static void ApplyPlaybackMetadata(Movie target, Movie source)
    {
        target.VideoUrl ??= source.VideoUrl;
        target.PlaybackLabel ??= source.PlaybackLabel;
        target.SourceCredit ??= source.SourceCredit;
        target.SourcePageUrl ??= source.SourcePageUrl;
        target.LicenseLabel ??= source.LicenseLabel;
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

    public static List<Movie> BuildOpenMovies()
    {
        var movies = BuildOriginalOpenMovies();
        movies.AddRange(BuildExpandedOpenMovies());
        return movies;
    }

    private static List<Movie> BuildOriginalOpenMovies() =>
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

    private static List<Movie> BuildExpandedOpenMovies() =>
    [
        new Movie
        {
            Title = "Spring",
            Synopsis = "A shepherd and her dog confront ancient spirits to restore the cycle of the seasons.",
            Genre = "Fantasy",
            Year = 2019,
            DurationMinutes = 8,
            MaturityRating = "G",
            PosterPath = "/images/spring-open-movie.svg",
            BackdropPath = "/images/spring-open-movie.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Spring_-_Blender_Open_Movie.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/films/spring/",
            LicenseLabel = "Creative Commons Attribution 4.0",
            Score = 8.5,
            Trending = true,
            NewRelease = true
        },
        new Movie
        {
            Title = "Sprite Fright",
            Synopsis = "Teenagers discover that a peaceful woodland community is far more dangerous than it appears.",
            Genre = "Animation",
            Year = 2021,
            DurationMinutes = 11,
            MaturityRating = "PG",
            PosterPath = "/images/sprite-fright.svg",
            BackdropPath = "/images/sprite-fright.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Sprite_Fright_-_Open_Movie_by_Blender_Studio.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Studio / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/films/sprite-fright/",
            LicenseLabel = "Creative Commons Attribution 4.0",
            Score = 8.2,
            Trending = true
        },
        new Movie
        {
            Title = "Cosmos Laundromat",
            Synopsis = "A suicidal sheep meets a mysterious salesman who offers him a life-changing journey.",
            Genre = "Fantasy",
            Year = 2015,
            DurationMinutes = 12,
            MaturityRating = "PG",
            PosterPath = "/images/cosmos-laundromat.svg",
            BackdropPath = "/images/cosmos-laundromat.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Cosmos_Laundromat_-_First_Cycle_-_Official_Blender_Foundation_release.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Foundation / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/projects/cosmos-laundromat/",
            LicenseLabel = "Creative Commons Attribution 3.0",
            Score = 8.0,
            Trending = true
        },
        new Movie
        {
            Title = "Coffee Run",
            Synopsis = "A woman races through memories of a relationship while trying to reach her morning coffee.",
            Genre = "Comedy",
            Year = 2020,
            DurationMinutes = 3,
            MaturityRating = "G",
            PosterPath = "/images/coffee-run.svg",
            BackdropPath = "/images/coffee-run.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Coffee_Run_-_Blender_Open_Movie-full_movie.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Studio / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/films/coffee-run/",
            LicenseLabel = "Creative Commons Attribution 4.0",
            Score = 7.9,
            NewRelease = true
        },
        new Movie
        {
            Title = "Charge",
            Synopsis = "In a post-apocalyptic world, an old robot faces one final confrontation over a precious source of energy.",
            Genre = "Sci-Fi",
            Year = 2022,
            DurationMinutes = 5,
            MaturityRating = "PG",
            PosterPath = "/images/charge-open-movie.svg",
            BackdropPath = "/images/charge-open-movie.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Charge_-_Blender_Open_Movie-full_movie.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Studio / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/films/charge/",
            LicenseLabel = "Creative Commons Attribution 4.0",
            Score = 8.1,
            Trending = true,
            NewRelease = true
        },
        new Movie
        {
            Title = "WING IT!",
            Synopsis = "An inexperienced engineer and an ambitious pilot improvise their way through a chaotic flight test.",
            Genre = "Comedy",
            Year = 2023,
            DurationMinutes = 4,
            MaturityRating = "G",
            PosterPath = "/images/wing-it.svg",
            BackdropPath = "/images/wing-it.svg",
            VideoUrl = "https://commons.wikimedia.org/wiki/Special:Redirect/file/WING_IT%21_-_Blender_Open_Movie-full_movie.webm",
            PlaybackLabel = "Full open movie",
            SourceCredit = "Blender Studio / Wikimedia Commons",
            SourcePageUrl = "https://studio.blender.org/films/wing-it/",
            LicenseLabel = "Creative Commons Attribution 4.0",
            Score = 8.0,
            NewRelease = true
        }
    ];
}
