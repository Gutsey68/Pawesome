using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Pawesome.Infrastructure.Extensions;
using Pawesome.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configure services and settings from appsettings.json and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Configure forwarded headers for reverse proxy scenarios
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<FluentValidationFilter>();
});

// Configure services using extension methods
builder.Services.AddPawesomeDatabase(builder.Configuration)
    .AddPawesomeIdentity()
    .AddPawesomeValidation()
    .AddPawesomeServices()
    .AddStripeServices(builder.Configuration)
    .AddSignalR();


var app = builder.Build();

// Configure localization
var supportedCultures = new[] { new CultureInfo("fr-FR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fr-FR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline
app.ConfigurePipeline();

app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");

// Initialize the database
await app.InitializeDatabaseAsync();

app.Run();