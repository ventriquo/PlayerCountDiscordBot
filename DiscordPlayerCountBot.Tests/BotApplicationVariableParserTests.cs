using DiscordPlayerCountBot.EnvironmentParser;

namespace DiscordPlayerCountBot.Tests;

[Collection("Configuration Test Suite")]
public class BotApplicationVariableParserTests
{
    [Fact]
    public void AddsDiscordChannelIdFromDedicatedEnvironmentVariable()
    {
        const string channelId = "824409821985636452";
        var originalChannelId = Environment.GetEnvironmentVariable("DISCORD_CHANNEL_ID");

        try
        {
            Environment.SetEnvironmentVariable("DISCORD_CHANNEL_ID", channelId);

            var applicationVariables = new BotApplicationVariableParser()
                .ParseTyped("SteamAPIKey,12345");

            Assert.Equal(channelId, applicationVariables["DISCORD_CHANNEL_ID"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable("DISCORD_CHANNEL_ID", originalChannelId);
        }
    }
}
