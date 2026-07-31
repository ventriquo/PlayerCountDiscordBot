using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;

namespace DiscordPlayerCountBot.Services;

public sealed class HealthServer(UpdateController updateController) : LoggableClass
{
    private const string HealthPortVariable = "HEALTH_PORT";

    public async Task StartAsync()
    {
        var port = GetHealthPort();
        if (port == null)
        {
            Info("Health endpoint is disabled. Set HEALTH_PORT to enable it.");
            return;
        }

        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(port.Value));

        var app = builder.Build();
        app.MapGet("/healthz", GetHealthResponse);

        await app.StartAsync();
        Info($"Health endpoint listening on port {port.Value} at /healthz.");
    }

    private IResult GetHealthResponse()
    {
        var health = updateController.GetHealthSnapshot();
        var response = new { status = health.Status.ToString().ToLowerInvariant() };

        return health.IsHealthy
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private int? GetHealthPort()
    {
        var value = Environment.GetEnvironmentVariable(HealthPortVariable);
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value, out var port) && port is > 0 and <= 65535)
            return port;

        Warn($"{HealthPortVariable} must be a valid TCP port. The health endpoint is disabled.");
        return null;
    }
}
