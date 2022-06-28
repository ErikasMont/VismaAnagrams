using Contracts.Models;

namespace Contracts.Interfaces;

public interface IUserRepository
{
    Task AddUser(User user);
    Task<IEnumerable<User>> ReadUsers();
    Task<User> ReadUser(string userIp);
    Task IncreaseSearchCount(string userIp);
    Task ReduceSearchCount(string userIp);
    Task Commit();
}