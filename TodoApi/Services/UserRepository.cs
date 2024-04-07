using Dapper;
using System.Data;
using System.Data.SqlClient;
using TodoApi.Models;

namespace TodoApi.Services;


/// <summary>
/// Represents a repository for managing user data.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="configuration">The configuration object.</param>
    public UserRepository(IConfiguration configuration)
    {
        _db = new SqlConnection(configuration.GetConnectionString("Default"));
    }

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user.</returns>
    public User AddUser(User user)
    {
        var procedureName = "CreateUser";
        var parameters = new DynamicParameters();
        parameters.Add("@Username", user.Username);
        parameters.Add("@PasswordHash", user.PasswordHash);
        parameters.Add("@PasswordSalt", user.PasswordSalt);

        var id = _db.Query<int>(procedureName, parameters, commandType: CommandType.StoredProcedure).Single();
        user.Id = id;
        return user;
    }

    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    public void DeleteUser(int userId)
    {
        var procedureName = "DeleteUser";
        var parameters = new DynamicParameters();
        parameters.Add("@Id", userId);

        _db.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Gets a user from the repository by username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The retrieved user.</returns>
    public User GetUserByUsername(string username)
    {
        var procedureName = "GetUserByUsername";
        var parameters = new DynamicParameters();
        parameters.Add("@Username", username);

        return _db.Query<User>(procedureName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault()!;
    }

    /// <summary>
    /// Gets a user from the repository by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The retrieved user.</returns>
    public User GetUserById(int id)
    {
        var procedureName = "GetUserById";
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        return _db.Query<User>(procedureName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault()!;
    }
}
