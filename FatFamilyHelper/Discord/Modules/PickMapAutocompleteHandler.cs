using Discord;
using Discord.Interactions;
using System.Threading.Tasks;
using System;
using FatFamilyHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FatFamilyHelper.Discord.Modules;

public class PickMapAutocompleteHandler : AutocompleteHandler
{
    private readonly ILogger<PickMapAutocompleteHandler> _logger;
    private readonly Settings _settings;

    public PickMapAutocompleteHandler(ILogger<PickMapAutocompleteHandler> logger, Settings settings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        if (autocompleteInteraction.Data.Current.Type != ApplicationCommandOptionType.String)
        {
            throw new Exception($"Type mismatch: needed a string but got {autocompleteInteraction.Data.Current.Type}.");
        }

        var currentVal = (string) autocompleteInteraction.Data.Current.Value;

        var matchingCategories = (new[] { PickMapInteractionModule.ArgValueAny }.Concat(
            _settings.Left4DeadSettings.Maps.Categories.Keys))
            .ToList();

        if (!string.IsNullOrEmpty(currentVal))
        {
            matchingCategories = matchingCategories.Where(m => m.Contains(currentVal, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }
        var results = matchingCategories.Select(c => new AutocompleteResult(c, c));

        // max - 25 suggestions at a time (API limit)
       return AutocompletionResult.FromSuccess(results.Take(25));
    }
}
