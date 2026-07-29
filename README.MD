# Player Count Discord Bot

This repository is maintained by [ventriquo](https://github.com/ventriquo). It is a fork of [GravityWolfNotAmused/PlayerCountDiscordBot](https://github.com/GravityWolfNotAmused/PlayerCountDiscordBot), with an added Project Zomboid server-status notification feature.

The bot polls a game server, updates its Discord activity and optional channel name with player counts, and provides a `/players` slash command.

## Server Status Notifications

Set `DISCORD_CHANNEL_ID` to the ID of the Discord text channel that should receive alerts. The bot must be able to view that channel and send messages in it.

For Docker Compose, set the variable in `DiscordPlayerCountBot/docker-compose.yml`:

```yaml
environment:
  DISCORD_CHANNEL_ID: "123456789012345678"
```

For a standard `Config.json` deployment, add it to `ApplicationTokens`; a process-level `DISCORD_CHANNEL_ID` environment variable is also supported:

```json
{
  "ApplicationTokens": {
    "SteamAPIKey": "your-steam-api-key",
    "DISCORD_CHANNEL_ID": "123456789012345678"
  }
}
```

The alert flow avoids poll-interval spam:

- The first successful poll establishes the server as online without sending a message.
- Three consecutive failed polls send `🚨 The Project Zomboid server is currently OFFLINE!` once.
- The first successful poll after that outage sends `✅ The Project Zomboid server is back ONLINE!` once.
- Starting the bot while the server is unavailable does not send an offline alert until the server has first been observed online.

## Configuration

The Docker example at `DiscordPlayerCountBot/docker-compose.yml` lists the supported bot and application variables. `BOT_CHANNELIDS` controls optional channel-name updates; `DISCORD_CHANNEL_ID` is specifically for status alert messages and can point to a different text channel.

See the upstream [project wiki](https://github.com/GravityWolfNotAmused/PlayerCountDiscordBot/wiki) for player-count provider configuration and deployment details.
