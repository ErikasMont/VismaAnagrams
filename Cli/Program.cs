using BusinessLogic.Repositories;
using BusinessLogic.Services;
using Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cli.Settings;
using Contracts.Interfaces;

string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(path)
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddScoped<IAnagramSolver, AnagramSolver>()
            .AddScoped<IWordRepository, WordRepository>())
    .Build();

Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

var ui = new Cli.UI(host.Services.GetService<IAnagramSolver>(), settings.AnagramCount, settings.MinInputLength);
ui.Run();
