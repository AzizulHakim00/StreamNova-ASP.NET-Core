using StreamNova.Models;

namespace StreamNova.Services;

public sealed class RecommendationService
{
    private readonly JsonDatabase _database;

    public RecommendationService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyList<Movie>> GetForUserAsync(Guid userId, int take = 10) =>
        _database.ReadAsync(state =>
        {
            var genreWeights = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var interactedMovieIds = new HashSet<int>();

            foreach (var item in state.Watchlist.Where(item => item.UserId == userId))
            {
                interactedMovieIds.Add(item.MovieId);
                AddGenreWeight(item.MovieId, 2.5);
            }

            foreach (var item in state.Progress.Where(item => item.UserId == userId))
            {
                interactedMovieIds.Add(item.MovieId);
                var completion = item.DurationSeconds <= 0 ? 0 : (double)item.ProgressSeconds / item.DurationSeconds;
                AddGenreWeight(item.MovieId, completion >= .7 ? 4 : 2);
            }

            foreach (var review in state.Reviews.Where(item => item.UserId == userId))
            {
                interactedMovieIds.Add(review.MovieId);
                AddGenreWeight(review.MovieId, Math.Max(1, review.Rating - 1));
            }

            var recommendations = state.Movies
                .Where(movie => !interactedMovieIds.Contains(movie.Id))
                .Select(movie => new
                {
                    Movie = movie,
                    Rank = (genreWeights.TryGetValue(movie.Genre, out var weight) ? weight : 0)
                           + movie.Score / 10
                           + (movie.Trending ? 1.5 : 0)
                           + (movie.NewRelease ? .75 : 0)
                })
                .OrderByDescending(item => item.Rank)
                .ThenByDescending(item => item.Movie.Score)
                .Take(take)
                .Select(item => item.Movie)
                .ToList();

            if (recommendations.Count == 0)
            {
                recommendations = state.Movies
                    .OrderByDescending(movie => movie.Trending)
                    .ThenByDescending(movie => movie.Score)
                    .Take(take)
                    .ToList();
            }

            return (IReadOnlyList<Movie>)recommendations;

            void AddGenreWeight(int movieId, double amount)
            {
                var movie = state.Movies.FirstOrDefault(item => item.Id == movieId);
                if (movie is null)
                {
                    return;
                }

                genreWeights[movie.Genre] = genreWeights.GetValueOrDefault(movie.Genre) + amount;
            }
        });
}
