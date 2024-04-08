using TodoLibrary.Models;

namespace TodoLibrary
{
    public interface ITodoData
    {
        Task<int> CompleteTodo(int assignedTo, int todoId);
        Task<TodoModel?> Create(int assignedTo, string task, bool isCompleted);
        Task<int> Delete(int assignedTo, int todoId);
        Task<List<TodoModel>> GetAllAssigned(int assignedTo);
        Task<TodoModel?> GetOneAssigned(int assignedTo, int todoId);
        Task<int> UpdateTask(int assignedTo, int todoId, string task);
    }
}