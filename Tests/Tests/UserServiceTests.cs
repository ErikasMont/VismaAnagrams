using BusinessLogic.Services;
using Contracts.Interfaces;
using Contracts.Models;

namespace Tests.Tests;

public class UserServiceTests
{
    private IUserService _userService;
    private IUserRepository _userRepository;
    
    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userService = new UserService(_userRepository);
    }

    [Test]
    public async Task GetUser_Always_ReturnsUser()
    {
        var ip = "127.0.0.1";
        var searchesLeft = 5;
        _userRepository.ReadUser(ip).Returns(new User(ip, searchesLeft));

        var result = await _userService.GetUser(ip);

        result.UserIp.ShouldBe(ip);
        result.SearchesLeft.ShouldBe(searchesLeft);
        _userRepository.Received().ReadUser(ip);
    }

    [Test]
    public async Task UserExists_IfUserExists_ReturnsTrue()
    {
        var ip = "0:0:0:1";
        _userRepository.ReadUsers().Returns(new List<User>()
        {
            new ("0:0:0:1", 5), new ("0:0:0:2", 3)
        });

        var result = await _userService.UserExists(ip);
        
        result.ShouldBeTrue();
        _userRepository.Received().ReadUsers();
    }
    
    [Test]
    public async Task UserExists_Always_ReturnsUserData()
    {
        var ip = "0:0:0:5";
        _userRepository.ReadUsers().Returns(new List<User>()
        {
            new ("0:0:0:1", 5), new ("0:0:0:2", 3)
        });

        var result = await _userService.UserExists(ip);
        
        result.ShouldBeFalse();
        _userRepository.Received().ReadUsers();
    }
}