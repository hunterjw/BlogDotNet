using BlogDotNet.BlazorWebApp;
using BlogDotNet.BlazorWebApp.Components;
using BlogDotNet.BlazorWebApp.Workers;
using BlogDotNet.Database.Migrations;
using BlogDotNet.DataServices.Extensions;
using BlogDotNet.DataServices.Models;
using BlogDotNet.Database.Migrations.Extensions;
using BlogDotNet.Database;
using BlogDotNet.Database.Extensions;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

AddDataServices(builder.Services, config);
AddMigrations(builder.Services, config);
AddDatabase(builder.Services, config);

builder.Services.AddSingleton(
    config
        .GetRequiredSection("BlogDotNet")
        .GetRequiredSection("Blog")
        .Get<BlogOptions>()
        ?? new BlogOptions());

builder.Services.AddScoped<TimeProvider, BrowserTimeProvider>();

builder.Services.AddHostedService<StartupWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void AddDataServices(IServiceCollection services, IConfiguration config)
{
    services.AddSingleton(
        config
            .GetRequiredSection("BlogDotNet")
            .GetRequiredSection("FileScanner")
            .Get<FileScannerServiceOptions>()
            ?? new FileScannerServiceOptions());
    services.AddBlogDotNetServices();
}

static void AddMigrations(IServiceCollection services, IConfiguration config)
{
    services.AddSingleton(
        config
            .GetRequiredSection("BlogDotNet")
            .GetRequiredSection("Migrations")
            .Get<MigrationServiceOptions>()
            ?? new MigrationServiceOptions());
    services
        .AddBlogDotNetMigrations(config.GetConnectionString("BlogDotNet"));
}

static void AddDatabase(IServiceCollection services, IConfiguration config)
{
    services.AddSingleton(new DatabaseOptions
    {
        ConnectionString = config.GetConnectionString("BlogDotNet") ?? string.Empty,
    });
    services.AddBlogDotNetDatabase();
}