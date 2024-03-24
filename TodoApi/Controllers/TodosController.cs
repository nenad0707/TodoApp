using Microsoft.AspNetCore.Mvc;


namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    // GET: api/Todos
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/Todos/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/Todos
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/Todos/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
