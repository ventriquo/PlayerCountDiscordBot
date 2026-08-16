using DiscordPlayerCountBot.ViewModels;

namespace DiscordPlayerCountBot.ViewModels.Palworld;

public sealed class PalworldViewModel : BaseViewModel
{
    public double ServerFps { get; set; }
    public double ServerFpsAverage { get; set; }
    public double ServerFrameTime { get; set; }
    public int Days { get; set; }
    public int BaseCampCount { get; set; }
    public int UptimeSeconds { get; set; }
}
