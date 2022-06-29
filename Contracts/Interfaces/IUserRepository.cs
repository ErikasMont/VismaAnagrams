using Contracts.Models;

namespace Contracts.Interfaces;

public interface IUserRepository
{
    Task AddUser(User user);
    Task<IEnumerable<User>> ReadUsers();
    Task<User> ReadUser(string userIp);
    Task ChangeSearchCount(User user);
    Task Commit();
}