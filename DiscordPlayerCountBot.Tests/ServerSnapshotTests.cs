using DiscordPlayerCountBot.Services;

namespace DiscordPlayerCountBot.Tests;

public sealed class ServerSnapshotTests
{
    [Fact]
    public void PublicSnapshotDoesNotExposePrivateServerAddress()
    {
        var snapshot = new ServerSnapshot(
            "Palworld Polaris",
            "10.0.1.1",
            "PALWORLDREST",
            "online",
            2,
            8,
            0,
            DateTimeOffset.Parse("2026-08-16T09:00:00Z"),
            59,
            59.4,
            16.83,
            13,
            2,
            1200);

        var publicSnapshot = snapshot.ToPublicSnapshot();
        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(publicSnapshot);

        Assert.Equal("Palworld Polaris", publicSnapshot.Name);
        Assert.Equal(2, publicSnapshot.Players);
        Assert.Equal(59, publicSnapshot.Metrics?.ServerFps);
        Assert.DoesNotContain("10.0.1.1", serialized);
        Assert.DoesNotContain("Address", serialized);
    }
}
