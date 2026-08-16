namespace DiscordPlayerCountBot.Data.Palworld;

public sealed class PalworldMetricsResponse
{
    [JsonProperty("currentplayernum")]
    public int CurrentPlayerCount { get; set; }

    [JsonProperty("maxplayernum")]
    public int MaxPlayerCount { get; set; }

    [JsonProperty("serverfps")]
    public double ServerFps { get; set; }

    [JsonProperty("serverfpsaverage")]
    public double ServerFpsAverage { get; set; }

    [JsonProperty("serverframetime")]
    public double ServerFrameTime { get; set; }

    [JsonProperty("days")]
    public int Days { get; set; }

    [JsonProperty("basecampnum")]
    public int BaseCampCount { get; set; }

    [JsonProperty("uptime")]
    public int UptimeSeconds { get; set; }
}
