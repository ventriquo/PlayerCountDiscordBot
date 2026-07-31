namespace DiscordPlayerCountBot.Services;

public enum HealthStatus
{
    Starting,
    NoBotsConfigured,
    DiscordDisconnected,
    UpdateStale,
    Healthy
}

public sealed record HealthSnapshot(HealthStatus Status)
{
    public bool IsHealthy => Status == HealthStatus.Healthy;
}

public sealed class BotHealthMonitor
{
    private const int MinimumStalenessSeconds = 90;

    private int _isConfigured;
    private int _updateIntervalSeconds = 30;
    private long _lastUpdateCompletedUtcTicks;

    public void MarkConfigured(int updateIntervalSeconds, DateTimeOffset now)
    {
        Volatile.Write(ref _updateIntervalSeconds, Math.Max(1, updateIntervalSeconds));
        MarkUpdateCompleted(now);
        Volatile.Write(ref _isConfigured, 1);
    }

    public void MarkUpdateCompleted(DateTimeOffset now)
    {
        Interlocked.Exchange(ref _lastUpdateCompletedUtcTicks, now.UtcDateTime.Ticks);
    }

    public HealthSnapshot GetSnapshot(bool hasBots, bool isDiscordConnected, DateTimeOffset now)
    {
        if (Volatile.Read(ref _isConfigured) == 0)
            return new(HealthStatus.Starting);

        if (!hasBots)
            return new(HealthStatus.NoBotsConfigured);

        if (!isDiscordConnected)
            return new(HealthStatus.DiscordDisconnected);

        var lastUpdateTicks = Interlocked.Read(ref _lastUpdateCompletedUtcTicks);
        var lastUpdate = new DateTimeOffset(lastUpdateTicks, TimeSpan.Zero);
        var updateAge = now - lastUpdate;
        var staleAfter = TimeSpan.FromSeconds(Math.Max(MinimumStalenessSeconds, Volatile.Read(ref _updateIntervalSeconds) * 2 + 10));

        return updateAge > staleAfter
            ? new(HealthStatus.UpdateStale)
            : new(HealthStatus.Healthy);
    }
}
