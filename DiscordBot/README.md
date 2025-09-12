# Discord Bot for Search Engine

This Discord bot allows you to use the search engine functionality directly from Discord using slash commands.

## Setup Instructions

### 1. Create a Discord Application and Bot

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications)
2. Click on "New Application" and give it a name (e.g., "Search Engine Bot")
3. Go to the "Bot" tab and click "Add Bot"
4. Under the "Token" section, click "Reset Token" and confirm, then copy your bot token
5. Under the "Bot" tab, make sure to enable the following permissions:
   - In the "Bot Permissions" section, check "applications.commands"

### 2. Configure the Bot

For security reasons, you should never store your Discord bot token in files that will be committed to a repository. Instead, follow these steps:

1. The `appsettings.json` file contains placeholder values:
```json
{
  "DiscordSettings": {
    "Token": "YOUR_DISCORD_BOT_TOKEN_HERE",
    "Prefix": "!",
    "GuildId": "YOUR_SERVER_ID_HERE"
  },
  "DatabaseType": "Sqlite"
}
```

2. Set your Discord bot token using one of these secure methods:

   **Option A: Using a local configuration file (recommended for development)**
   
   Create a new file called `appsettings.local.json` in the DiscordBot project folder:
   ```json
   {
     "DiscordSettings": {
       "Token": "YOUR_ACTUAL_BOT_TOKEN_HERE"
     }
   }
   ```
   
   **Option B: Using environment variables (recommended for production)**
   
   Set an environment variable named `DISCORD_BOT_TOKEN`:
   
   ```powershell
   # Windows PowerShell (temporary, for current session)
   $env:DISCORD_BOT_TOKEN="YOUR_ACTUAL_BOT_TOKEN_HERE"
   
   # Windows Command Prompt (temporary, for current session)
   set DISCORD_BOT_TOKEN=YOUR_ACTUAL_BOT_TOKEN_HERE
   
   # Windows (permanent, system-wide)
   [Environment]::SetEnvironmentVariable("DISCORD_BOT_TOKEN", "YOUR_ACTUAL_BOT_TOKEN_HERE", "Machine")
   ```

3. All token-containing files are listed in `.gitignore` and will not be committed to the repository.

4. Set your Discord server (guild) ID in the `appsettings.json` file.

> **Note**: To get your server ID, enable Developer Mode in Discord (Settings > App Settings > Advanced > Developer Mode), then right-click on your server icon and select "Copy ID".

### 3. Invite the Bot to Your Server

1. In the Discord Developer Portal, go to the "OAuth2" tab, then "URL Generator"
2. Select the following scopes:
   - bot
   - applications.commands
3. Select the following bot permissions:
   - Send Messages
   - Embed Links
   - Read Message History
   - Use Slash Commands
4. Copy the generated URL and open it in your browser
5. Select the server where you want to add the bot and authorize it

> **Important**: The bot needs the `applications.commands` scope to register slash commands. If you've already added the bot to your server without this scope, you'll need to re-add it.

### 4. Build and Run the Bot

Using Visual Studio:
1. Set DiscordBot as the startup project
2. Press F5 to build and run

Using command line:
```powershell
cd "c:\path\to\SearchEngine\DiscordBot"
dotnet run
```

## Usage

The bot now responds to the following slash commands:

- `/search [query] [results]` - Search for documents containing the specified terms
  - `query`: The search terms to look for
  - `results`: (Optional) Number of results to display (default: 10, max: 20)
- `/casesensitive [setting]` - Toggle case sensitivity for searches
  - `setting`: Choose either "On" or "Off"
- `/help` - Show help information for all available commands

Example:
```
/search query:example search terms results:5
```

> **Note**: Slash commands will automatically appear when you type `/` in the Discord chat input field. This provides a more user-friendly interface with autocompletion and parameter hints.

## Troubleshooting

If the bot doesn't respond to commands:
- Make sure the bot has the necessary permissions in the Discord server
- Check that the bot token is correctly set in `appsettings.json`
- Verify that the database connection is working correctly
- Check the console output for any error messages

## Note on Database Selection

When starting the bot, it will use the database type specified in `appsettings.json`. Make sure:
- If using SQLite, the database file is properly set up from previous indexing
- If using Postgres, your Postgres server is running and accessible with the correct credentials

## Security Considerations

Keep your bot token secure:
- Never commit the token to source control
- Consider using environment variables or a secure vault for production deployments
- If your token is compromised, reset it immediately in the Discord Developer Portal
- **IMPORTANT**: If you've added your token directly in appsettings.json and shared the code (including in chat tools like this), reset your token immediately in the Discord Developer Portal
