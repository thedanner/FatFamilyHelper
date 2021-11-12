using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2, Constants.GroupLfd, Constants.GroupLfd2)]
    public class PickMapModule : ModuleBase<SocketCommandContext>, ICommandModule
    {
        private const string Command = "map";
        private const string CommandAlias = "maps";

        private const string ArgList = "list";
        private static readonly string[] ArgsAny = { "any", "all" };

        private readonly ILogger<PickMapModule> _logger;
        private readonly Settings _settings;
        
        public PickMapModule(ILogger<PickMapModule> logger, Settings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command(Command)]
        [Alias(CommandAlias)]
        [Summary("Picks a random map.")]
        public async Task HandleCommandAsync(string? firstArg = null, string? secondArg = null)
        {
            var maps = _settings.Left4DeadSettings.Maps;

            if (string.IsNullOrEmpty(firstArg))
            {
                firstArg = maps.DefaultCategory;
            }

            if (ArgList.Equals(firstArg, StringComparison.CurrentCultureIgnoreCase))
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

            if (ArgsAny.Contains(firstArg, StringComparer.CurrentCultureIgnoreCase))
            {
                var allMaps = maps.Categories.Values.SelectMany(m => m).ToList();
                var map = RandomHelper.PickSecureRandom(allMaps);

                await ReplyAsync($"You should play **{map}**! (from all maps)");

                return;
            }

            // Pick a map!
            if (maps.Categories.TryGetValue(firstArg, out var categoryMaps))
            {
                var map = RandomHelper.PickSecureRandom(categoryMaps);

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

        public string GetGeneralHelpMessage(HelpContext helpContext) =>
            $"  - `{helpContext.GenericCommandExample} {ArgList}`: lists the categories\n" +
            $"  - `{helpContext.GenericCommandExample} {ArgList} <category>`: lists the maps in the categories\n" +
            $"  - `{helpContext.GenericCommandExample}`: pick a random map from the \"{_settings.Left4DeadSettings.Maps.DefaultCategory}\" category\n" +
            $"  - `{helpContext.GenericCommandExample} <{string.Join("|", ArgsAny)}>`: pick a random map from all of the categories\n" +
            $"  - `{helpContext.GenericCommandExample} <category>`: pick a map from the given category\n" +
            $"    Base command aliases: {helpContext.GetGroupAliasesString()}.\n" +
            $"    Sub-command aliases {helpContext.GetCommandAliasesString()}.";
    }
}
