using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        // Mock IConfiguration
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.SetupGet(c => c["Jwt:Key"]).Returns("your_secret_test_key_here_very_secret_key");
        _configurationMock.SetupGet(c => c["Jwt:Issuer"]).Returns("test_issuer");
        _configurationMock.SetupGet(c => c["Jwt:Audience"]).Returns("test_audience");

        // Mock IUserRepository
        _userRepositoryMock = new Mock<IUserRepository>();

        // Create AuthController
        _authController = new AuthController(_configurationMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public void Register_ShouldCreateUser_WhenGivenValidData()
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = "new_user", Password = "new_password" };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername(registerDTO.Username)).Returns((User?)null!);

        // Act
        var result = _authController.Register(registerDTO);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        var statusCodeResult = result as StatusCodeResult;
        Assert.Equal(201, statusCodeResult!.StatusCode);
    }


    [Fact]
    public void Login_ShouldReturnToken_WhenGivenValidCredentials()
    {
        // Arrange
        var password = "correct_password";
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        var loginDTO = new LoginDTO { Username = "existing_user", Password = password };
        var user = new User { Username = loginDTO.Username, Id = 1, PasswordHash = passwordHash, PasswordSalt = passwordSalt };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginDTO.Username)).Returns(user);

        // Act
        var result = _authController.Login(loginDTO);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void Delete_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "user_to_delete", Id = 1 };
        _userRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).Returns(user);

        // Act
        var result = _authController.Delete(user.Id);

        // Assert
        Assert.IsType<OkResult>(result);
        // Proverite da li je korisnik uklonjen iz repozitorijuma
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}
