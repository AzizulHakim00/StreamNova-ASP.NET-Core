using StreamNova.Models;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class UserMovieService
{
    private readonly JsonDatabase _database;

    public UserMovieService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<bool> IsInWatchlistAsync(Guid userId, int movieId) =>
        _database.ReadAsync(state => state.Watchlist.Any(item => item.UserId == userId && item.MovieId == movieId));

    public Task<bool> ToggleWatchlistAsync(Guid userId, int movieId) =>
        _database.UpdateAsync(state =>
        {
            if (!state.Movies.Any(movie => movie.Id == movieId))
            {
                return false;
            }

            var existing = state.Watchlist.FirstOrDefault(item => item.UserId == userId && item.MovieId == movieId);
            if (existing is not null)
            {
                state.Watchlist.Remove(existing);
                return false;
            }

            state.Watchlist.Add(new WatchlistItem { UserId = userId, MovieId = movieId });
            return true;
        });

    public Task<IReadOnlyList<Movie>> GetWatchlistAsync(Guid userId) =>
        _database.ReadAsync(state =>
        {
            var orderedIds = state.Watchlist
                .Where(item => item.UserId == userId)
                .OrderByDescending(item => item.AddedAtUtc)
                .Select(item => item.MovieId)
                .ToList();
            var movies = state.Movies.ToDictionary(movie => movie.Id);
            return (IReadOnlyList<Movie>)orderedIds
                .Where(movies.ContainsKey)
                .Select(id => movies[id])
                .ToList();
        });

    public Task<ViewingProgress?> GetProgressAsync(Guid userId, int movieId) =>
        _database.ReadAsync(state => state.Progress.FirstOrDefault(progress =>
            progress.UserId == userId && progress.MovieId == movieId));

    public Task<IReadOnlyList<ContinueWatchingItemViewModel>> GetContinueWatchingAsync(Guid userId, int take = 10) =>
        _database.ReadAsync(state =>
        {
            var movies = state.Movies.ToDictionary(movie => movie.Id);
            return (IReadOnlyList<ContinueWatchingItemViewModel>)state.Progress
                .Where(progress => progress.UserId == userId && progress.ProgressSeconds > 0)
                .Where(progress => progress.DurationSeconds <= 0 || progress.ProgressSeconds < progress.DurationSeconds * 0.95)
                .Where(progress => movies.ContainsKey(progress.MovieId))
                .OrderByDescending(progress => progress.UpdatedAtUtc)
                .Take(Math.Clamp(take, 1, 30))
                .Select(progress => new ContinueWatchingItemViewModel
                {
                    Movie = movies[progress.MovieId],
                    ProgressSeconds = progress.ProgressSeconds,
                    DurationSeconds = progress.DurationSeconds,
                    UpdatedAtUtc = progress.UpdatedAtUtc
                })
                .ToList();
        });

    public Task SaveProgressAsync(Guid userId, int movieId, int progressSeconds, int durationSeconds) =>
        _database.UpdateAsync(state =>
        {
            if (!state.Movies.Any(movie => movie.Id == movieId))
            {
                return false;
            }

            durationSeconds = Math.Max(1, durationSeconds);
            progressSeconds = Math.Clamp(progressSeconds, 0, durationSeconds);
            var item = state.Progress.FirstOrDefault(progress => progress.UserId == userId && progress.MovieId == movieId);
            if (item is null)
            {
                state.Progress.Add(new ViewingProgress
                {
                    UserId = userId,
                    MovieId = movieId,
                    ProgressSeconds = progressSeconds,
                    DurationSeconds = durationSeconds
                });
            }
            else
            {
                item.ProgressSeconds = progressSeconds;
                item.DurationSeconds = durationSeconds;
                item.UpdatedAtUtc = DateTime.UtcNow;
            }
            return true;
        });

    public Task<int> WatchlistCountAsync() => _database.ReadAsync(state => state.Watchlist.Count);
    public Task<int> WatchlistCountForUserAsync(Guid userId) =>
        _database.ReadAsync(state => state.Watchlist.Count(item => item.UserId == userId));

    public Task<int> ProgressCountAsync() => _database.ReadAsync(state => state.Progress.Count);
    public Task<int> ContinueWatchingCountForUserAsync(Guid userId) =>
        _database.ReadAsync(state => state.Progress.Count(progress =>
            progress.UserId == userId && progress.ProgressSeconds > 0 &&
            (progress.DurationSeconds <= 0 || progress.ProgressSeconds < progress.DurationSeconds * 0.95)));
}
