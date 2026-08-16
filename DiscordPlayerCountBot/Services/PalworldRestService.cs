using DiscordPlayerCountBot.Data.Palworld;
using DiscordPlayerCountBot.Http;

namespace DiscordPlayerCountBot.Services;

public sealed class PalworldRestService : IPalworldRestService
{
    public async Task<PalworldMetricsResponse?> GetMetricsAsync(string address, int port, string adminPassword)
    {
        using var httpClient = new HttpExecuter();
        // A failed route should not block the bot's update loop for HttpClient's default timeout.
        httpClient.HttpClient.Timeout = TimeSpan.FromSeconds(10);
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"admin:{adminPassword}"));

        return await httpClient.GET<object, PalworldMetricsResponse>(
            $"http://{address}:{port}/v1/api/metrics",
            authToken: new Tuple<string, string>("Authorization", $"Basic {credentials}"));
    }
}
