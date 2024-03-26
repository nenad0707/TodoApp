using System.Security.Cryptography.X509Certificates;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoLibrary;

public class TodoData
{
  private readonly ISqlDataAccess _sql;

  public TodoData(ISqlDataAccess sql)
  {
    this._sql = sql;
  }

  public Task<List<TodoModel>> GetAllAssigned(int assignedTo)
  {
    return _sql.LoadData<TodoModel, dynamic>("dbo.spTodos_GetAllAssigned", new { AssignedTo = assignedTo }, "Default");
  }
}
