using BusinessLogic.DataAccess;
using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cli;
using BusinessLogic.Helpers;
using Contracts.Interfaces;
using EF.DatabaseFirst.DataAccess;
using EF.DatabaseFirst.Models;
using Microsoft.EntityFrameworkCore;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("AnagramsDb");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDbContext<AnagramsDbContext>(option => option.UseSqlServer(connectionString))
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

var anagramCount = config.GetRequiredSection("AnagramCount").Get<int>();
var minInputLength = config.GetRequiredSection("MinInputLength").Get<int>();
var anagramApiClientUrl = config.GetRequiredSection("LocalAnagramApiClientURL").Get<string>();

var ui = new UI(host.Services.GetRequiredService<IWordService>(), anagramCount, minInputLength, anagramApiClientUrl);
await ui.RunAsync();