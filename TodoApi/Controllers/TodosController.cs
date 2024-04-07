using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoLibrary;
using TodoLibrary.Models;

namespace TodoApi.Controllers;

/// <summary>
/// Controller for managing Todo items.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodoData _data;
    private readonly ILogger<TodosController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodosController"/> class.
    /// </summary>
    /// <param name="data">The ITodoData implementation.</param>
    /// <param name="logger">The logger.</param>
    public TodosController(ITodoData data, ILogger<TodosController> logger)
    {
        _data = data;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdText!);
    }

    // GET: api/Todos
    /// <summary>
    /// Get all Todo items for the current user.
    /// </summary>
    /// <returns>List of Todo items.</returns>
    [HttpGet]
    public async Task<ActionResult<List<TodoModel>>> Get()
    {
        _logger.LogInformation("GET api/Todos called.");

        try
        {
            var output = await _data.GetAllAssigned(GetUserId());

            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Todos failed.");
            return BadRequest();
        }
    }

    // GET api/Todos/5
    /// <summary>
    /// Get a specific Todo item by its ID.
    /// </summary>
    /// <param name="todoId">The ID of the Todo item.</param>
    /// <returns>The Todo item.</returns>
    [HttpGet("{todoId}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        _logger.LogInformation("GET api/Todos/{TodoId} called.", todoId);

        try
        {
            var output = await _data.GetOneAssigned(GetUserId(), todoId);

            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "The GET call to {ApiPath} failed. The Id was {TodoId}",
                $"api/Todos/Id",
                todoId);
            return BadRequest();
        }
    }

    // POST api/Todos
    /// <summary>
    /// Create a new Todo item.
    /// </summary>
    /// <param name="task">The task description.</param>
    /// <param name="isCompleted">Whether the task is completed.</param>
    /// <returns>The created Todo item.</returns>
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task, bool isCompleted)
    {
        _logger.LogInformation("POST api/Todos called. Task: {Task}", task);

        try
        {
            var output = await _data.Create(GetUserId(), task, isCompleted);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The POST call to api/Todos failed. The task value was {Task}", task);
            return BadRequest();
        }
    }

    // PUT api/Todos/5
    /// <summary>
    /// Update a specific Todo item.
    /// </summary>
    /// <param name="todoId">The ID of the Todo item.</param>
    /// <param name="task">The updated task description.</param>
    /// <returns>An ActionResult.</returns>
    [HttpPut("{todoId}")]
    public async Task<ActionResult> Put(int todoId, [FromBody] string task)
    {
        _logger.LogInformation("PUT api/Todos/{TodoId} called. Task: {Task}", todoId, task);

        try
        {
            await _data.UpdateTask(GetUserId(), todoId, task);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Todos/{TodoId} failed. The task value was {Task}", todoId, task);
            return BadRequest();
        }
    }

    // PUT api/Todos/5/Complete
    /// <summary>
    /// Mark a specific Todo item as completed.
    /// </summary>
    /// <param name="todoId">The ID of the Todo item.</param>
    /// <returns>An IActionResult.</returns>
    [HttpPut("{todoId}/Complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        _logger.LogInformation("PUT api/Todos/{TodoId}/Complete called.", todoId);

        try
        {
            await _data.CompleteTodo(GetUserId(), todoId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Todos/{TodoId}/Complete failed.", todoId);
            return BadRequest();
        }
    }

    // DELETE api/Todos/5
    /// <summary>
    /// Delete a specific Todo item.
    /// </summary>
    /// <param name="todoId">The ID of the Todo item.</param>
    /// <returns>An IActionResult.</returns>
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        _logger.LogInformation("DELETE api/Todos/{TodoId} called.", todoId);

        try
        {
            var rowsAffected = await _data.Delete(GetUserId(), todoId);

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The DELETE call to api/Todos/{TodoId} failed.", todoId);
            return BadRequest();
        }
    }
}
