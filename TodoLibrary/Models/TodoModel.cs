namespace TodoLibrary.Models;


/// <summary>
/// Represents a todo item.
/// </summary>
public class TodoModel
{
    /// <summary>
    /// Gets or sets the ID of the todo item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the task description of the todo item.
    /// </summary>
    public string? Task { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user assigned to the todo item.
    /// </summary>
    public int AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the todo item is completed.
    /// </summary>
    public bool IsCompleted { get; set; }
}
