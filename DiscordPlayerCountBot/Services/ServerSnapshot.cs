namespace DiscordPlayerCountBot.Services;

public sealed record ServerSnapshot(
    string Name,
    string Address,
    string Provider,
    string Status,
    int? Players,
    int? MaxPlayers,
    int? QueuedPlayers,
    DateTimeOffset? LastUpdatedUtc,
    double? ServerFps,
    double? ServerFpsAverage,
    double? ServerFrameTime,
    int? Days,
    int? BaseCampCount,
    int? UptimeSeconds)
{
    public PublicServerSnapshot ToPublicSnapshot()
    {
        var metrics = ServerFps.HasValue
            ? new PalworldPublicMetrics(ServerFps.Value, ServerFpsAverage ?? 0, ServerFrameTime ?? 0, Days ?? 0, BaseCampCount ?? 0, UptimeSeconds ?? 0)
            : null;

        return new PublicServerSnapshot(Name, Provider, Status, Players, MaxPlayers, QueuedPlayers, metrics, LastUpdatedUtc);
    }
}

public sealed record PublicServerSnapshot(
    string Name,
    string Provider,
    string Status,
    int? Players,
    int? MaxPlayers,
    int? QueuedPlayers,
    PalworldPublicMetrics? Metrics,
    DateTimeOffset? LastUpdatedUtc);

public sealed record PalworldPublicMetrics(
    double ServerFps,
    double ServerFpsAverage,
    double ServerFrameTime,
    int Days,
    int BaseCampCount,
    int UptimeSeconds);
