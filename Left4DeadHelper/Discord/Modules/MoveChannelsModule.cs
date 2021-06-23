using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2, Constants.GroupLfd, Constants.GroupLfd2, Constants.GroupDivorce)]
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
        [Alias(Constants.CommandVoiceChat)]
        [Summary("Moves users into respective voice channels based on game team.")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task HandleVoiceChatAsync()
        {
            if (Context.Message == null) return;
            if (Context.Guild == null) return;

            try
            {
                using var rcon = _serviceProvider.GetRequiredService<IRCONWrapper>();

                await rcon.ConnectAsync();

                var settings = _serviceProvider.GetRequiredService<Settings>();
                var guildSettings = settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == Context.Guild.Id);

                var mover = _serviceProvider.GetRequiredService<IDiscordChatMover>();

                var moveResult = await mover.MovePlayersToCorrectChannelsAsync(
                    rcon,
                    new DiscordSocketClientWrapper(Context.Client),
                    new SocketGuildWrapper(Context.Guild),
                    CancellationToken.None);

                string replyMessage;

                if (moveResult.MoveCount == 0)
                {
                    replyMessage = "Nobody was playing.";
                }
                else if (moveResult.MoveCount == 1)
                {
                    replyMessage = "1 player moved.";
                }
                else
                {
                    replyMessage = $"{moveResult.MoveCount} players moved.";
                }

                if (moveResult.UnmappedSteamUsers.Any())
                {
                    string whoShouldFix;
                    if (guildSettings != null && guildSettings.ConfigMaintainers.Any())
                    {
                        whoShouldFix = string.Join(", ", guildSettings.ConfigMaintainers.Select(m => $"<@{m.DiscordId}>"));
                    }
                    else
                    {
                        whoShouldFix = "you-know-who";
                    }

                    replyMessage +=
                        $"\n\nSorry, I couldn't move these people: {string.Join(", ", moveResult.UnmappedSteamUsers.Select(u => u.Name))} " +
                        $"(missing mappings from the bot config). Bother {whoShouldFix} to fix it.";
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
                _logger.LogError(e, "Got an error trying to move players :(");
            }
        }
    }
}
