using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Left4DeadHelper.Models
{
    public class HelpContext
    {
        public HelpContext(
            IReadOnlyList<string> triggers,
            string? group, IReadOnlyList<string>? groupAliases, string? groupSummary,
            string? command, IReadOnlyList<string>? commandAliases, string? commandSummary,
            IReadOnlyList<ParameterInfo>? args)
        {
            Triggers = triggers ?? throw new ArgumentNullException(nameof(triggers));
            if (triggers.Count == 0)
            {
                throw new ArgumentException("At least one trigger is required.", nameof(triggers));
            }
            
            Group = group;
            GroupAliases = groupAliases;
            GroupSummary = groupSummary;

            Command = command;
            CommandAliases = commandAliases;
            CommandSummary = commandSummary;

            Args = args;
        }

        public IReadOnlyList<string> Triggers { get; }
        
        public string? Group { get; }
        public IReadOnlyList<string>? GroupAliases { get; }
        public string GetGroupAliasesString(string joiner = ", ", string? aliasPrefix = "`", string? aliasSuffix = "`") =>
            GetAliasesString(GroupAliases, joiner, aliasPrefix, aliasSuffix);
        public string? GroupSummary { get; }

        public string? Command { get; }
        public IReadOnlyList<string>? CommandAliases { get; }
        public string GetCommandAliasesString(string joiner = ", ", string? aliasPrefix = "`", string? aliasSuffix = "`") =>
            GetAliasesString(CommandAliases, joiner, aliasPrefix, aliasSuffix);
        public string? CommandSummary { get; }

        public IReadOnlyList<ParameterInfo>? Args { get; }


        public string CommandShortcutForHelp =>
            new[] { Group, Command }
                .Where(s => !string.IsNullOrEmpty(s))
                .Aggregate((accValue, next) => accValue + "_" + next)
            ?? throw new Exception("Neither Group nor Command were present.");

        public string GenericCommandExample =>
            Triggers[0] + CommandWithoutTrigger;

        public string CommandWithoutTrigger =>
            new[] { Group, Command }
                .Where(s => !string.IsNullOrEmpty(s))
                .Aggregate((accValue, next) => accValue + " " + next)
            ?? throw new Exception("Neither Group nor Command were present.");


        public static string GetAliasesString(IReadOnlyList<string>? aliases, string joiner, string? aliasPrefix, string? aliasSuffix)
        {
            if (string.IsNullOrEmpty(joiner))
            {
                throw new ArgumentException($"'{nameof(joiner)}' cannot be null or empty.", nameof(joiner));
            }

            if (aliases != null)
            {
                return string.Join(joiner, aliases.Select(a => (aliasPrefix ?? "") + a + aliasSuffix ?? ""));
            }

            return "";
        }
    }
}
