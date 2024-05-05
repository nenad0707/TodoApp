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
    /// Retrieves a paginated list of all todo items assigned to a specific user.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo items are assigned.</param>
    /// <param name="pageNumber">The page number of the results to fetch.</param>
    /// <param name="pageSize">The number of todo items to return per page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="TodoModel"/> objects representing the assigned todo items. If no items are found, the list will be empty.</returns>
    public Task<List<TodoModel>> GetAllAssigned(int assignedTo, int pageNumber, int pageSize)
    {
        return _sql.LoadData<TodoModel, dynamic>(
            "dbo.spTodos_GetAllAssigned",
            new { AssignedTo = assignedTo, PageNumber = pageNumber, PageSize = pageSize },
            "Default");
    }

    /// <summary>
    /// Retrieves the total count of todo items assigned to a specific user.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo items are assigned.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total number of todo items assigned to the specified user. If no items are found, returns 0.</returns>
    public async Task<int> GetTotalCount(int assignedTo)
    {
        var result = await _sql.LoadData<int, dynamic>(
            "dbo.spTodos_GetTotalCount",
            new { AssignedTo = assignedTo },
            "Default");

        return result.FirstOrDefault();
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
    public async Task<int> UpdateTask(int assignedTo, int todoId, string task)
    {

        return await _sql.SaveData("dbo.spTodos_UpdateTask", new { AssignedTo = assignedTo, TodoId = todoId, Task = task }, "Default");
    }

    /// <summary>
    /// Marks a specific todo item as completed.
    /// </summary>
    /// <param name="assignedTo">The ID of the user to whom the todo item is assigned.</param>
    /// <param name="todoId">The ID of the todo item to mark as completed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<int> CompleteTodo(int assignedTo, int todoId)
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
        return await _sql.SaveData("dbo.spTodos_Delete", new { AssignedTo = assignedTo, TodoId = todoId }, "Default");
    }
}
