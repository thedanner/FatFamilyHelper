using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class MoveChannelsModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MoveChannelsModule> _logger;

        public MoveChannelsModule(ILogger<MoveChannelsModule> logger, IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command]
        [Alias("vc")]
        [Summary("Moves users into respective voice channels based on game team.")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task HandleCommandAsync()
        {
            try
            {
                if (Context.Message == null) return;
                if (Context.Guild == null) return;

                using (var rcon = _serviceProvider.GetRequiredService<IRCONWrapper>())
                {
                    await rcon.ConnectAsync();

                    var mover = _serviceProvider.GetRequiredService<IDiscordChatMover>();

                    var moveCount = await mover.MovePlayersToCorrectChannelsAsync(
                        rcon,
                        new DiscordSocketClientWrapper(Context.Client),
                        new SocketGuildWrapper(Context.Guild),
                        CancellationToken.None);

                    string replyMessage;

                    if (moveCount == -1)
                    {
                        replyMessage = "Nobody was playing.";
                    }
                    else if (moveCount == 1)
                    {
                        replyMessage = "1 player moved.";
                    }
                    else
                    {
                        replyMessage = $"{moveCount} players moved.";
                    }

                    await ReplyAsync(replyMessage);

                    return;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in {0}.{1}().", nameof(MoveChannelsModule), nameof(HandleCommandAsync));
            }
        }
    }
}
