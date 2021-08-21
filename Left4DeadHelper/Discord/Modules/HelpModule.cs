using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private const string Command = "help";

        private readonly ILogger<HelpModule> _logger;
        private readonly Settings _settings;
        private readonly IReadOnlyList<ICommandModule> _commandModules;

        private Dictionary<ICommandModule, IReadOnlyList<HelpContext>>? _commandModuleHelpContexts;

        public HelpModule(ILogger<HelpModule> logger, Settings settings, IEnumerable<ICommandModule> commandModules)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _commandModules = commandModules?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(commandModules));
        }

        [Command(Command)]
        [Summary("Shows command help.")]
        public async Task HandleHelpAsync(string? subcommand = "")
        {
            var lines = new List<string>(50);

            var prefixes = _settings.DiscordSettings.Prefixes;
            var prefixesStr = "";
            if (prefixes.Any())
            {
                prefixesStr = $"`{string.Join("`, `", prefixes)}`, and ";
            }

            var triggers = $"{prefixesStr}\"<@{Context.Client.CurrentUser.Id}> \"";

            if (string.IsNullOrWhiteSpace(subcommand))
            {
                lines.Add($"Welcome to my help message! I'm <@{Context.Client.CurrentUser.Id}>.");
                lines.Add("");
                lines.Add($"I listen to the following triggers (message prefixes):");

                lines.Add(triggers + ". That last one is tagging me.");

                lines.Add("");
                lines.Add("I know about the following commands:");
                lines.Add($"- `help <any of the commands below>`");
                
                foreach (var commandModule in CommandModuleHelpContexts)
                {
                    foreach (var helpContext in commandModule.Value)
                    {
                        lines.Add($"- `<trigger>{helpContext.CommandShortcutForHelp}`: {helpContext.CommandSummary ?? helpContext.GroupSummary}");
                    }
                }

                lines.Add("");
                lines.Add("You can run help with any of those listed commands to get help and usage information.");
                lines.Add("");
                lines.Add("Actual command usage may vary a bit, due to aliases, shortcuts, etc.");
            }
            else
            {
                ICommandModule? command = null;
                HelpContext? helpContextForCommand = null;

                foreach(var commandModule in CommandModuleHelpContexts)
                {
                    foreach (var helpContext in commandModule.Value)
                    {
                        if (subcommand.Equals(helpContext.CommandShortcutForHelp, StringComparison.CurrentCultureIgnoreCase))
                        {
                            command = commandModule.Key;
                            helpContextForCommand = helpContext;
                            break;
                        }
                    }
                }

                if (command != null && helpContextForCommand != null)
                {
                    lines.Add($"Here's the help for `{helpContextForCommand.GenericCommandExample}`:");
                    lines.Add("");
                    lines.Add("Usage:");
                    lines.Add(command.GetGeneralHelpMessage(helpContextForCommand));
                    lines.Add("");
                    lines.Add($"The triggers are {triggers}.");
                }
                else
                {
                    lines.Add($"Sorry, I don't know about a command called \"{subcommand}\".");
                }
            }

            await ReplyAsync(string.Join("\n", lines));
        }

        private Dictionary<ICommandModule, IReadOnlyList<HelpContext>> CommandModuleHelpContexts
        {
            get
            {
                if (_commandModuleHelpContexts == null)
                {
                    _commandModuleHelpContexts = new Dictionary<ICommandModule, IReadOnlyList<HelpContext>>(_commandModules.Count);

                    var prefixes = new List<string>(_settings.DiscordSettings.Prefixes.Select(p => p.ToString()))
                    {
                        "<@{Context.Client.CurrentUser.Id}> " // tag the bot
                    };

                    foreach (var commandModule in _commandModules)
                    {
                        var moduleHelpContexts = BuildHelpContextFromCommandModule(commandModule, prefixes)
                            .ToList()
                            .AsReadOnly();

                        if (moduleHelpContexts.Any())
                        {
                            _commandModuleHelpContexts[commandModule] = moduleHelpContexts;
                        }
                    }
                }

                return _commandModuleHelpContexts;
            }
        }

        private IEnumerable<HelpContext> BuildHelpContextFromCommandModule(ICommandModule commandModule, List<string> triggers)
        {
            if (commandModule is null)
            {
                throw new ArgumentNullException(nameof(commandModule));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            if (triggers.Count == 0)
            {
                throw new ArgumentException("At least one prefix is required.", nameof(triggers));
            }

            if (commandModule is ModuleBase<SocketCommandContext>)
            {
                var moduleType = commandModule.GetType();

                var group = moduleType.GetCustomAttribute<GroupAttribute>()?.Prefix;
                var groupAliases = moduleType.GetCustomAttribute<AliasAttribute>()?.Aliases;
                var groupSummary = moduleType.GetCustomAttribute<SummaryAttribute>()?.Text;

                var methods = moduleType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var commandAttribute = method.GetCustomAttribute<CommandAttribute>();

                    if (commandAttribute != null)
                    {
                        var command = commandAttribute?.Text;

                        var commandAliases = method.GetCustomAttribute<AliasAttribute>()?.Aliases;
                        var commandSummary = method.GetCustomAttribute<SummaryAttribute>()?.Text;

                        yield return new HelpContext(triggers,
                            group, groupAliases?.ToList().AsReadOnly(), groupSummary,
                            command, commandAliases?.ToList().AsReadOnly(), commandSummary,
                            method.GetParameters().ToList().AsReadOnly());
                    }
                }
            }
        }
    }
}
