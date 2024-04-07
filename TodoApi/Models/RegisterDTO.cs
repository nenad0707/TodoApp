namespace TodoApi.Models;


/// <summary>
/// Represents the data transfer object for user registration.
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string? Password { get; set; }
}
