namespace StreamNova.Models;

public sealed class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.Customer;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
}
