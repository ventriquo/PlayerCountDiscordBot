using DiscordPlayerCountBot.EnvironmentParser.Base;

namespace DiscordPlayerCountBot.EnvironmentParser;

public class BotApplicationVariableParser : EnvironmentParserBase<Dictionary<string, string>>
{
    private const string DiscordChannelIdKey = "DISCORD_CHANNEL_ID";

    public override string GetKey() => "BOT_APPLICATION_VARIABLES";
    public override Dictionary<string, string> ParseTyped(string? environmentVariable)
    {
        var applicationVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(environmentVariable))
        {
            foreach (var pair in environmentVariable.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var values = pair.Split(',', 2, StringSplitOptions.TrimEntries);

                if (values.Length != 2 || string.IsNullOrWhiteSpace(values[0]) || string.IsNullOrWhiteSpace(values[1]))
                    throw new FormatException($"Invalid application variable '{pair}'. Expected 'Name,Value'.");

                applicationVariables[values[0]] = values[1];
            }
        }

        var discordChannelId = Environment.GetEnvironmentVariable(DiscordChannelIdKey);
        if (!string.IsNullOrWhiteSpace(discordChannelId))
        {
            if (!ulong.TryParse(discordChannelId, out _))
                throw new FormatException($"{DiscordChannelIdKey} must be a valid Discord channel ID.");

            applicationVariables[DiscordChannelIdKey] = discordChannelId.Trim();
        }

        return applicationVariables;
    }
}
