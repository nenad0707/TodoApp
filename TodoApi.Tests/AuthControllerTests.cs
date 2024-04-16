using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;
public class AuthControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly AuthController _authController;
    private readonly Mock<IConfiguration> _configurationMock;

    public AuthControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _configurationMock = new Mock<IConfiguration>();

        // Create AuthController
        _authController = new AuthController(_configurationMock.Object, _userRepositoryMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public void Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = "testpassword" };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns((User)null!);

        // Act
        var result = _authController.Login(loginDTO);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Theory]
    [InlineData("testpassword", true)]
    [InlineData("testpassword", false)]
    public void Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect(string loginPassword, bool expected)
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = loginPassword };
        var user = new User { Username = "testuser", PasswordHash = new byte[0], PasswordSalt = new byte[0] };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns(user);
        _authServiceMock.Setup(auth => auth.VerifyPasswordHash(loginPassword, user.PasswordHash, user.PasswordSalt)).Returns(expected);

        // Act
        var result = _authController.Login(loginDTO);

        // Assert
        if (expected)
        {
            Assert.IsType<OkObjectResult>(result);
        }
        else
        {
            Assert.IsType<UnauthorizedResult>(result);
        }
    }

    [Theory]
    [InlineData("testpassword")]
    public void Login_ShouldReturnOk_WhenUserFound(string loginPassword)
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = loginPassword };
        var user = new User { Username = "testuser", PasswordHash = new byte[0], PasswordSalt = new byte[0] };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns(user);
        _authServiceMock.Setup(auth => auth.VerifyPasswordHash(loginPassword, user.PasswordHash, user.PasswordSalt)).Returns(true);
        _authServiceMock.Setup(auth => auth.GenerateJwtToken(user)).Returns("test_token");

        // Act
        var result = _authController.Login(loginDTO);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData("testuser")]
    public void Register_ShouldReturnBadRequest_WhenUserExists(string username)
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = username, Password = "testpassword" };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername(username)).Returns(new User());

        // Act
        var result = _authController.Register(registerDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory]
    [InlineData("testuser")]
    public void Register_ShouldReturnStatusCode201_WhenUserDoesNotExist(string username)
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = username, Password = "testpassword" };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername(username)).Returns((User)null!);

        // Act
        var result = _authController.Register(registerDTO);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
    }

    [Theory]
    [InlineData("testpassword")]
    public void Register_ShouldAddUser_WhenUserDoesNotExist(string password)
    {
        // Arrange
        var registerDTO = new RegisterDTO { Username = "testuser", Password = password };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns((User)null!);

        // Act
        var result = _authController.Register(registerDTO);

        // Assert
        _userRepositoryMock.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public void Delete_ShouldReturnNotFound_WhenUserDoesNotExist(int userId)
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns((User)null!);

        // Act
        var result = _authController.Delete(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
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
    }

    [Fact]
    public void Delete_ShouldCallDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "user_to_delete", Id = 1 };
        _userRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).Returns(user);

        // Act
        var result = _authController.Delete(user.Id);

        // Assert
        _userRepositoryMock.Verify(repo => repo.DeleteUser(user.Id), Times.Once);
    }

    [Fact]
    public void Delete_ShouldNotCallDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistingUserId = 1;
        _userRepositoryMock.Setup(repo => repo.GetUserById(nonExistingUserId)).Returns((User)null!);

        // Act
        var result = _authController.Delete(nonExistingUserId);

        // Assert
        _userRepositoryMock.Verify(repo => repo.DeleteUser(nonExistingUserId), Times.Never);
    }

    [Fact]
    public void Login_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // Arrange
        var loginDTO = new LoginDTO { Username = "testuser", Password = "testpassword" };
        _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns((User)null!);

        // Act
        var result = _authController.Login(loginDTO);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
}

