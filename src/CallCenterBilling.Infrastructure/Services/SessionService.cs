using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CallCenterBilling.Infrastructure.Services;
public class SessionService : ISessionService
{
    private readonly IAgentSessionRepository _sessionRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IRealTimeNotificationService _notificationService;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        IAgentSessionRepository sessionRepository,
        IAgentRepository agentRepository,
        IRealTimeNotificationService notificationService,
        ILogger<SessionService> logger)
    {
        _sessionRepository = sessionRepository;
        _agentRepository = agentRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<int> StartSessionAsync(int agentId, string connectionId)
    {
        // End any existing active session for this agent
        var existingSession = await _sessionRepository.GetActiveSessionByAgentIdAsync(agentId);
        if (existingSession != null)
        {
            existingSession.Logout();
            await _sessionRepository.UpdateAsync(existingSession);
        }

        // Create new session
        var session = new AgentSession
        {
            AgentId = agentId,
            ConnectionId = connectionId,
            LoginTime = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow
        };

        var sessionId = await _sessionRepository.AddAsync(session);

        // Update agent status to active
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent != null)
        {
            agent.Activate();
            await _agentRepository.UpdateAsync(agent);
        }

        // Notify that agent is connected
        await _notificationService.NotifyAgentConnectedAsync(agentId);

        _logger.LogInformation("Started session {SessionId} for agent {AgentId}", sessionId, agentId);
        return sessionId;
    }

    public async Task EndSessionAsync(string connectionId)
    {
        var session = await _sessionRepository.GetByConnectionIdAsync(connectionId);
        if (session == null) return;

        session.Logout();
        await _sessionRepository.UpdateAsync(session);

        // Update agent status to offline
        var agent = await _agentRepository.GetByIdAsync(session.AgentId);
        if (agent != null)
        {
            agent.Deactivate();
            await _agentRepository.UpdateAsync(agent);
        }

        // Notify that agent is disconnected
        await _notificationService.NotifyAgentDisconnectedAsync(session.AgentId);

        _logger.LogInformation("Ended session for agent {AgentId}", session.AgentId);
    }

    public async Task UpdateHeartbeatAsync(string connectionId)
    {
        var session = await _sessionRepository.GetByConnectionIdAsync(connectionId);
        if (session == null) return;

        session.UpdateHeartbeat();
        await _sessionRepository.UpdateAsync(session);
    }

    public async Task CleanupExpiredSessionsAsync()
    {
        var timeout = TimeSpan.FromMinutes(5); // Consider sessions expired after 5 minutes of inactivity
        await _sessionRepository.EndExpiredSessionsAsync(timeout);

        // Update agent statuses for expired sessions
        var expiredSessions = await _sessionRepository.GetActiveSessionsAsync();
        foreach (var session in expiredSessions.Where(s => s.IsExpired(timeout)))
        {
            var agent = await _agentRepository.GetByIdAsync(session.AgentId);
            if (agent != null)
            {
                agent.Deactivate();
                await _agentRepository.UpdateAsync(agent);
                await _notificationService.NotifyAgentDisconnectedAsync(agent.Id);
            }
        }
    }

    public async Task<bool> IsAgentOnlineAsync(int agentId)
    {
        var session = await _sessionRepository.GetActiveSessionByAgentIdAsync(agentId);
        return session != null && !session.IsExpired(TimeSpan.FromMinutes(5));
    }
}

