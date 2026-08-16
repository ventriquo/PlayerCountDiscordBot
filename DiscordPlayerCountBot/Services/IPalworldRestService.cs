using DiscordPlayerCountBot.Data.Palworld;

namespace DiscordPlayerCountBot.Services;

public interface IPalworldRestService
{
    Task<PalworldMetricsResponse?> GetMetricsAsync(string address, int port, string adminPassword);
}
