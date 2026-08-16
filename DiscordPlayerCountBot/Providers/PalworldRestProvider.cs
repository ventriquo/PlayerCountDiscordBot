using DiscordPlayerCountBot.Attributes;
using DiscordPlayerCountBot.Bot;
using DiscordPlayerCountBot.Enums;
using DiscordPlayerCountBot.Providers.Base;
using DiscordPlayerCountBot.Services;
using DiscordPlayerCountBot.ViewModels;
using DiscordPlayerCountBot.ViewModels.Palworld;

namespace DiscordPlayerCountBot.Providers;

[Name("Palworld REST")]
public sealed class PalworldRestProvider(IPalworldRestService service) : ServerInformationProvider
{
    private const string PasswordVariable = "PalworldAdminPassword";

    public override DataProvider GetRequiredProviderType() => DataProvider.PALWORLDREST;

    public override async Task<BaseViewModel?> GetServerInformation(
        BotInformation information,
        Dictionary<string, string> applicationVariables)
    {
        try
        {
            var addressAndPort = information.GetAddressAndPort();
            var metrics = await service.GetMetricsAsync(
                addressAndPort.Item1,
                addressAndPort.Item2,
                applicationVariables[PasswordVariable]);

            if (metrics == null)
                throw new ApplicationException($"Palworld REST API at {information.Address} returned no metrics.");

            HandleLastException(information);

            return new PalworldViewModel
            {
                Address = addressAndPort.Item1,
                Port = addressAndPort.Item2,
                Players = metrics.CurrentPlayerCount,
                MaxPlayers = metrics.MaxPlayerCount,
                QueuedPlayers = 0,
                ServerFps = metrics.ServerFps,
                ServerFpsAverage = metrics.ServerFpsAverage,
                ServerFrameTime = metrics.ServerFrameTime,
                Days = metrics.Days,
                BaseCampCount = metrics.BaseCampCount,
                UptimeSeconds = metrics.UptimeSeconds
            };
        }
        catch (Exception exception)
        {
            HandleException(exception, information.Id.ToString());
            return null;
        }
    }
}
