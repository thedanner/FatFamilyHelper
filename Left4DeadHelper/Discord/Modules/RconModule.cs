using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Rcon;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class RconModule : ModuleBase<SocketCommandContext>
    {
        private const string Command = "rcon";

        private readonly ILogger<RconModule> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RconModule(ILogger<RconModule> logger, IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(Command)]
        [Summary("Tests rcon connectivity.")]
        [RequireUserPermission(GuildPermission.MoveMembers)] // Same as MoveChannelsModule.
        public async Task HandleCommandAsync(string subcommand)
        {
            try
            {
                var message = Context.Message;
                if (message == null) return;

                if (!"test".Equals(subcommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    await ReplyAsync("Only the 'test' command is currently supported.");
                    return;
                }

                using (var rcon = _serviceProvider.GetRequiredService<IRCONWrapper>())
                {
                    await rcon.ConnectAsync();

                    // "test" option / command.
                    var printInfo = await rcon.SendCommandAsync<PrintInfo>("sm_printinfo");

                    _logger.LogInformation("Got a result back from PrintInfo.");

                    await ReplyAsync($"Test succeeded. Check bot logs for details.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in {0}.{1}().", nameof(RconModule), nameof(HandleCommandAsync));

                await ReplyAsync($"Test FAILED. Check bot logs for details.");
            }
        }
    }
}
