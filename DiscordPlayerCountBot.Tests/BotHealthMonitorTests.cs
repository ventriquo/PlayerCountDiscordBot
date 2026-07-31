using DiscordPlayerCountBot.Services;

namespace DiscordPlayerCountBot.Tests;

public class BotHealthMonitorTests
{
    private static readonly DateTimeOffset Now = new(2026, 7, 31, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ReportsStartingUntilTheBotConfigurationLoads()
    {
        var monitor = new BotHealthMonitor();

        Assert.Equal(HealthStatus.Starting, monitor.GetSnapshot(true, true, Now).Status);
    }

    [Fact]
    public void ReportsNoBotsWhenConfigurationDoesNotContainAnyBots()
    {
        var monitor = new BotHealthMonitor();
        monitor.MarkConfigured(30, Now);

        Assert.Equal(HealthStatus.NoBotsConfigured, monitor.GetSnapshot(false, true, Now).Status);
    }

    [Fact]
    public void ReportsDiscordDisconnectedWhenAnyClientIsNotConnected()
    {
        var monitor = new BotHealthMonitor();
        monitor.MarkConfigured(30, Now);

        Assert.Equal(HealthStatus.DiscordDisconnected, monitor.GetSnapshot(true, false, Now).Status);
    }

    [Fact]
    public void ReportsStaleWhenUpdatesStopForLongerThanTheAllowedWindow()
    {
        var monitor = new BotHealthMonitor();
        monitor.MarkConfigured(30, Now);

        Assert.Equal(HealthStatus.UpdateStale, monitor.GetSnapshot(true, true, Now.AddSeconds(91)).Status);
    }

    [Fact]
    public void ReportsHealthyWhenDiscordIsConnectedAndUpdatesAreFresh()
    {
        var monitor = new BotHealthMonitor();
        monitor.MarkConfigured(30, Now);
        monitor.MarkUpdateCompleted(Now.AddSeconds(30));

        var health = monitor.GetSnapshot(true, true, Now.AddSeconds(60));

        Assert.True(health.IsHealthy);
        Assert.Equal(HealthStatus.Healthy, health.Status);
    }
}
