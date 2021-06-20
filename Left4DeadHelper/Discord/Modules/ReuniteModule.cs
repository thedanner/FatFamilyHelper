using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupReunite)]
    [Alias(Constants.GroupRemarry)]
    public class ReuniteModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReuniteModule> _logger;

        public ReuniteModule(ILogger<ReuniteModule> logger, IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command]
        [Summary("Moves users from the configured secondary channel into the primary channel.")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task HandleVoiceChatAsync()
        {
            try
            {
                if (Context.Message == null) return;
                if (Context.Guild == null) return;

                var settings = _serviceProvider.GetRequiredService<Settings>();
                var guildSettings = settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == Context.Guild.Id);

                var mover = _serviceProvider.GetRequiredService<IDiscordChatMover>();

                var moveResult = await mover.RenuitePlayersAsync(
                    new DiscordSocketClientWrapper(Context.Client),
                    new SocketGuildWrapper(Context.Guild),
                    CancellationToken.None);

                string replyMessage;

                if (moveResult.MoveCount == 0)
                {
                    replyMessage = "Nobody was in the channel to move.";
                }
                else if (moveResult.MoveCount == 1)
                {
                    replyMessage = "1 person moved.";
                }
                else
                {
                    replyMessage = $"{moveResult.MoveCount} people moved.";
                }

                MessageReference? replyToMessageRef = null;
                if (guildSettings != null && guildSettings.ReferenceCommandsInReplies)
                {
                    replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild.Id);
                }

                await ReplyAsync(replyMessage, messageReference: replyToMessageRef);

                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to reuninte users :(");
            }
        }
    }
}
