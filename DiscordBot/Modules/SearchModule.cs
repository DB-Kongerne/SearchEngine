using Discord;
using Discord.Interactions;
using SearchAPI;
using Shared;
using System.Text;

namespace DiscordBot.Modules
{
    // Using InteractionModuleBase instead of ModuleBase for slash commands
    public class SearchModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly SearchLogic _searchLogic;

        public SearchModule(SearchLogic searchLogic)
        {
            _searchLogic = searchLogic;
        }

        [SlashCommand("search", "Searches for documents containing the specified terms")]
        public async Task SearchAsync(
            [Summary("query", "The search terms to look for")] string searchQuery, 
            [Summary("results", "Number of results to display")][MinValue(1)][MaxValue(20)] int results = 10)
        {
            // Defer the response to give us more time to process
            await DeferAsync();
            
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await FollowupAsync("Please provide search terms.", ephemeral: true);
                return;
            }

            var query = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = _searchLogic.Search(query, results);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Search Results for: {string.Join(" ", result.Query)}")
                .WithColor(Color.Blue)
                .WithFooter($"Found {result.Hits} documents in {result.TimeUsed.TotalMilliseconds:F1} ms");

            if (result.Ignored.Count > 0)
            {
                embedBuilder.AddField("Ignored Terms", string.Join(", ", result.Ignored));
            }

            if (result.DocumentHits.Count == 0)
            {
                embedBuilder.WithDescription("No results found.");
            }
            else
            {
                var resultDescription = new StringBuilder();
                int idx = 1;
                
                foreach (var doc in result.DocumentHits)
                {
                    resultDescription.AppendLine($"{idx}. **{doc.Document.mUrl}**");
                    resultDescription.AppendLine($"   Hits: {doc.NoOfHits} • Indexed: {doc.Document.mIdxTime}");
                    
                    if (doc.Missing.Count > 0)
                    {
                        resultDescription.AppendLine($"   Missing terms: {string.Join(", ", doc.Missing)}");
                    }
                    
                    resultDescription.AppendLine();
                    idx++;
                    
                    // Discord has a character limit for descriptions, so let's limit the results
                    if (resultDescription.Length > 4000)
                    {
                        resultDescription.AppendLine("*Results truncated due to size limitations...*");
                        break;
                    }
                }
                
                embedBuilder.WithDescription(resultDescription.ToString());
            }

            await FollowupAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("casesensitive", "Toggles case sensitivity for searches")]
        public async Task CaseSensitiveAsync(
            [Summary("setting", "Turn case sensitivity on or off")][Choice("On", "on")][Choice("Off", "off")] string setting)
        {
            bool enabled = setting.Equals("on", StringComparison.OrdinalIgnoreCase);
            _searchLogic.SetCaseSensitivity(enabled);
            
            await RespondAsync($"Case sensitivity is now {(enabled ? "ON" : "OFF")}.");
        }

        [SlashCommand("help", "Shows help information for the search bot")]
        public async Task HelpAsync()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Search Bot Help")
                .WithColor(Color.Gold)
                .WithDescription("Here are the commands you can use:")
                .AddField("/search [query] [results]", "Search for documents containing the specified terms")
                .AddField("/casesensitive [setting]", "Toggles case sensitivity for searches")
                .AddField("/help", "Shows this help information");

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
