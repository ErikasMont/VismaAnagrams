using BusinessLogic.DataAccess;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Contracts.Interfaces;
using WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAnagramSolver, AnagramSolver>()
    .AddScoped<IWordService, WordService>()
    .AddScoped<WordDbAccess>()
    .AddScoped<WordFileAccess>()
    .AddScoped<ServiceResolver>(serviceProvider => key =>
    {
        return key switch
        {
            "File" => serviceProvider.GetService<WordFileAccess>(),
            "Db" => serviceProvider.GetService<WordDbAccess>()
        };
    });

builder.Services.Configure<WordSettings>(builder.Configuration.GetSection("WordSettings"));

var app = builder.Build();

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