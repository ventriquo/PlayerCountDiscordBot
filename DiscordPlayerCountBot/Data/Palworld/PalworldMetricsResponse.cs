namespace DiscordPlayerCountBot.Data.Palworld;

public sealed class PalworldMetricsResponse
{
    [JsonProperty("currentplayernum")]
    public int CurrentPlayerCount { get; set; }

    [JsonProperty("maxplayernum")]
    public int MaxPlayerCount { get; set; }
}
