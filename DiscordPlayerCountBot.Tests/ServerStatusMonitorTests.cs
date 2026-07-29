namespace DiscordPlayerCountBot.Tests;

public class ServerStatusMonitorTests
{
    [Fact]
    public void DoesNotReportOfflineUntilThreeConsecutivePollsFail()
    {
        var monitor = new DiscordPlayerCountBot.Services.ServerStatusMonitor();

        Assert.Null(monitor.RecordSuccessfulPoll());
        Assert.Null(monitor.RecordFailedPoll());
        Assert.Null(monitor.RecordFailedPoll());
        Assert.Equal(DiscordPlayerCountBot.Services.ServerStatusChange.Offline, monitor.RecordFailedPoll());
        Assert.Null(monitor.RecordFailedPoll());
    }

    [Fact]
    public void ReportsOnlineOnceAfterAConfirmedOfflineState()
    {
        var monitor = new DiscordPlayerCountBot.Services.ServerStatusMonitor();

        monitor.RecordSuccessfulPoll();
        monitor.RecordFailedPoll();
        monitor.RecordFailedPoll();
        Assert.Equal(DiscordPlayerCountBot.Services.ServerStatusChange.Offline, monitor.RecordFailedPoll());

        Assert.Equal(DiscordPlayerCountBot.Services.ServerStatusChange.Online, monitor.RecordSuccessfulPoll());
        Assert.Null(monitor.RecordSuccessfulPoll());
    }

    [Fact]
    public void DoesNotReportOfflineWhenTheFirstPollsFail()
    {
        var monitor = new DiscordPlayerCountBot.Services.ServerStatusMonitor();

        monitor.RecordFailedPoll();
        monitor.RecordFailedPoll();
        Assert.Null(monitor.RecordFailedPoll());
        Assert.Null(monitor.RecordSuccessfulPoll());
    }

    [Fact]
    public void SuccessfulPollResetsTheFailureCounter()
    {
        var monitor = new DiscordPlayerCountBot.Services.ServerStatusMonitor();

        monitor.RecordSuccessfulPoll();
        monitor.RecordFailedPoll();
        monitor.RecordFailedPoll();
        monitor.RecordSuccessfulPoll();
        monitor.RecordFailedPoll();
        Assert.Null(monitor.RecordFailedPoll());
    }
}
