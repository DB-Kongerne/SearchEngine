# Troubleshooting Discord Bot Issues

## Setting Up Slash Commands

If your slash commands aren't appearing when you type `/` in Discord:

### Step 1: Check Server ID

1. Ensure you've set the correct server ID in `appsettings.json`
2. To get your server ID:
   - Enable Developer Mode in Discord (Settings > App Settings > Advanced > Developer Mode)
   - Right-click on your server icon and select "Copy ID"
   - Paste this ID in the `GuildId` field in `appsettings.json`

### Step 2: Check Bot Permissions

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications)
2. Select your application
3. Go to the "OAuth2" tab, then "URL Generator"
4. Ensure you've selected both the `bot` and `applications.commands` scopes
5. Re-invite the bot to your server using the generated URL

### Step 3: Registration Delay for Global Commands

If you're not using a server ID (global commands):
- Global command registration can take up to 1 hour to propagate across all Discord servers
- For faster testing, always use a server ID during development

### Step 4: Check Command Registration

To see if your commands are registered:
1. Run your bot and check the console output for "Slash commands registered!"
2. If commands are registered but not appearing in Discord, try restarting Discord

## Command Response Issues

If you see "Application did not respond" errors:

1. Check that you're using `RespondAsync()` or `DeferAsync()` followed by `FollowupAsync()`
2. Discord requires interactions to be acknowledged within 3 seconds
3. For commands that take longer to process, use `DeferAsync()` immediately

## Testing Your Slash Commands

To ensure your slash commands work:

1. Make sure you've added your server ID to `appsettings.json`
2. Run your bot: `dotnet run`
3. Check the console output for successful command registration
4. In your Discord server, type `/` to see if your commands appear
5. Try each command with its parameters

## Debugging Tips

If you encounter errors:

1. Check the console output for error messages
2. Verify that your bot has the correct permissions in your server
3. For testing, consider adding debug logging:

```csharp
// Add to your command methods
Console.WriteLine($"Command executed with parameters: {parameterName}");
```

## Security Reminder

🚨 **IMPORTANT SECURITY WARNING** 🚨

I noticed your Discord bot token is visible in the appsettings.json file. This token should be kept secret as it grants full access to your bot.

**You should immediately reset your bot token in the Discord Developer Portal:**

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications)
2. Select your application
3. Go to the "Bot" tab
4. Click "Reset Token"
5. Copy the new token and update it in your appsettings.json file
6. Do NOT share this token in any public places

## Slash Command Reference

Here are the slash commands you've implemented:

| Command | Description | Parameters |
|---------|-------------|------------|
| `/search` | Search for documents | `query`: Search terms<br>`results`: Number of results (optional) |
| `/casesensitive` | Toggle case sensitivity | `setting`: "On" or "Off" |
| `/help` | Show help information | None |
