using System.Text.Json;
using StreamNova.Models;

namespace StreamNova.Services;

public sealed class JsonDatabase
{
    private readonly IWebHostEnvironment _environment;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private string FilePath
    {
        get
        {
            var configuredPath = Environment.GetEnvironmentVariable("STREAMNOVA_DATA_PATH");
            return string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(_environment.ContentRootPath, "App_Data", "streamnova.json")
                : Path.GetFullPath(configuredPath);
        }
    }

    public JsonDatabase(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task InitializeAsync(PasswordService passwordService)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        if (File.Exists(FilePath))
        {
            await UpdateAsync(state =>
            {
                PlayableCatalogMigration.Apply(state);
                return true;
            });
            return;
        }

        var adminPassword = passwordService.Hash("Admin@123");
        var userPassword = passwordService.Hash("User@123");

        var state = new DatabaseState
        {
            SchemaVersion = PlayableCatalogMigration.SchemaVersion,
            Users =
            [
                new AppUser
                {
                    Name = "StreamNova Admin",
                    Email = "admin@streamnova.local",
                    PasswordHash = adminPassword.Hash,
                    PasswordSalt = adminPassword.Salt,
                    Role = UserRoles.Admin
                },
                new AppUser
                {
                    Name = "Demo Viewer",
                    Email = "user@streamnova.local",
                    PasswordHash = userPassword.Hash,
                    PasswordSalt = userPassword.Salt,
                    Role = UserRoles.Customer
                }
            ],
            Movies = BuildInitialMovies()
        };

        await WriteInternalAsync(state);
    }

    public async Task<T> ReadAsync<T>(Func<DatabaseState, T> selector)
    {
        await _lock.WaitAsync();
        try
        {
            var state = await ReadInternalAsync();
            return selector(state);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<T> UpdateAsync<T>(Func<DatabaseState, T> update)
    {
        await _lock.WaitAsync();
        try
        {
            var state = await ReadInternalAsync();
            var result = update(state);
            await WriteInternalAsync(state);
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<DatabaseState> ReadInternalAsync()
    {
        if (!File.Exists(FilePath))
        {
            return new DatabaseState();
        }

        await using var stream = File.OpenRead(FilePath);
        return await JsonSerializer.DeserializeAsync<DatabaseState>(stream, _jsonOptions)
            ?? new DatabaseState();
    }

    private async Task WriteInternalAsync(DatabaseState state)
    {
        var temporary = FilePath + ".tmp";
        await using (var stream = File.Create(temporary))
        {
            await JsonSerializer.SerializeAsync(stream, state, _jsonOptions);
        }

        File.Move(temporary, FilePath, true);
    }

    private static List<Movie> BuildInitialMovies()
    {
        var movies = PlayableCatalogMigration.BuildOpenMovies();
        movies.AddRange(SeedMovies());
        for (var index = 0; index < movies.Count; index++)
        {
            movies[index].Id = index + 1;
        }

        return movies;
    }

    private static List<Movie> SeedMovies() =>
    [
        new Movie
        {
            Id = 1,
            Title = "Interstellar",
            Synopsis = "A team of explorers travels through a wormhole in space in an attempt to ensure humanity's survival.",
            Genre = "Sci-Fi",
            Year = 2014,
            DurationMinutes = 169,
            MaturityRating = "PG-13",
            PosterPath = "/images/interstellar.svg",
            BackdropPath = "/images/interstellar.svg",
            Score = 8.7,
            Featured = true,
            Trending = true
        },
        new Movie
        {
            Id = 2,
            Title = "Blade Runner 2049",
            Synopsis = "A young officer uncovers a long-buried secret that leads him to track down a former police officer.",
            Genre = "Sci-Fi",
            Year = 2017,
            DurationMinutes = 164,
            MaturityRating = "R",
            PosterPath = "/images/blade-runner.svg",
            BackdropPath = "/images/blade-runner.svg",
            Score = 8.0,
            Trending = true
        },
        new Movie
        {
            Id = 3,
            Title = "Shutter Island",
            Synopsis = "A marshal investigates the disappearance of a patient from an isolated hospital and questions everything around him.",
            Genre = "Thriller",
            Year = 2010,
            DurationMinutes = 138,
            MaturityRating = "R",
            PosterPath = "/images/shutter-island.svg",
            BackdropPath = "/images/shutter-island.svg",
            Score = 8.2,
            Trending = true
        },
        new Movie
        {
            Id = 4,
            Title = "Fast X",
            Synopsis = "A street-racing family faces a powerful enemy connected to its past and fights to protect everyone it loves.",
            Genre = "Action",
            Year = 2023,
            DurationMinutes = 141,
            MaturityRating = "PG-13",
            PosterPath = "/images/fast-x.svg",
            BackdropPath = "/images/fast-x.svg",
            Score = 6.3,
            NewRelease = true
        },
        new Movie
        {
            Id = 5,
            Title = "Barbie",
            Synopsis = "A journey into the real world challenges a perfect life and reveals what it means to choose your own story.",
            Genre = "Comedy",
            Year = 2023,
            DurationMinutes = 114,
            MaturityRating = "PG-13",
            PosterPath = "/images/barbie.svg",
            BackdropPath = "/images/barbie.svg",
            Score = 7.0,
            NewRelease = true
        },
        new Movie
        {
            Id = 6,
            Title = "Spider-Man",
            Synopsis = "A young hero learns that great ability comes with responsibility while protecting his city from danger.",
            Genre = "Action",
            Year = 2002,
            DurationMinutes = 121,
            MaturityRating = "PG-13",
            PosterPath = "/images/spiderman.svg",
            BackdropPath = "/images/spiderman.svg",
            Score = 7.4,
            NewRelease = true
        }
    ];
}