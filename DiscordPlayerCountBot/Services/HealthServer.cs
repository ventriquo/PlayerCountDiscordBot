using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
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
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(port.Value));

        var app = builder.Build();
        app.UseCors();
        app.MapGet("/healthz", GetHealthResponse);
        app.MapGet("/server-healthz", GetServerHealthResponse);
        app.MapGet("/api/servers", GetServerSnapshotsResponse);

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

    private IResult GetServerHealthResponse()
    {
        var servers = updateController.GetServerSnapshots();
        var status = GetAggregateServerStatus(servers);
        var response = new { status, servers = servers.Select(server => server.ToPublicSnapshot()).ToArray() };

        return status == "online"
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private IResult GetServerSnapshotsResponse()
    {
        return Results.Ok(new
        {
            servers = updateController.GetServerSnapshots()
                .Select(server => server.ToPublicSnapshot())
                .ToArray()
        });
    }

    private static string GetAggregateServerStatus(IReadOnlyList<ServerSnapshot> servers)
    {
        if (servers.Count == 0)
            return "unknown";

        if (servers.Any(server => server.Status == "offline"))
            return "offline";

        return servers.Any(server => server.Status == "unknown")
            ? "unknown"
            : "online";
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
