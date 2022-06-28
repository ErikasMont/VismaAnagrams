using BusinessLogic.DataAccess;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using EF.CodeFirst.Data;
using EF.CodeFirst.DataAccess;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("AnagramsCodeFirstDb");
builder.Services.AddDbContext<AnagramsCodeFirstDbContext>(option => 
        option.UseSqlServer(connectionString))
    .AddScoped<IAnagramSolver, AnagramSolver>()
    .AddScoped<IWordService, WordService>()
    .AddScoped<WordEfDbAccess>()
    .AddScoped<WordFileAccess>()
    .AddScoped<ServiceResolver>(serviceProvider => key =>
    {
        return key switch
        {
            RepositoryType.File => serviceProvider.GetService<WordFileAccess>(),
            RepositoryType.Db => serviceProvider.GetService<WordEfDbAccess>()
        };
    });

builder.Services.Configure<WordSettings>(builder.Configuration.GetSection("WordSettings"));

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>()
           .CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<AnagramsCodeFirstDbContext>();

    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();