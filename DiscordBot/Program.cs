using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Database;
using System.Reflection;

namespace DiscordBot
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client = null!;
        private InteractionService _interactions = null!;
        private IServiceProvider _services = null!;

        public static Task Main(string[] args) => new Program().MainAsync();

        public Program()
        {
            // Create configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true)  // Add local config if it exists
                .AddEnvironmentVariables();  // Add environment variables

            _config = builder.Build();
        }

        public async Task MainAsync()
        {
            // Configure Discord client
            var discordConfig = new DiscordSocketConfig
            {
                // Required intents for slash commands
                GatewayIntents = GatewayIntents.Guilds
            };

            _client = new DiscordSocketClient(discordConfig);
            
            // Initialize services
            _services = ConfigureServices();
            
            // Create interaction service
            _interactions = new InteractionService(_client);

            _client.Log += LogAsync;
            _interactions.Log += LogAsync;

            // Register handlers
            _client.Ready += ReadyAsync;
            _client.InteractionCreated += InteractionCreatedAsync;

            // Login and start the bot
            var token = _config["DiscordSettings:Token"];
            
            // Also check environment variable as an alternative
            if (string.IsNullOrWhiteSpace(token))
            {
                token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            }
            
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Error: No Discord bot token found. Please set your bot token in one of these ways:");
                Console.WriteLine("1. In appsettings.local.json file (recommended for development)");
                Console.WriteLine("2. As an environment variable named DISCORD_BOT_TOKEN");
                Console.WriteLine("3. In appsettings.json (not recommended for production or source control)");
                return;
            }
            
            Console.WriteLine("Discord bot token found. Logging in...");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(Timeout.Infinite);
        }

        // Called when the client is ready to process events
        private async Task ReadyAsync()
        {
            // Register all modules that are public and inherit InteractionModuleBase<T>
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            
            if (ulong.TryParse(_config["DiscordSettings:GuildId"], out ulong guildId) && guildId != 0)
            {
                // Register commands for a specific guild for instant testing
                Console.WriteLine($"Registering commands to guild with ID: {guildId}");
                await _interactions.RegisterCommandsToGuildAsync(guildId, true);
            }
            else
            {
                // Register globally (can take up to an hour to propagate)
                Console.WriteLine("Registering commands globally");
                await _interactions.RegisterCommandsGloballyAsync(true);
            }
            
            Console.WriteLine("Slash commands registered!");
        }

        private async Task InteractionCreatedAsync(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context for the interaction
                var context = new SocketInteractionContext(_client, interaction);
                
                // Execute the interaction
                var result = await _interactions.ExecuteCommandAsync(context, _services);
                
                // If the interaction failed, log the error
                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Error handling interaction: {result.Error} - {result.ErrorReason}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception handling interaction: {ex}");
                
                // If we receive an error, respond to let the user know
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => 
                    {
                        try
                        {
                            // Try to respond with an error message
                            await msg.Result.ModifyAsync(props => props.Content = "An error occurred while processing your command.");
                        }
                        catch
                        {
                            // If we can't modify the original response, try to respond with a new message
                            if (interaction.HasResponded)
                                await interaction.FollowupAsync("An error occurred while processing your command.", ephemeral: true);
                            else
                                await interaction.RespondAsync("An error occurred while processing your command.", ephemeral: true);
                        }
                    });
                }
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices()
        {
            var databaseType = _config["DatabaseType"] == "Postgres" ? 
                DatabaseFactory.DatabaseType.Postgres : 
                DatabaseFactory.DatabaseType.Sqlite;

            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_config)
                .AddSingleton(DatabaseFactory.CreateDatabase(databaseType))
                .AddSingleton<SearchAPI.SearchLogic>();

            return services.BuildServiceProvider();
        }
    }
}
