using DiscordPlayerCountBot.EnvironmentParser;

namespace DiscordPlayerCountBot.Tests;

[Collection("Configuration Test Suite")]
public class BotApplicationVariableParserTests
{
    [Fact]
    public void AddsDiscordChannelIdFromDedicatedEnvironmentVariable()
    {
        const string channelId = "824409821985636452";
        var originalChannelId = System.Environment.GetEnvironmentVariable("DISCORD_CHANNEL_ID");

        try
        {
            System.Environment.SetEnvironmentVariable("DISCORD_CHANNEL_ID", channelId);

            var applicationVariables = new BotApplicationVariableParser()
                .ParseTyped("SteamAPIKey,12345");

            Assert.Equal(channelId, applicationVariables["DISCORD_CHANNEL_ID"]);
        }
        finally
        {
            System.Environment.SetEnvironmentVariable("DISCORD_CHANNEL_ID", originalChannelId);
        }
    }
}
