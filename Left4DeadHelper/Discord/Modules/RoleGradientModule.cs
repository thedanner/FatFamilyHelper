using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Wrappers.DiscordNet;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class RoleGradientModule : ModuleBase<SocketCommandContext>
    {
        private const string Command = "roles";

        public ILogger<PickMapModule> Logger { get; }
        public IDiscordSocketClientWrapper Client { get; }
        public Settings Settings { get; }

        public RoleGradientModule(ILogger<PickMapModule> logger, IDiscordSocketClientWrapper client, Settings settings)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command(Command)]
        [Summary("Sets role colors!")]
        public async Task HandleCommandAsync(params string[] args)
        {
            // TODO figure out how to get guild roles in the same order as the UI shows.
            // TODO run gradient calculation.
            // TODO actually set role colors!
        }
    }
}
