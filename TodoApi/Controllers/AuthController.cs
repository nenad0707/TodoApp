using Microsoft.AspNetCore.Mvc;
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
    private readonly IAuthenticationService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="authService">The authentication service.</param>
    public AuthController(IConfiguration configuration, IUserRepository userRepository, IAuthenticationService authService)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _authService = authService;
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
        if (user == null || !_authService.VerifyPasswordHash(loginDTO.Password!, user.PasswordHash!, user.PasswordSalt!))
        {
            return Unauthorized();
        }

        var token = _authService.GenerateJwtToken(user);
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

        _authService.CreatePasswordHash(registerDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
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
}
