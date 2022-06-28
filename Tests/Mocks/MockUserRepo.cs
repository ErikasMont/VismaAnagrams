using Contracts.Interfaces;
using Contracts.Models;

namespace Tests.Mocks;

public class MockUserRepo : IUserRepository
{
    private readonly int _setNumber;

    public MockUserRepo(int setNumber)
    {
        _setNumber = setNumber;
    }
    
    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddUser(User user)
    {
        
    }

    public async Task<IEnumerable<User>> ReadUsers()
    {
        return new List<User>()
        {
            new("0:0:0:1", 5), new("0:0:0:2", 0), new("0:0:0:3", 2)
        };
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="userIp"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<User> ReadUser(string userIp)
    {
        return _setNumber switch
        {
            1 => new User("127.0.0.1", 5),
            2 => new User("127.0.0.1", 0)
        };
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="userIp"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task IncreaseSearchCount(string userIp)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <param name="userIp"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ReduceSearchCount(string userIp)
    {
        
    }

    /// <summary>
    /// Method required for interface realization
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Commit()
    {
        
    }
}