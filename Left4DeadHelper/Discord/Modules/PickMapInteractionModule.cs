using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using Discord.Interactions;
using System.Collections.Generic;

namespace Left4DeadHelper.Discord.Modules;

[Group("maps", "Left 4 Dead 2 maps commands")]
public class PickMapInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    internal const string ArgValueAny = "any";

    private readonly ILogger<PickMapInteractionModule> _logger;
    private readonly Settings _settings;

    public PickMapInteractionModule(ILogger<PickMapInteractionModule> logger, Settings settings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    [SlashCommand("pick", "Picks a map randomly")]
    public async Task HandleCommandAsync(
        [Summary(description: "map category"), Autocomplete(typeof(PickMapAutocompleteHandler))]
        string? category = null)
    {
        var maps = _settings.Left4DeadSettings.Maps;

        var q = ArgValueAny.Equals(category, StringComparison.CurrentCultureIgnoreCase);

        if (ArgValueAny.Equals(category, StringComparison.CurrentCultureIgnoreCase))
        {
            var allMaps = maps.Categories.Values.SelectMany(m => m).ToList();
            var map = RandomHelper.PickSecureRandom(allMaps);
            await RespondAsync($"You should play **{map}**! (from all maps)");
            return;
        }

        if (string.IsNullOrEmpty(category))
        {
            category = maps.DefaultCategory;
        }

        if (maps.Categories.TryGetValue(category, out var categoryMaps))
        {
            var map = RandomHelper.PickSecureRandom(categoryMaps);
            await RespondAsync($"You should play **{map}**! (from {category} maps)");
            return;
        }

        var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
        await RespondAsync($"There's no category called \"{category}\" 🤷. " +
            $"The categories are:{Environment.NewLine}{categoriesStr}");
    }

    [Group("list", "List categories and maps")]
    public class ListIntractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<ListIntractionModule> _logger;
        private readonly Settings _settings;

        public ListIntractionModule(ILogger<ListIntractionModule> logger, Settings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [SlashCommand("categories", "List categories")]
        public async Task HandleCategoriesAsync()
        {
            var maps = _settings.Left4DeadSettings.Maps;
            var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
            await RespondAsync($"The categories are:{Environment.NewLine}{categoriesStr}");
        }

        [SlashCommand("choices", "List maps, in the given category if specified")]
        public async Task HandleMapsAsync(
            [Summary(description: "map category"), Autocomplete(typeof(PickMapAutocompleteHandler))]
            string? category = null)
        {
            var maps = _settings.Left4DeadSettings.Maps;

            if (string.IsNullOrEmpty(category))
            {
                category = maps.DefaultCategory;
            }

            if (ArgValueAny.Equals(category, StringComparison.CurrentCultureIgnoreCase))
            {
                var lines = new List<string>();

                foreach (var mapCat in maps.Categories)
                {
                    lines.Add($"**{mapCat.Key}** maps:");
                    foreach (var map in mapCat.Value)
                    {
                        lines.Add($"- {map}");
                    }
                    lines.Add("");
                }

                lines.RemoveAt(lines.Count - 1);

                await RespondAsync(string.Join("\n", lines));

                return;
            }

            if (maps.Categories.TryGetValue(category, out var listCategoryMaps))
            {
                var mapsStr = string.Join(Environment.NewLine, listCategoryMaps.Select(k => $"- **{k}**"));
                await RespondAsync($"The {category} maps are:{Environment.NewLine}{mapsStr}");
                return;
            }

            var categoriesStr = string.Join(Environment.NewLine, maps.Categories.Keys.Select(k => $"- **{k}**"));
            await RespondAsync(
                $"There's no category called \"{category}\" 🤷. " +
                $"The categories are:{Environment.NewLine}{categoriesStr}");
        }
    }
}
