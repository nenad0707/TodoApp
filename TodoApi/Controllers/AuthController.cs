using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;



/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="userRepository">The user repository.</param>
    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Logs in a user with the provided credentials.
    /// </summary>
    /// <param name="loginDTO">The login credentials.</param>
    /// <returns>The generated JWT token.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO loginDTO)
    {
        var user = _userRepository.GetUserByUsername(loginDTO.Username!);
        if (user == null || !VerifyPasswordHash(loginDTO.Password!, user.PasswordHash!, user.PasswordSalt!))
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDTO">The user registration details.</param>
    /// <returns>The HTTP status code indicating the result of the registration.</returns>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDTO? registerDTO)
    {
        if (_userRepository.GetUserByUsername(registerDTO!.Username!) != null)
        {
            return BadRequest("Username is already taken.");
        }

        CreatePasswordHash(registerDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User
        {
            Username = registerDTO.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _userRepository.AddUser(user);
        return StatusCode(201);
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>The HTTP status code indicating the result of the deletion.</returns>
    [HttpDelete("delete/{id}")]
    public IActionResult Delete(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        _userRepository.DeleteUser(id);
        return Ok();
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The generated JWT token.</returns>
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, user.Username!),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Creates a password hash and salt for the specified password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="passwordHash">The generated password hash.</param>
    /// <param name="passwordSalt">The generated password salt.</param>
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Verifies the password hash for the specified password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="passwordHash">The password hash to verify against.</param>
    /// <param name="passwordSalt">The password salt.</param>
    /// <returns><c>true</c> if the password hash is valid; otherwise, <c>false</c>.</returns>
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != passwordHash[i]) return false;
        }
        return true;
    }
}
