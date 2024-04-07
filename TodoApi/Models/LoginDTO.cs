namespace TodoApi.Models;


/// <summary>
/// Represents the login data transfer object.
/// </summary>
public class LoginDTO
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
