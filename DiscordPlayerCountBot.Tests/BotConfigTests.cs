using DiscordPlayerCountBot.Bot;

namespace DiscordPlayerCountBot.Tests;

public class BotConfigTests
{
    [Fact]
    public void CreateDefaultsIncludesAnEmptyNotificationChannelId()
    {
        var config = new BotConfig();

        config.CreateDefaults();

        Assert.True(config.ApplicationTokens.TryGetValue("DISCORD_CHANNEL_ID", out var channelId));
        Assert.Equal(string.Empty, channelId);
    }
}
