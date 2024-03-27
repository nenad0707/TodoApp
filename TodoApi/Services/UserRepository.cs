using Dapper;
using System.Data;
using System.Data.SqlClient;
using TodoApi.Models;

namespace TodoApi.Services;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IConfiguration configuration)
    {
        _db = new SqlConnection(configuration.GetConnectionString("Default"));
    }

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

    public void DeleteUser(int userId)
    {
        var procedureName = "DeleteUser";
        var parameters = new DynamicParameters();
        parameters.Add("@Id", userId);

        _db.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    public User GetUserByUsername(string username)
    {
        var procedureName = "GetUserByUsername";
        var parameters = new DynamicParameters();
        parameters.Add("@Username", username);

        return _db.Query<User>(procedureName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault()!;
    }

    public User GetUserById(int id)
    {
        var procedureName = "GetUserById";
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        return _db.Query<User>(procedureName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault()!;
    }

}
