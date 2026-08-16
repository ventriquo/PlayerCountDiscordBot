# Player Count Discord Bot

This repository is maintained by [ventriquo](https://github.com/ventriquo). It is a fork of [GravityWolfNotAmused/PlayerCountDiscordBot](https://github.com/GravityWolfNotAmused/PlayerCountDiscordBot) with Project Zomboid status notifications, native Palworld REST API support, and Docker Compose deployment guidance.

The bot polls a game server, updates its Discord activity with player counts, and provides a `/players` slash command.

## Windows Installation

1. Download `DiscordPlayerCountBot-v1.0.0-win-x64.exe` from the [v1.0.0 release](https://github.com/ventriquo/PlayerCountDiscordBot/releases/tag/v1.0.0).
2. Run the executable once in an empty folder. It creates `Config.json`, then exits.
3. Edit `Config.json` with the Discord bot token, server address, Steam API key, and optional alert channel ID.
4. Run the executable again.

The release is self-contained for Windows x64. It is not code-signed, so Windows SmartScreen may show a warning on first run.

```json
{
  "UpdateTime": 30,
  "ServerInformation": [
    {
      "Name": "Project Zomboid",
      "Address": "203.0.113.10:16261",
      "Token": "your-discord-bot-token",
      "Status": 0,
      "UseNameAsLabel": false,
      "ProviderType": 0
    }
  ],
  "ApplicationTokens": {
    "SteamAPIKey": "your-steam-api-key",
    "DISCORD_CHANNEL_ID": "123456789012345678"
  }
}
```

## Server Status Notifications

`DISCORD_CHANNEL_ID` is the Discord text channel that receives alerts. The bot needs `View Channel` and `Send Messages` permission there.

- The first successful poll establishes the server as online without sending a message.
- Three consecutive failed polls send `🚨 The Project Zomboid server is currently OFFLINE!` once.
- The first successful poll after that outage sends `✅ The Project Zomboid server is back ONLINE!` once.

`ChannelID` and `BOT_CHANNELIDS` are unrelated to notifications. They rename a Discord text or voice channel with the live player count, require `Manage Channels`, and should be omitted unless that feature is intended.

## Docker Compose

The committed [docker-compose.yml](DiscordPlayerCountBot/docker-compose.yml) deliberately contains variable references rather than secrets or sample server values. Set the values in a `.env` file beside it for a local deployment, or in your deployment platform's environment-variable screen.

Run Docker Compose from the `DiscordPlayerCountBot` directory:

```powershell
docker compose up -d --build
docker compose logs -f discordcounter
```

Set these environment variables:

```yaml
environment:
  ISDOCKER: "true"
  BOT_NAMES: "Project Zomboid"
  BOT_PUBADDRESSES: "203.0.113.10"
  BOT_PORTS: "16261"
  BOT_DISCORD_TOKENS: "your-discord-bot-token"
  BOT_USENAMETAGS: "false"
  BOT_STATUSES: "0"
  BOT_PROVIDERTYPES: "0"
  BOT_UPDATE_TIME: "30"
  DISCORD_CHANNEL_ID: "123456789012345678"
  BOT_APPLICATION_VARIABLES: "SteamAPIKey,your-steam-api-key"
```

All `BOT_*` server lists use `;` to configure multiple servers, and values at the same position belong to the same server. Replace `203.0.113.10` with the Project Zomboid server address reachable from the container. `BOT_PROVIDERTYPES: "0"` selects the Steam provider used by the Project Zomboid example. `BOT_APPLICATION_VARIABLES` uses `Name,Value` pairs separated by `;`.

## Palworld REST API Support

This fork supports Palworld's native REST API as a dedicated player-count provider. It does not use the legacy A2S query protocol or fragile RCON text parsing. The provider calls Palworld's authenticated `GET /v1/api/metrics` endpoint and maps `currentplayernum` and `maxplayernum` into the bot's normal player-count display and status-alert flow.

Enable the API in `PalWorldSettings.ini`:

```ini
RESTAPIEnabled=True
RESTAPIPort=8212
```

Restart the Palworld server after changing these settings. The API uses Basic Auth with username `admin` and the server's `AdminPassword`. Keep port `8212` private and allow access only from the bot host or its private network.

For a Palworld server in Docker Compose or Coolify, use:

```text
BOT_NAMES=Palworld
BOT_PUBADDRESSES=<palworld-server-ip>
BOT_PORTS=8212
BOT_PROVIDERTYPES=7
BOT_APPLICATION_VARIABLES=PalworldAdminPassword,<palworld-admin-password>
```

`BOT_PROVIDERTYPES=7` selects the Palworld REST provider. `BOT_PORTS` is the REST API port, not Palworld's game port (`8211`) or RCON port (`25575`). The password is kept in `BOT_APPLICATION_VARIABLES`; mark it as a secret in Coolify. For the Windows `Config.json` deployment, use `ProviderType: 7`, set the server `Address` to `<palworld-server-ip>:8212`, and add `PalworldAdminPassword` to `ApplicationTokens`.

To enable live channel renaming, add `BOT_CHANNELIDS` with the target channel ID and grant the bot `Manage Channels`. Do not use it as the alert channel setting.

## Coolify Deployment

1. Create a new **Docker Compose** resource from `https://github.com/ventriquo/PlayerCountDiscordBot.git`.
2. Select the `latest` branch. Set **Base Directory** to `/DiscordPlayerCountBot` and the Compose file path to `/docker-compose.yml`.
3. In Coolify's **Environment Variables** panel, add the variables from the Compose example above. Use your own server values; for the Polaris Zomboid server they are `BOT_NAMES=Polaris Zomboid`, `BOT_PUBADDRESSES=103.47.225.60`, `BOT_PORTS=16261`, and `BOT_PROVIDERTYPES=0`.
4. Mark `BOT_DISCORD_TOKENS` and the Steam API key inside `BOT_APPLICATION_VARIABLES` as secrets. Set `DISCORD_CHANNEL_ID` to the Discord text channel ID that should receive alerts.
5. Deploy the resource. This is a background bot, so it does not expose a web port and does not need a domain.

Do not add `BOT_CHANNELIDS` unless you want player-count channel renaming and have granted the Discord bot `Manage Channels`. It is not the status-alert setting.

## Uptime Kuma Monitoring

Docker deployments expose a health endpoint on port `8080` at `/healthz`. It returns `200` only when the bot configuration is loaded, every Discord client is connected, and the player-count update loop has completed recently. It returns `503` while starting, disconnected, or stalled.

To monitor a Coolify deployment from Uptime Kuma:

1. Add a dedicated health domain to the `discordcounter` service in Coolify, for example `https://playercount-health.example.com:8080`.
2. Point that hostname's DNS record to the Coolify server and redeploy. `HEALTH_PORT` defaults to `8080` in the supplied Compose file.
3. In Kuma, create an `HTTP(s)` monitor with `https://playercount-health.example.com/healthz`, a `60` second interval, `10` second timeout, and `1` retry. A `200` response is healthy; after one retry, two consecutive failed checks create an incident.

The health endpoint monitors the bot process and Discord connection. It deliberately does not become unhealthy when the Project Zomboid server is offline: the bot's existing three-failed-poll Discord notification covers that separate condition.

## Support

For crash logs and support, join [Discord](https://discord.gg/TarcPvb7H7).
