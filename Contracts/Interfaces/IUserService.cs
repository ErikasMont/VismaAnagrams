using Contracts.Models;

namespace Contracts.Interfaces;

public interface IUserService
{
    Task AddUser(string userIp);
    Task<User> GetUser(string userIp);
    Task<bool> UserExists(string userIp);
    Task UpdateSearchCount(string option, string userIp);
}