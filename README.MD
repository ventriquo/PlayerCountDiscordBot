# Player Count Discord Bot

This repository is maintained by [ventriquo](https://github.com/ventriquo). It is a fork of [GravityWolfNotAmused/PlayerCountDiscordBot](https://github.com/GravityWolfNotAmused/PlayerCountDiscordBot) with Project Zomboid status notifications and Docker Compose deployment guidance.

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

Run Docker Compose from the `DiscordPlayerCountBot` directory:

```powershell
docker compose up -d --build
docker compose logs -f discordcounter
```

Set these environment variables in `docker-compose.yml` or your deployment platform's Compose environment editor:

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

To enable live channel renaming, add `BOT_CHANNELIDS` with the target channel ID and grant the bot `Manage Channels`. Do not use it as the alert channel setting.

## Support

For crash logs and support, join [Discord](https://discord.gg/TarcPvb7H7).
