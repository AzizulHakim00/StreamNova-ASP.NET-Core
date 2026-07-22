using StreamNova.Models;

namespace StreamNova.Services;

public static class PlayableCatalogMigration
{
    public const int SchemaVersion = 5;

    public static void Apply(DatabaseState state)
    {
        if (state.SchemaVersion < 2)
        {
            EnsureCatalogItems(state, BuildOriginalOpenMovies());
        }

        if (state.SchemaVersion < 3)
        {
            EnsureOfficialPreviews(state);
        }

        if (state.SchemaVersion < 4)
        {
            EnsureCatalogItems(state, BuildExpandedOpenMovies());
        }

        if (state.SchemaVersion < 5)
        {
            EnsureCatalogItems(state, BuildCuratedTrailers());
        }

        state.SchemaVersion = SchemaVersion;
    }

    private static void EnsureCatalogItems(DatabaseState state, IReadOnlyList<Movie> additions)
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
            SourcePageUrl = "https://www.warnerbros.com/movies/blade-runner-2049",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Shutter Island",
            VideoUrl = "https://www.youtube.com/watch?v=v8yrZSkKxTA",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Paramount Pictures",
            SourcePageUrl = "https://www.paramountpictures.com/movies/shutter-island",
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova"
        },
        new Movie
        {
            Title = "Fast X",
            VideoUrl = "https://www.youtube.com/watch?v=SAhlmquynBY",
            PlaybackLabel = "Official trailer",
            SourceCredit = "Universal Pictures",
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

    private static List<Movie> BuildCuratedTrailers() =>
    [
        CreateTrailer(
            "Oppenheimer",
            "The story of the physicist whose work on the Manhattan Project changed history and the modern world.",
            "Drama", 2023, 180, "R", "/images/trailer-drama.svg",
            "https://www.youtube.com/watch?v=uYPbbksJxIg",
            "Universal Pictures", "https://www.universalpictures.com/movies/oppenheimer", 8.6, trending: true, newRelease: true),
        CreateTrailer(
            "The Batman",
            "A young Batman follows a trail of corruption while a serial killer targets Gotham's elite.",
            "Thriller", 2022, 176, "PG-13", "/images/trailer-action.svg",
            "https://www.youtube.com/watch?v=mqqft2x_Aa4",
            "Warner Bros. Pictures", "https://www.warnerbros.com/movies/batman", 7.8, trending: true),
        CreateTrailer(
            "Avatar: The Way of Water",
            "The Sully family fights to protect one another and the ocean clans of Pandora.",
            "Sci-Fi", 2022, 192, "PG-13", "/images/trailer-space.svg",
            "https://www.youtube.com/watch?v=d9MyW72ELq0",
            "20th Century Studios", "https://www.avatar.com/movies/avatar-the-way-of-water", 7.6, newRelease: true),
        CreateTrailer(
            "Top Gun: Maverick",
            "An elite naval aviator returns to train a new generation for a dangerous mission.",
            "Action", 2022, 131, "PG-13", "/images/trailer-action.svg",
            "https://www.youtube.com/watch?v=giXco2jaZ_4",
            "Paramount Pictures", "https://www.paramountpictures.com/movies/top-gun-maverick", 8.3, trending: true),
        CreateTrailer(
            "Joker",
            "A struggling performer descends into isolation and becomes an infamous figure in Gotham City.",
            "Drama", 2019, 122, "R", "/images/trailer-drama.svg",
            "https://www.youtube.com/watch?v=zAGVQLHvwOY",
            "Warner Bros. Pictures", "https://www.warnerbros.com/movies/joker", 8.4),
        CreateTrailer(
            "Dune",
            "A gifted young heir travels to the most dangerous planet in the universe to protect his people.",
            "Sci-Fi", 2021, 155, "PG-13", "/images/trailer-space.svg",
            "https://www.youtube.com/watch?v=n9xhJrPXop4",
            "Warner Bros. Pictures", "https://www.warnerbros.com/movies/dune", 8.0, trending: true),
        CreateTrailer(
            "Everything Everywhere All at Once",
            "An exhausted laundromat owner is swept into a multiverse-spanning adventure.",
            "Sci-Fi", 2022, 140, "R", "/images/trailer-comedy.svg",
            "https://www.youtube.com/watch?v=wxN1T1uxQ2g",
            "A24", "https://a24films.com/films/everything-everywhere-all-at-once", 7.8, newRelease: true),
        CreateTrailer(
            "Spider-Man: Across the Spider-Verse",
            "Miles Morales journeys across the multiverse and clashes with a team of Spider-People.",
            "Animation", 2023, 140, "PG", "/images/trailer-animation.svg",
            "https://www.youtube.com/watch?v=cqGjhVJWtEg",
            "Sony Pictures Entertainment", "https://www.sonypictures.com/movies/spidermanacrossthespiderverse", 8.6, trending: true, newRelease: true)
    ];

    private static Movie CreateTrailer(
        string title,
        string synopsis,
        string genre,
        int year,
        int durationMinutes,
        string maturityRating,
        string artwork,
        string videoUrl,
        string sourceCredit,
        string sourcePageUrl,
        double score,
        bool trending = false,
        bool newRelease = false) =>
        new()
        {
            Title = title,
            Synopsis = synopsis,
            Genre = genre,
            Year = year,
            DurationMinutes = durationMinutes,
            MaturityRating = maturityRating,
            PosterPath = artwork,
            BackdropPath = artwork,
            VideoUrl = videoUrl,
            PlaybackLabel = "Official trailer",
            SourceCredit = sourceCredit,
            SourcePageUrl = sourcePageUrl,
            LicenseLabel = "Official promotional preview; full feature not hosted by StreamNova",
            Score = score,
            Trending = trending,
            NewRelease = newRelease
        };

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
