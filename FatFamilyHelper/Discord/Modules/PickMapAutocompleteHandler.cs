using System;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Interactions;
using FatFamilyHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Collections.Generic;

namespace FatFamilyHelper.Discord.Modules;

public class PickMapAutocompleteHandler : AutocompleteHandler
{
    private readonly ILogger<PickMapAutocompleteHandler> _logger;
    private readonly Left4DeadSettings? _left4DeadSettings;

    public PickMapAutocompleteHandler(ILogger<PickMapAutocompleteHandler> logger, IOptions<Left4DeadSettings>? left4DeadSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _left4DeadSettings = left4DeadSettings?.Value;
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

        var keys = _left4DeadSettings?.Maps.Categories.Keys.ToList();
        if (keys is null) keys = new List<string>(0);

        var matchingCategories = (new[] { PickMapInteractionModule.ArgValueAny })
            .Concat(keys)
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
