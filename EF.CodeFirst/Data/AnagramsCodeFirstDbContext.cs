using EF.CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.Data;

public class AnagramsCodeFirstDbContext : DbContext
{
    public AnagramsCodeFirstDbContext()
    {
    }
    
    public AnagramsCodeFirstDbContext(DbContextOptions<AnagramsCodeFirstDbContext> options) : base(options)
    {
    }

    public DbSet<CachedWord> CachedWords { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<User> Users { get; set; }
}