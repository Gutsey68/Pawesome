using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Pawesome.Infrastructure.Extensions;
using Pawesome.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

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
    .AddStripeServices(builder.Configuration);

builder.Services.AddSignalR();

// Configure localization
var cultureInfo = new CultureInfo("fr-FR");
var supportedCultures = new[] { cultureInfo };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("fr-FR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigurePipeline();

// Initialize the database
await app.InitializeDatabaseAsync();

app.Run();