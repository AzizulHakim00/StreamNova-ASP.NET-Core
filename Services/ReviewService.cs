using StreamNova.Models;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class ReviewService
{
    private readonly JsonDatabase _database;

    public ReviewService(JsonDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyList<MovieReviewViewModel>> GetForMovieAsync(int movieId, Guid? currentUserId) =>
        _database.ReadAsync(state =>
        {
            var users = state.Users.ToDictionary(user => user.Id);
            return (IReadOnlyList<MovieReviewViewModel>)state.Reviews
                .Where(review => review.MovieId == movieId)
                .OrderByDescending(review => review.UpdatedAtUtc)
                .Select(review => new MovieReviewViewModel
                {
                    Id = review.Id,
                    ReviewerName = users.TryGetValue(review.UserId, out var user) ? user.Name : "StreamNova viewer",
                    Rating = review.Rating,
                    Comment = review.Comment,
                    UpdatedAtUtc = review.UpdatedAtUtc,
                    IsOwner = currentUserId.HasValue && review.UserId == currentUserId.Value
                })
                .ToList();
        });

    public Task<MovieReview?> GetUserReviewAsync(Guid userId, int movieId) =>
        _database.ReadAsync(state => state.Reviews.FirstOrDefault(review =>
            review.UserId == userId && review.MovieId == movieId));

    public Task<(double Average, int Count)> GetSummaryAsync(int movieId) =>
        _database.ReadAsync(state =>
        {
            var reviews = state.Reviews.Where(review => review.MovieId == movieId).ToList();
            return reviews.Count == 0
                ? (0d, 0)
                : (Math.Round(reviews.Average(review => review.Rating), 1), reviews.Count);
        });

    public Task<bool> SaveAsync(Guid userId, int movieId, int rating, string comment) =>
        _database.UpdateAsync(state =>
        {
            if (!state.Users.Any(user => user.Id == userId) || !state.Movies.Any(movie => movie.Id == movieId))
            {
                return false;
            }

            rating = Math.Clamp(rating, 1, 5);
            comment = comment.Trim();
            var review = state.Reviews.FirstOrDefault(item => item.UserId == userId && item.MovieId == movieId);
            if (review is null)
            {
                state.Reviews.Add(new MovieReview
                {
                    UserId = userId,
                    MovieId = movieId,
                    Rating = rating,
                    Comment = comment
                });
            }
            else
            {
                review.Rating = rating;
                review.Comment = comment;
                review.UpdatedAtUtc = DateTime.UtcNow;
            }

            return true;
        });

    public Task<bool> DeleteAsync(Guid userId, Guid reviewId, bool isAdmin) =>
        _database.UpdateAsync(state =>
        {
            var review = state.Reviews.FirstOrDefault(item => item.Id == reviewId);
            if (review is null || (!isAdmin && review.UserId != userId))
            {
                return false;
            }

            state.Reviews.Remove(review);
            return true;
        });

    public Task<int> CountAsync() => _database.ReadAsync(state => state.Reviews.Count);

    public Task<int> CountForUserAsync(Guid userId) =>
        _database.ReadAsync(state => state.Reviews.Count(review => review.UserId == userId));
}
