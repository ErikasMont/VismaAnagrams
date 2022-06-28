using BusinessLogic.Services;
using Contracts.Interfaces;
using Tests.Mocks;

namespace Tests.Tests;

public class UserServiceTests
{
    private IUserService _userService;
    
    [SetUp]
    public void Setup()
    {
        var userRepository = new MockUserRepo(1);
        _userService = new UserService(userRepository);
    }

    [Test]
    public async Task GetUser_IfNoUserFound_ReturnsNull()
    {
        var ip = "127.0.0.1";

        var result = await _userService.GetUser(ip);

        result.UserIp.ShouldBe(ip);
        result.SearchesLeft.ShouldBe(5);
    }

    [Test]
    public async Task UserExists_IfUserExists_ReturnsTrue()
    {
        var ip = "0:0:0:1";

        var result = await _userService.UserExists(ip);
        
        result.ShouldBeTrue();
    }
    
    [Test]
    public async Task UserExists_Always_ReturnsUserData()
    {
        var ip = "0:0:0:5";

        var result = await _userService.UserExists(ip);
        
        result.ShouldBeFalse();
    }
}