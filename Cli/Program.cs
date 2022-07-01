using System.Diagnostics;
using System.Text.RegularExpressions;
using BusinessLogic.DataAccess;
using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cli;
using BusinessLogic.Helpers;
using Contracts.Interfaces;
using EF.CodeFirst.DataAccess;
using EF.CodeFirst.Data;
using Microsoft.EntityFrameworkCore;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("AnagramsCodeFirstDb");
var anagramApiClientUrl = config.GetRequiredSection("LocalAnagramApiClientURL").Get<string>();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDbContext<AnagramsCodeFirstDbContext>(option => option.UseSqlServer(connectionString))
            .AddSingleton<IAnagramSolver, AnagramSolver>()
            .AddSingleton<IWordService, WordService>()
            .AddSingleton<IDisplay, UIWithEvents>(provider =>
            {
                var ui = new UIWithEvents(CapitalizeFirstLetter, provider.GetRequiredService<IWordService>(),
                    anagramApiClientUrl);
                ui.PrintEvent += WriteToConsole;
                ui.PrintEvent += WriteToTxt;
                return ui;
            })
            .AddSingleton<WordFileAccess>()
            .AddSingleton<WordEfDbAccess>()
            .AddSingleton<ServiceResolver>(serviceProvider => key =>
            {
                return key switch
                {
                    RepositoryType.File => serviceProvider.GetService<WordFileAccess>(),
                    RepositoryType.Db => serviceProvider.GetService<WordEfDbAccess>()
                };
            }))
    .Build();

var dbContext = host.Services.GetRequiredService<AnagramsCodeFirstDbContext>();
dbContext.Database.EnsureCreated();

var ui = host.Services.GetRequiredService<IDisplay>();
await ui.RunAsync();

void WriteToConsole(object? sender, string message)
{
    Console.WriteLine(message);
}

void WriteToDebug(object? sender, string message)
{
    Debug.WriteLine(message);
}

void WriteToTxt(object? sender, string message)
{
    using var fs = File.Open("delegates.txt", FileMode.Append, FileAccess.Write);
    using var sw = new StreamWriter(fs);
    sw.WriteLine(message);
}

string CapitalizeFirstLetter(string input)
{
    return Regex.Replace(input, "^[a-z]", m => m.Value.ToUpper());
}
