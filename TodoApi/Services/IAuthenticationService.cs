using TodoApi.Models;

namespace TodoApi.Services;


/// <summary>
/// Represents an interface for authentication service.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>The generated JWT token.</returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Verifies the password hash and salt against the specified password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The password hash to compare.</param>
    /// <param name="passwordSalt">The password salt to compare.</param>
    /// <returns>True if the password is verified; otherwise, false.</returns>
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);

    /// <summary>
    /// Creates the password hash and salt for the specified password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="passwordHash">The generated password hash.</param>
    /// <param name="passwordSalt">The generated password salt.</param>
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
}
