using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Models;
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
        private readonly List<ICommandModule> _commandModules;

        public HelpModule(ILogger<HelpModule> logger, Settings settings, IEnumerable<ICommandModule> commandModules)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _commandModules = commandModules?.ToList() ?? throw new ArgumentNullException(nameof(commandModules));
        }

        [Command(Command)]
        [Summary("Shows command help.")]
        public async Task HandleHelpAsync(string? subcommand = "")
        {
            var lines = new List<string>(20);

            var prefixes = _settings.DiscordSettings.Prefixes;
            var prefixesStr = "";
            if (prefixes.Any())
            {
                prefixesStr = $"`{string.Join("`, `", prefixes)}`, and ";
            }

            var triggers = $"{prefixesStr}\"<@{Context.Client.CurrentUser.Id}> \" (a.k.a., tag me)";

            if (string.IsNullOrWhiteSpace(subcommand))
            {
                lines.Add($"Welcome to my help message! I'm <@{Context.Client.CurrentUser.Id}>, as you can probably tell.");
                lines.Add("");
                lines.Add($"I listen to the following triggers (message prefixes):");

                lines.Add(triggers + ".");

                lines.Add("");
                lines.Add("I know about the following commands:");
                lines.Add($"- `help <any of the commands below>`");
                foreach (var cmd in _commandModules)
                {
                    lines.Add($"- `{cmd.CommandString}`: {GetSummary(cmd)}");
                }

                lines.Add("");
                lines.Add("You can run help with any of those listed commands to get help and usage information.");
                lines.Add("");
                lines.Add("Note that actual command usage may vary a bit, due to aliases, shortcuts, etc.");
            }
            else
            {
                var command = _commandModules.FirstOrDefault(c =>
                    subcommand.Equals(c.CommandString, StringComparison.CurrentCultureIgnoreCase));

                if (command != null)
                {
                    lines.Add($"Here's the help for `{command.CommandString}`:");
                    lines.Add("");
                    lines.Add(command.GetGeneralHelpMessage());
                    lines.Add("");
                    lines.Add($"(As a reminder, the triggers are {triggers}.)");
                }
                else
                {
                    lines.Add($"Sorry, I don't know about a command called \"{subcommand}\".");
                }
            }

            await ReplyAsync(string.Join("\n", lines));
        }

        private string GetSummary(ICommandModule cmd)
        {
            if (cmd is ModuleBase<SocketCommandContext>)
            {
                var methodWithSummary = cmd.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(m => m.GetCustomAttribute<SummaryAttribute>() != null);

                if (methodWithSummary != null)
                {
                    var summary = methodWithSummary.GetCustomAttribute<SummaryAttribute>();

                    if (summary != null)
                    {
                        return summary.Text;
                    }
                }
            }

            return "";
        }
    }
}
