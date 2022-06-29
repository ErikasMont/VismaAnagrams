using Contracts.Interfaces;
using EF.CodeFirst.Data;
using EF.CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.DataAccess;

public class UserEfDbAccess : IUserRepository
{
    private readonly AnagramsCodeFirstDbContext _context;

    public UserEfDbAccess(AnagramsCodeFirstDbContext context)
    {
        _context = context;
    }
    
    public async Task AddUser(Contracts.Models.User user)
    {
        await _context.AddAsync(new User() { UserIp = user.UserIp, SearchesLeft = user.SearchesLeft });
    }

    public async Task<IEnumerable<Contracts.Models.User>> ReadUsers()
    {
        return await _context.Users.Select(x => new Contracts.Models.User(x.UserIp, x.SearchesLeft)).ToListAsync();
    }

    public async Task<Contracts.Models.User> ReadUser(string userIp)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIp == userIp);
        return new Contracts.Models.User(user.UserIp, user.SearchesLeft);
    }

    public async Task ChangeSearchCount(Contracts.Models.User user)
    {
        var foundUser = await _context.Users.FirstOrDefaultAsync(x => x.UserIp == user.UserIp);
        foundUser.SearchesLeft = user.SearchesLeft;
        _context.Users.Update(foundUser);
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}