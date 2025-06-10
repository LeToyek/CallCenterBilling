using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Application.Services;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Infrastructure.Data;
using CallCenterBilling.Infrastructure.Repositories;
using CallCenterBilling.Infrastructure.BackgroundServices;
using CallCenterBilling.Infrastructure.Services;

namespace CallCenterBilling.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>((options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.ConfigureWarnings(warnings => { warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning); });
        });

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure cookie settings
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(24);
            options.SlidingExpiration = true;
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<ICallRepository, CallRepository>();
        services.AddScoped<IAgentSessionRepository, AgentSessionRepository>();


        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<ICallService, CallService>();
        services.AddScoped<IAgentStatusService, AgentStatusService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IRealTimeNotificationService, RealTimeNotificationService>();

        // Background Services
        services.AddHostedService<SessionCleanupService>();
        services.AddHostedService<CallCenterSimulationService>();

        return services;
    }
}