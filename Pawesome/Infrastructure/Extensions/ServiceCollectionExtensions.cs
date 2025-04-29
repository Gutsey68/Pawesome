using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Pawesome.Infrastructure.Filters;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Repositories;
using Pawesome.Services;

namespace Pawesome.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the database context for the application
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPawesomeDatabase(this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        return services;
    }
    
    /// <summary>
    /// Adds and configures identity services for authentication and authorization
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPawesomeIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
        });
        
        return services;
    }
    
    /// <summary>
    /// Registers validators for data validation using FluentValidation and configures automatic validation filter
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPawesomeValidation(this IServiceCollection services)
    {
        // Register all validators from the assembly automatically
        services.AddValidatorsFromAssemblyContaining<Program>();
    
        // Register components needed for automatic validation in controllers
        services.AddScoped<FluentValidationFilter>();
        services.AddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();

        return services;
    }
    
    /// <summary>
    /// Registers application services, repositories and AutoMapper
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPawesomeServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IAnimalTypeRepository, AnimalTypeRepository>();
        services.AddScoped<IAdvertRepository, AdvertRepository>();
        
        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAdvertService, AdvertService>();
        services.AddScoped<IAnimalTypeService, AnimalTypeService>();
        
        // Register AutoMapper
        services.AddAutoMapper(typeof(Program).Assembly);
        
        return services;
    }
}