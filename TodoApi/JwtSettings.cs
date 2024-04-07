namespace TodoApi;


/// <summary>
/// Represents the settings for JWT authentication.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the key used for JWT encryption.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the issuer of the JWT.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Gets or sets the audience of the JWT.
    /// </summary>
    public string? Audience { get; set; }
}
