using BusinessLogic.DataAccess;
using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cli;
using Contracts.Interfaces;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton<IAnagramSolver, AnagramSolver>()
            .AddSingleton<IWordRepository, WordDataAccess>())
    .Build();

var anagramCount = config.GetRequiredSection("AnagramCount").Get<int>();
var minInputLength = config.GetRequiredSection("MinInputLength").Get<int>();

var ui = new UI(host.Services.GetRequiredService<IAnagramSolver>(), anagramCount, minInputLength);
ui.Run();
