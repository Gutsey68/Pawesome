using Pawesome.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure services using extension methods
builder.Services.AddPawesomeDatabase(builder.Configuration)
    .AddPawesomeIdentity()
    .AddPawesomeValidation()
    .AddPawesomeServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigurePipeline();

// Initialize the database
await app.InitializeDatabaseAsync();

app.Run();