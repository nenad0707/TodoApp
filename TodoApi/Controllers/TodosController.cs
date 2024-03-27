using Microsoft.AspNetCore.Mvc;
using TodoLibrary;
using TodoLibrary.Models;
using System.Text.Json;
using System.Security.Claims;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodoData _data;
    private readonly int userId;

    public TodosController(ITodoData data)
    {
        _data = data;
        var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        userId = int.Parse(userIdText!);
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoModel>>> Get()
    {
        var output = _data.GetAllAssigned(userId);
        return Ok(output);
    }


    // GET api/Todos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        var output = await _data.GetOneAssigned(userId, todoId);
        return Ok(output);
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        var output = await _data.Create(userId, task);
        return Ok(output);
    }

    // PUT api/Todos/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // PUT api/Todos/5/Complete
    [HttpPut("{id}/complete")]
    public IActionResult Complete(int id)
    {
        throw new NotImplementedException();
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        throw new NotImplementedException();
    }
}
