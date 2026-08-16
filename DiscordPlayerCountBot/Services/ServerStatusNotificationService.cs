namespace DiscordPlayerCountBot.Services;

public enum ServerStatusChange
{
    Offline,
    Online
}

public enum ServerAvailability
{
    Unknown,
    Online,
    Offline
}

public sealed class ServerStatusMonitor
{
    public const int FailedPollThreshold = 3;

    private bool _hasObservedOnline;
    private bool _isOffline;
    private int _consecutiveFailedPolls;
    private int _currentStatus = (int)ServerAvailability.Unknown;

    public ServerAvailability CurrentStatus => (ServerAvailability)Volatile.Read(ref _currentStatus);

    public ServerStatusChange? RecordSuccessfulPoll()
    {
        _consecutiveFailedPolls = 0;

        if (!_hasObservedOnline)
        {
            _hasObservedOnline = true;
            Volatile.Write(ref _currentStatus, (int)ServerAvailability.Online);
            return null;
        }

        if (!_isOffline)
            return null;

        _isOffline = false;
        Volatile.Write(ref _currentStatus, (int)ServerAvailability.Online);
        return ServerStatusChange.Online;
    }

    public ServerStatusChange? RecordFailedPoll()
    {
        if (!_hasObservedOnline || _isOffline)
            return null;

        _consecutiveFailedPolls++;
        if (_consecutiveFailedPolls < FailedPollThreshold)
            return null;

        _isOffline = true;
        Volatile.Write(ref _currentStatus, (int)ServerAvailability.Offline);
        return ServerStatusChange.Offline;
    }
}

public sealed class ServerStatusNotificationService : LoggableClass
{
    private readonly DiscordSocketClient _discordClient;
    private readonly ulong? _channelId;
    private readonly ServerStatusMonitor _statusMonitor = new();
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    public ServerStatusNotificationService(DiscordSocketClient discordClient, ulong? channelId)
    {
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        _channelId = channelId;
    }

    public ServerAvailability CurrentStatus => _statusMonitor.CurrentStatus;

    public async Task RecordSuccessfulPollAsync()
    {
        await RecordPollAsync(_statusMonitor.RecordSuccessfulPoll);
    }

    public async Task RecordFailedPollAsync()
    {
        await RecordPollAsync(_statusMonitor.RecordFailedPoll);
    }

    private async Task RecordPollAsync(Func<ServerStatusChange?> recordPoll)
    {
        await _stateLock.WaitAsync();
        try
        {
            var statusChange = recordPoll();
            if (statusChange == null || _channelId == null)
                return;

            var message = statusChange == ServerStatusChange.Offline
                ? "🚨 The Project Zomboid server is currently OFFLINE!"
                : "✅ The Project Zomboid server is back ONLINE!";

            try
            {
                var channel = await _discordClient.GetChannelAsync(_channelId.Value) as IMessageChannel;
                if (channel == null)
                {
                    Warn($"Could not send server status notification: Discord channel {_channelId.Value} was not found or is not a message channel.");
                    return;
                }

                await channel.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                Error($"Could not send server status notification to Discord channel {_channelId.Value}.", null, exception);
            }
        }
        finally
        {
            _stateLock.Release();
        }
    }
}
