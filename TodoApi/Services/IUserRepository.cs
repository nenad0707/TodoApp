using TodoApi.Models;

namespace TodoApi.Services;

public interface IUserRepository
{
    User AddUser(User user);
    void DeleteUser(int userId);
    User GetUserByUsername(string username);
    User GetUserById(int id);
}
