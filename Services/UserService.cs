using StreamNova.Models;
using StreamNova.ViewModels;

namespace StreamNova.Services;

public sealed class UserService
{
    private readonly JsonDatabase _database;
    private readonly PasswordService _passwordService;

    public UserService(JsonDatabase database, PasswordService passwordService)
    {
        _database = database;
        _passwordService = passwordService;
    }

    public Task<AppUser?> FindByEmailAsync(string email) =>
        _database.ReadAsync(state => state.Users.FirstOrDefault(
            user => user.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)));

    public Task<AppUser?> FindByIdAsync(Guid id) =>
        _database.ReadAsync(state => state.Users.FirstOrDefault(user => user.Id == id));

    public async Task<AppUser?> ValidateAsync(string email, string password)
    {
        var user = await FindByEmailAsync(email);
        if (user is null || !_passwordService.Verify(password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        return user;
    }

    public Task<(bool Success, string Error, AppUser? User)> RegisterAsync(
        string name,
        string email,
        string password)
    {
        email = email.Trim().ToLowerInvariant();
        return _database.UpdateAsync(state =>
        {
            if (state.Users.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "An account with this email already exists.", (AppUser?)null);
            }

            var encoded = _passwordService.Hash(password);
            var user = new AppUser
            {
                Name = name.Trim(),
                Email = email,
                PasswordHash = encoded.Hash,
                PasswordSalt = encoded.Salt,
                Role = UserRoles.Customer
            };
            state.Users.Add(user);
            return (true, string.Empty, user);
        });
    }

    public Task<AppUser?> UpdateNameAsync(Guid userId, string name) =>
        _database.UpdateAsync(state =>
        {
            var user = state.Users.FirstOrDefault(item => item.Id == userId);
            if (user is null)
            {
                return null;
            }

            user.Name = name.Trim();
            return user;
        });

    public Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword) =>
        _database.UpdateAsync(state =>
        {
            var user = state.Users.FirstOrDefault(item => item.Id == userId);
            if (user is null || !_passwordService.Verify(currentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return false;
            }

            var encoded = _passwordService.Hash(newPassword);
            user.PasswordHash = encoded.Hash;
            user.PasswordSalt = encoded.Salt;
            return true;
        });

    public Task<IReadOnlyList<AdminUserViewModel>> GetAdminUsersAsync() =>
        _database.ReadAsync(state => (IReadOnlyList<AdminUserViewModel>)state.Users
            .OrderByDescending(user => user.CreatedAtUtc)
            .Select(user => new AdminUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAtUtc = user.CreatedAtUtc,
                WatchlistCount = state.Watchlist.Count(item => item.UserId == user.Id),
                ReviewCount = state.Reviews.Count(review => review.UserId == user.Id)
            })
            .ToList());

    public Task<int> CountAsync() => _database.ReadAsync(state => state.Users.Count);
}
