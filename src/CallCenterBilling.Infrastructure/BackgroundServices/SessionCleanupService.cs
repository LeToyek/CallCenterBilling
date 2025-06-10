using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CallCenterBilling.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace CallCenterBilling.Infrastructure.BackgroundServices;

public class SessionCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Run every minute

    public SessionCleanupService(IServiceProvider serviceProvider, ILogger<SessionCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

                await sessionService.CleanupExpiredSessionsAsync();
                _logger.LogDebug("Session cleanup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during session cleanup");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
