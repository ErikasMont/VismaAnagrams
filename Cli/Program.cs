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
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("AnagramsCodeFirstDb");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDbContext<AnagramsCodeFirstDbContext>(option => option.UseSqlServer(connectionString))
            .AddSingleton<IAnagramSolver, AnagramSolver>()
            .AddSingleton<IWordService, WordService>()
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
var anagramCount = config.GetRequiredSection("AnagramCount").Get<int>();
var minInputLength = config.GetRequiredSection("MinInputLength").Get<int>();
var anagramApiClientUrl = config.GetRequiredSection("LocalAnagramApiClientURL").Get<string>();

/*var ui = new UI(WriteToConsole, CapitalizeFirstLetter, host.Services.GetRequiredService<IWordService>(), anagramCount, 
    minInputLength, anagramApiClientUrl);
await ui.RunAsync();*/

var uiWithEvents = new UIWithEvents(CapitalizeFirstLetter, host.Services.GetRequiredService<IWordService>(),
    anagramCount, minInputLength, anagramApiClientUrl);
uiWithEvents.PrintEvent += WriteToConsole;
uiWithEvents.PrintEvent += WriteToTxt;
await uiWithEvents.RunAsync();

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
