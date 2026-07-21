using StreamNova.Models;

namespace StreamNova.Services;

public sealed class MovieService
{
    private readonly JsonDatabase _database;

    public MovieService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyList<Movie>> GetAllAsync() =>
        _database.ReadAsync(state => (IReadOnlyList<Movie>)state.Movies.OrderBy(movie => movie.Title).ToList());

    public Task<Movie?> GetAsync(int id) =>
        _database.ReadAsync(state => state.Movies.FirstOrDefault(movie => movie.Id == id));

    public Task<IReadOnlyList<Movie>> SearchAsync(string? query, string? genre) =>
        _database.ReadAsync(state =>
        {
            IEnumerable<Movie> movies = state.Movies;
            if (!string.IsNullOrWhiteSpace(query))
            {
                movies = movies.Where(movie =>
                    movie.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    movie.Synopsis.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    movie.Genre.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                movies = movies.Where(movie =>
                    movie.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
            }

            return (IReadOnlyList<Movie>)movies.OrderByDescending(movie => movie.Year).ToList();
        });

    public Task<IReadOnlyList<string>> GetGenresAsync() =>
        _database.ReadAsync(state => (IReadOnlyList<string>)state.Movies
            .Select(movie => movie.Genre)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(genre => genre)
            .ToList());

    public Task<Movie> SaveAsync(Movie movie)
    {
        return _database.UpdateAsync(state =>
        {
            if (movie.Id == 0)
            {
                movie.Id = state.Movies.Count == 0 ? 1 : state.Movies.Max(item => item.Id) + 1;
                state.Movies.Add(movie);
            }
            else
            {
                var index = state.Movies.FindIndex(item => item.Id == movie.Id);
                if (index < 0)
                {
                    throw new InvalidOperationException("Movie not found.");
                }
                state.Movies[index] = movie;
            }

            if (movie.Featured)
            {
                foreach (var item in state.Movies.Where(item => item.Id != movie.Id))
                {
                    item.Featured = false;
                }
            }

            return movie;
        });
    }

    public Task<bool> DeleteAsync(int id) => _database.UpdateAsync(state =>
    {
        var removed = state.Movies.RemoveAll(movie => movie.Id == id) > 0;
        state.Watchlist.RemoveAll(item => item.MovieId == id);
        state.Progress.RemoveAll(item => item.MovieId == id);
        state.Reviews.RemoveAll(item => item.MovieId == id);
        return removed;
    });

    public Task<int> CountAsync() => _database.ReadAsync(state => state.Movies.Count);
}
