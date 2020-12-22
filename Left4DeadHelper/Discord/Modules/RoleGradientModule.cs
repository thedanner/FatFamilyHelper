using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
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
        public IServiceProvider ServiceProvider { get; }
        public Settings Settings { get; }

        public RoleGradientModule(ILogger<PickMapModule> logger, IServiceProvider serviceProvider, Settings settings)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command(Command)]
        [Summary("Sets role colors!")]
        public async Task HandleCommandAsync(params string[] args)
        {

        }
    }
}
