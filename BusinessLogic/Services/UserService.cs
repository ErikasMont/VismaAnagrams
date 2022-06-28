using Contracts.Interfaces;
using Contracts.Models;

namespace BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task AddUser(string userIp)
    {
        var user = new User(userIp, 5);
        await _userRepository.AddUser(user);
        await _userRepository.Commit();
    }

    public async Task<User> GetUser(string userIp)
    {
        return await _userRepository.ReadUser(userIp);
    }

    public async Task<bool> UserExists(string userIp)
    {
        var users = await _userRepository.ReadUsers();
        return users.Any(x => x.UserIp == userIp);
    }

    public async Task UpdateSearchCount(string option, string userIp)
    {
        switch (option)
        {
            case "reduce":
                await _userRepository.ReduceSearchCount(userIp);
                await _userRepository.Commit();
                break;
            case "increase":
                await _userRepository.IncreaseSearchCount(userIp);
                await _userRepository.Commit();
                break;
        }
    }
}