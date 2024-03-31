using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;
using TodoLibrary;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodoData _data;

    public TodosController(ITodoData data)
    {
        _data = data;
    }

    private int GetUserId()
    {
        
        var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdText);
    }

    // GET: api/Todos
    [HttpGet]
    public async Task<ActionResult<List<TodoModel>>> Get()
    {
        var output = await _data.GetAllAssigned(GetUserId());

        return Ok(output);
    }

    // GET api/Todos/5
    [HttpGet("{todoId}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        var output = await _data.GetOneAssigned(GetUserId(), todoId);

        return Ok(output);
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task, bool isCompleted)
    {
        var output = await _data.Create(GetUserId(), task, isCompleted);

        return Ok(output);
    }

    // PUT api/Todos/5
    [HttpPut("{todoId}")]
    public async Task<ActionResult> Put(int todoId, [FromBody] string task)
    {
        await _data.UpdateTask(GetUserId(), todoId, task);

        return Ok();
    }

    // PUT api/Todos/5/Complete
    [HttpPut("{todoId}/Complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        await _data.CompleteTodo(GetUserId(), todoId);

        return Ok();
    }

    // DELETE api/Todos/5
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        await _data.Delete(GetUserId(), todoId);

        return Ok();
    }
}
