using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TodoApi.Controllers;
using TodoLibrary.Models;
using TodoLibrary;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;


namespace TodoApi.Tests;
public class TodosControllerTests
{
    private readonly Mock<ITodoData> _todoDataMock;
    private readonly Mock<ILogger<TodosController>> _loggerMock;
    private readonly TodosController _todosController;
    private readonly ControllerContext _controllerContext;

    public TodosControllerTests()
    {
        _todoDataMock = new Mock<ITodoData>();
        _loggerMock = new Mock<ILogger<TodosController>>();
        _todosController = new TodosController(_todoDataMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }, "mock"));

        _controllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        _todosController.ControllerContext = _controllerContext;
    }



    [Fact]
    public async Task Post_ShouldReturnCreatedTodo_WhenSuccessful()
    {
        // Arrange
        var newTask = "New Task";
        var isCompleted = false;
        var newTodo = new TodoModel { Id = 3, Task = newTask, AssignedTo = 1, IsCompleted = isCompleted };
        _todoDataMock.Setup(data => data.Create(It.IsAny<int>(), newTask, isCompleted)).ReturnsAsync(newTodo);

        // Act
        var result = await _todosController.Post(newTask, isCompleted);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTodo = Assert.IsType<TodoModel>(okResult.Value);
        Assert.Equal(newTask, returnedTodo.Task);
        Assert.Equal(isCompleted, returnedTodo.IsCompleted);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var newTask = "New Task";
        var isCompleted = false;
        _todoDataMock.Setup(data => data.Create(It.IsAny<int>(), newTask, isCompleted)).ThrowsAsync(new Exception());

        // Act
        var result = await _todosController.Post(newTask, isCompleted);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task Put_ShouldReturnOk_WhenTaskIsUpdated()
    {
        // Arrange
        var todoId = 1;
        var updatedTask = "Updated Task";
        _todoDataMock.Setup(data => data.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(1);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(okResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Task updated successfully.", messageValue);

        var affectedRowsProperty = type.GetProperty("affectedRows");
        Assert.NotNull(affectedRowsProperty); // Check that the 'affectedRows' property exists
        var affectedRowsValue = affectedRowsProperty.GetValue(returnValue);
        Assert.NotNull(affectedRowsValue); // Check that the 'affectedRows' property has a value
        Assert.Equal(1, affectedRowsValue);
    }


    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenNoTaskIsUpdated()
    {
        // Arrange
        var todoId = 1;
        var updatedTask = "Updated Task";
        _todoDataMock.Setup(data => data.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(0);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Task not found or no changes made.", messageValue);
    }



    [Fact]
    public async Task Complete_ShouldReturnOk_WhenTodoIsMarkedAsComplete()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.CompleteTodo(It.IsAny<int>(), todoId)).ReturnsAsync(1);

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
    public async Task Complete_ShouldReturnNotFound_WhenTodoIsNotUpdated()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.CompleteTodo(It.IsAny<int>(), todoId)).ReturnsAsync(0);

        // Act
        var result = await _todosController.Complete(todoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Todo not found or already completed.", messageValue);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenTodoIsDeleted()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(1);

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
        Assert.Equal("Todo deleted successfully.", messageValue);

        var rowsAffectedProperty = type.GetProperty("rowsAffected");
        Assert.NotNull(rowsAffectedProperty); // Check that the 'rowsAffected' property exists
        var rowsAffectedValue = rowsAffectedProperty.GetValue(returnValue);
        Assert.NotNull(rowsAffectedValue); // Check that the 'rowsAffected' property has a value
        Assert.Equal(1, rowsAffectedValue);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTodoIsNotDeleted()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(0);

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Todo not found.", messageValue);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ThrowsAsync(new Exception());

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenTodoIsNull()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.GetOneAssigned(It.IsAny<int>(), todoId)).ReturnsAsync((TodoModel)null!);

        // Act
        var result = await _todosController.Get(todoId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TodoModel>>(result);
        Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task Delete_RemovesTodo_ReturnsSuccessMessage()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(1);

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
        Assert.Equal("Todo deleted successfully.", messageValue);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenTodoIsNull()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ReturnsAsync(0);

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Todo not found.", messageValue);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var todoId = 1;
        _todoDataMock.Setup(data => data.Delete(It.IsAny<int>(), todoId)).ThrowsAsync(new Exception());

        // Act
        var result = await _todosController.Delete(todoId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Put_UpdatesTodo_ReturnsSuccessMessage()
    {
        // Arrange
        var todoId = 1;
        var updatedTask = "Updated Task";
        _todoDataMock.Setup(data => data.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(1);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(okResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Task updated successfully.", messageValue);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNoTaskIsUpdated()
    {
        // Arrange
        var todoId = 1;
        var updatedTask = "Updated Task";
        _todoDataMock.Setup(data => data.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ReturnsAsync(0);

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<object>(notFoundResult.Value);

        // Use reflection to check the properties of the anonymous type
        var type = returnValue.GetType();
        var messageProperty = type.GetProperty("message");
        Assert.NotNull(messageProperty); // Check that the 'message' property exists
        var messageValue = messageProperty.GetValue(returnValue);
        Assert.NotNull(messageValue); // Check that the 'message' property has a value
        Assert.Equal("Task not found or no changes made.", messageValue);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var todoId = 1;
        var updatedTask = "Updated Task";
        _todoDataMock.Setup(data => data.UpdateTask(It.IsAny<int>(), todoId, updatedTask)).ThrowsAsync(new Exception());

        // Act
        var result = await _todosController.Put(todoId, updatedTask);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
