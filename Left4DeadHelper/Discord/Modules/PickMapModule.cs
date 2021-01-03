using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class PickMapModule : ModuleBase<SocketCommandContext>
    {
        private const string Command = "map";

        private readonly ILogger<PickMapModule> _logger;
        private readonly Settings _settings;
        private readonly RNGCryptoServiceProvider _random;

        public PickMapModule(ILogger<PickMapModule> logger, Settings settings, RNGCryptoServiceProvider random)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        [Command(Command)]
        [Alias("maps")]
        [Summary("Picks a random map.")]
        public async Task HandleCommandAsync(string? firstArg = null, string? secondArg = null)
        {
            var maps = _settings.Left4DeadSettings.Maps;

            if (string.IsNullOrEmpty(firstArg))
            {
                firstArg = maps.DefaultCategory;
            }

            if ("help".Equals(firstArg, StringComparison.CurrentCultureIgnoreCase))
            {
                var linePrefix = $"{_settings.DiscordSettings.Prefixes[0]}{Constants.GroupL4d} {Command}";

                var outputMessage = $@"Usage:
**{linePrefix} help**: shows help
**{linePrefix} list**: lists the categories
**{linePrefix} list <category>**: lists the maps in the categories
**{linePrefix}**: pick a random map from the ""{firstArg}"" category
**{linePrefix} any** or **{linePrefix} all**: pick a random map from all of the categories
**{linePrefix} <category>**: pick a map from the given category";

                await ReplyAsync(outputMessage);
                return;
            }

            if ("list".Equals(firstArg, StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(secondArg))
                {
                    var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
                    await ReplyAsync($"The categories are:{Environment.NewLine}{categoriesStr}");
                }
                else
                {
                    if (maps.Categories.TryGetValue(secondArg, out var listCategoryMaps))
                    {
                        var mapsStr = string.Join(Environment.NewLine, listCategoryMaps.Select(k => $"- **{k}**"));
                        await ReplyAsync($"The maps in \"{secondArg}\" are:{Environment.NewLine}{mapsStr}");
                    }
                    else
                    {
                        var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
                        await ReplyAsync(
                            $"There's no category called \"{secondArg}\" 🤷. " +
                            $"The categories are:{Environment.NewLine}{categoriesStr}");
                    }
                }
                return;
            }

            if ("any".Equals(firstArg, StringComparison.CurrentCultureIgnoreCase)
                || "all".Equals(firstArg, StringComparison.CurrentCultureIgnoreCase))
            {
                var allMaps = maps.Categories.Values.SelectMany(m => m).ToList();
                var rollResult = _random.RollDice((byte)allMaps.Count);
                var map = allMaps[rollResult - 1];

                await ReplyAsync($"You should play **{map}**! (from all maps)");

                return;
            }

            // Pick a map!
            if (maps.Categories.TryGetValue(firstArg, out var categoryMaps))
            {
                var rollResult = _random.RollDice((byte)categoryMaps.Count);
                var map = categoryMaps[rollResult - 1];

                await ReplyAsync($"You should play **{map}**! (from the\"{firstArg}\" list)");
                
                return;
            }
            else
            {
                var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
                await ReplyAsync($"There's no category called \"{firstArg}\" 🤷. " +
                    $"The categories are:{Environment.NewLine}{categoriesStr}");
                return;
            }
        }
    }
}
