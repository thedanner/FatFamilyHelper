using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class RoleGradientModule : ModuleBase<SocketCommandContext>
    {
        private const string Command = "roles";

        public ILogger<PickMapModule> Logger { get; }
        public Settings Settings { get; }

        public RoleGradientModule(ILogger<PickMapModule> logger, Settings settings)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command(Command)]
        [Summary("Sets role colors!")]
        public async Task HandleCommandAsync()
        {
            var guild = Context.Client.GetGuild(Settings.DiscordSettings.GuildId);

            if (guild == null)
            {
                throw new Exception("Could not get guild.");
            }
            
            var allRolesSorted = new List<SocketRole>(guild.Roles.OrderByDescending(r => r.Position));

            // TODO run gradient calculation.
            // TODO actually set role colors!
        }
    }
}
