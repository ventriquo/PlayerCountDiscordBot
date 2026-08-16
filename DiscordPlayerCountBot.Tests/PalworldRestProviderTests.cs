using DiscordPlayerCountBot.Bot;
using DiscordPlayerCountBot.Data.Palworld;
using DiscordPlayerCountBot.Enums;
using DiscordPlayerCountBot.Providers;
using DiscordPlayerCountBot.Services;

namespace DiscordPlayerCountBot.Tests;

public class PalworldRestProviderTests
{
    [Fact]
    public async Task MapsPalworldMetricsToTheCommonPlayerViewModel()
    {
        var provider = new PalworldRestProvider(new StubPalworldRestService
        {
            Response = new PalworldMetricsResponse
            {
                CurrentPlayerCount = 7,
                MaxPlayerCount = 32
            }
        });

        var result = await provider.GetServerInformation(
            new BotInformation
            {
                Name = "Palworld",
                Address = "127.0.0.1:8212",
                Token = "unused",
                ProviderType = (int)DataProvider.PALWORLDREST
            },
            new Dictionary<string, string>
            {
                ["PalworldAdminPassword"] = "test-password"
            });

        Assert.NotNull(result);
        Assert.Equal(7, result!.Players);
        Assert.Equal(32, result.MaxPlayers);
        Assert.Equal(0, result.QueuedPlayers);
    }

    [Fact]
    public void UsesAProviderValueAfterExistingProviderValues()
    {
        Assert.Equal(7, (int)DataProvider.PALWORLDREST);
    }

    private sealed class StubPalworldRestService : IPalworldRestService
    {
        public PalworldMetricsResponse? Response { get; init; }

        public Task<PalworldMetricsResponse?> GetMetricsAsync(string address, int port, string adminPassword)
        {
            return Task.FromResult(Response);
        }
    }
}
