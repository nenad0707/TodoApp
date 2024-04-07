using System.Security.Cryptography.X509Certificates;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoLibrary;

/// <summary>
/// Provides data access methods for todo items.
/// </summary>
public class TodoData : ITodoData
{
    private readonly ISqlDataAccess _sql;

    public TodoData(ISqlDataAccess sql)
    {
        this._sql = sql;
    }

    /// <summary>
    /// Retrieves a list of all assigned todo items for a specific user.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo items are assigned.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of TodoModel objects.</returns>
    public Task<List<TodoModel>> GetAllAssigned(int assignedTo)
    {
        return _sql.LoadData<TodoModel, dynamic>("dbo.spTodos_GetAllAssigned", new { AssignedTo = assignedTo }, "Default");
    }

    /// <summary>
    /// Retrieves a specific assigned todo item for a user.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="todoId">The ID of the todo item to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a TodoModel object, or null if the todo item is not found.</returns>
    public async Task<TodoModel?> GetOneAssigned(int assignedTo, int todoId)
    {
        var results = await _sql.LoadData<TodoModel, dynamic>("dbo.spTodos_GetOneAssigned", new { AssignedTo = assignedTo, TodoId = todoId }, "Default");

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Creates a new todo item and assigns it to a user.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="task">The task description of the todo item.</param>
    /// <param name="isCompleted">A flag indicating whether the todo item is completed or not.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created TodoModel object, or null if the creation fails.</returns>
    public async Task<TodoModel?> Create(int assignedTo, string task, bool isCompleted)
    {
        var results = await _sql.LoadData<TodoModel, dynamic>("dbo.spTodos_Create", new { Task = task, AssignedTo = assignedTo, IsCompleted = isCompleted }, "Default");

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Updates the task description of a specific todo item.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="todoId">The ID of the todo item to update.</param>
    /// <param name="task">The new task description for the todo item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateTask(int assignedTo, int todoId, string task)
    {
        return _sql.SaveData<dynamic>("dbo.spTodos_UpdateTask", new { AssignedTo = assignedTo, TodoId = todoId, Task = task }, "Default");
    }

    /// <summary>
    /// Marks a specific todo item as completed.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="todoId">The ID of the todo item to mark as completed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task CompleteTodo(int assignedTo, int todoId)
    {
        return _sql.SaveData<dynamic>("dbo.spTodos_CompleteTodo", new { AssignedTo = assignedTo, TodoId = todoId }, "Default");
    }

    /// <summary>
    /// Deletes a todo item from the database.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="todoId">The ID of the todo item to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
    public async Task<int> Delete(int assignedTo, int todoId)
    {
        return await _sql.ExecuteDeleteStoredProcedure<dynamic>("dbo.spTodos_Delete", new { AssignedTo = assignedTo, TodoId = todoId }, "Default");
    }
}
