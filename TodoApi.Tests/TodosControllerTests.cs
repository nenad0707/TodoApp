using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TodoApi.Controllers;
using TodoApi.Services;
using TodoLibrary;
using TodoLibrary.Models;

namespace TodoApi.Tests;



public class TodosControllerTests
{
    private readonly TodosController _todosController;
    private readonly Mock<ITodoData> _mockTodoData;
    private readonly Mock<ILogger<TodosController>> _mockLogger;
    private readonly AuthController _authController;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IUserRepository> _mockUserRepository;
    public TodosControllerTests()
    {
        _mockTodoData = new Mock<ITodoData>();
        _mockLogger = new Mock<ILogger<TodosController>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockUserRepository = new Mock<IUserRepository>();

        // Configurations for JWT
        _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns("your_jwt_secret_key");
        _mockConfiguration.SetupGet(c => c["Jwt:Issuer"]).Returns("your_jwt_issuer");
        _mockConfiguration.SetupGet(c => c["Jwt:Audience"]).Returns("your_jwt_audience");

        _authController = new AuthController(_mockConfiguration.Object, _mockUserRepository.Object);

        _todosController = new TodosController(_mockTodoData.Object, _mockLogger.Object);

        _todosController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }, "TestAuthentication"))
            }
        };

    }

    [Fact]
    public void GenerateJwtToken_ReturnsValidToken_ForValidUser()
    {
        // Arrange
        var password = "testPassword";

        _authController.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new TodoApi.Models.User
        {
            Id = 1,
            Username = "testUser",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        using var hmac = new System.Security.Cryptography.HMACSHA512();

        var key = hmac.Key;

        // Use this key for generating the JWT token
        _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns(Convert.ToBase64String(key));

        // Act
        var token = _authController.GenerateJwtToken(user);

        // Assert
        Assert.NotNull(token);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithListOfTodos()
    {
        // Arrange

        var todos = new List<TodoModel>
        {
            new TodoModel { Id = 1, Task = "Test Todo 1", IsCompleted = false },
            new TodoModel { Id = 2, Task = "Test Todo 2", IsCompleted = true },

        };

        _mockTodoData.Setup(d => d.GetAllAssigned(It.IsAny<int>())).ReturnsAsync(todos);

        // Act
        var result = await _todosController.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<TodoModel>>(okResult.Value);
        Assert.Equal(todos.Count, returnValue.Count);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithListOfTodos_WhenTokenIsValid()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todos = new List<TodoModel>
        {
            new TodoModel { Id = 1, Task = "Test Todo 1", IsCompleted = false },
            new TodoModel { Id = 2, Task = "Test Todo 2", IsCompleted = true },

        };

        _mockTodoData.Setup(d => d.GetAllAssigned(It.IsAny<int>())).ReturnsAsync(todos);

        // Act
        var result = await _todosController.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnValue = Assert.IsType<List<TodoModel>>(okResult.Value);

        Assert.Equal(todos.Count, returnValue.Count);
    }

    [Fact]
    public async Task GetTodoById_ReturnsOkResult_WithTodoModel_WhenTodoExists()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 1;

        var todo = new TodoModel { Id = todoId, Task = "Test Todo", IsCompleted = false };

        _mockTodoData.Setup(d => d.GetOneAssigned(It.IsAny<int>(), todoId)).ReturnsAsync(todo);

        // Act
        var result = await _todosController.Get(todoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnValue = Assert.IsType<TodoModel>(okResult.Value);

        Assert.Equal(todo.Id, returnValue.Id);

        Assert.Equal(todo.Task, returnValue.Task);

        Assert.Equal(todo.IsCompleted, returnValue.IsCompleted);
    }

    [Fact]
    public async Task GetTodoById_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 1;

        _mockTodoData.Setup(d => d.GetOneAssigned(It.IsAny<int>(), todoId)).ReturnsAsync((TodoModel)null!);

        // Act
        var result = await _todosController.Get(todoId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Post_CreatesNewTodo_ReturnsCreatedTodo()
    {
        // Arrange
        var taskDescription = "Test Task";

        var isCompleted = false;

        var newTodo = new TodoModel { Id = 3, Task = taskDescription, IsCompleted = isCompleted };

        _mockTodoData.Setup(d => d.Create(It.IsAny<int>(), taskDescription, isCompleted)).ReturnsAsync(newTodo);

        // Act
        var result = await _todosController.Post(taskDescription, isCompleted);

        // Assert
        var createdAtActionResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnValue = Assert.IsType<TodoModel>(createdAtActionResult.Value);

        Assert.Equal(newTodo.Id, returnValue.Id);

        Assert.Equal(newTodo.Task, returnValue.Task);
        Assert.Equal(newTodo.IsCompleted, returnValue.IsCompleted);
    }

    [Fact]
    public async Task Put_NoUpdate_ReturnsNotFound()
    {
        // Arrange
        var todoId = 5;

        var updatedTask = "Updated Test Task";

        var affectedRows = 0;

        _mockTodoData.Setup(d => d.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(affectedRows);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        Assert.NotNull(messageProperty); // This will fail if 'message' property does not exist

        if (messageProperty != null)
        {
            Assert.Equal("Task not found or no changes made.", messageProperty.GetValue(returnValue));
        }
    }

    [Fact]
    public async Task Put_UpdatesTodo_ReturnsSuccessMessage()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 5;

        var updatedTask = "Updated Test Task";

        var affectedRows = 1;

        _mockTodoData.Setup(d => d.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(affectedRows);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(okResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        var affectedRowsProperty = type.GetProperty("affectedRows");

        Assert.NotNull(messageProperty);

        Assert.NotNull(affectedRowsProperty);

        Assert.Equal("Task updated successfully.", messageProperty.GetValue(returnValue));

        Assert.Equal(affectedRows, affectedRowsProperty.GetValue(returnValue));

    }

    [Fact]
    public async Task Delete_NoTodoFound_ReturnsNotFound()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 5;

        var rowsAffected = 0;

        _mockTodoData.Setup(d => d.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(rowsAffected);

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        Assert.NotNull(messageProperty);

        Assert.Equal("Todo not found.", messageProperty.GetValue(returnValue));

    }


    [Fact]
    public async Task Complete_MarksTodoAsComplete_ReturnsSuccessMessage()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 5;

        var rowsAffected = 1;

        _mockTodoData.Setup(d => d.CompleteTodo(It.IsAny<int>(), todoId)).ReturnsAsync(rowsAffected);

        // Act
        var result = await _todosController.Complete(todoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(okResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        Assert.NotNull(messageProperty); // Check that the 'message' property exists

        var messageValue = messageProperty.GetValue(returnValue);

        Assert.NotNull(messageValue); // Check that the 'message' property has a value

        Assert.Equal("Todo marked as complete successfully.", messageProperty.GetValue(returnValue));
    }

    [Fact]
    public async Task Complete_NoTodoFound_ReturnsNotFound()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 5;

        var rowsAffected = 0; // Pretpostavimo da nijedan red nije ažuriran

        _mockTodoData.Setup(d => d.CompleteTodo(It.IsAny<int>(), todoId)).ReturnsAsync(rowsAffected);

        // Act
        var result = await _todosController.Complete(todoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        Assert.NotNull(messageProperty);

        Assert.Equal("Todo not found or already completed.", messageProperty.GetValue(returnValue));
    }

    [Fact]
    public async Task Delete_RemovesTodo_ReturnsSuccessMessage()
    {
        // Arrange
        var fakeToken = GenerateFakeJwtToken();

        var authHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken).ToString();

        _todosController.ControllerContext.HttpContext.Request.Headers.Authorization = authHeaderValue;

        var todoId = 5;

        var rowsAffected = 1;

        _mockTodoData.Setup(d => d.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(rowsAffected);

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnValue = Assert.IsAssignableFrom<object>(okResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();

        var messageProperty = type.GetProperty("message");

        Assert.NotNull(messageProperty); // Check that the 'message' property exists

        var messageValue = messageProperty.GetValue(returnValue);

        Assert.NotNull(messageValue); // Check that the 'message' property has a value

        Assert.Equal("Todo deleted successfully.", messageValue.ToString()); // Check that the value is what you expect

    }

    private string GenerateFakeJwtToken()
    {
        var password = "testPassword";

        _authController.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new TodoApi.Models.User
        {
            Id = 1,
            Username = "testUser",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        // Use an empty constructor to generate a key of length 512 bits
        using var hmac = new System.Security.Cryptography.HMACSHA512();

        var key = hmac.Key;

        // Use this key for generating the JWT token
        _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns(Convert.ToBase64String(key));

        return _authController.GenerateJwtToken(user);
    }
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();

        passwordSalt = hmac.Key;

        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

}
