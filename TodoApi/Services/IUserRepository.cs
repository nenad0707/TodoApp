using TodoApi.Models;

namespace TodoApi.Services;


/// <summary>
/// Represents a repository for managing user data.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user.</returns>
    User AddUser(User user);

    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    void DeleteUser(int userId);

    /// <summary>
    /// Retrieves a user from the repository by username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The retrieved user.</returns>
    User GetUserByUsername(string username);

    /// <summary>
    /// Retrieves a user from the repository by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The retrieved user.</returns>
    User GetUserById(int id);
}
