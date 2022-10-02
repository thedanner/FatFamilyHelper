using Discord;
using Discord.Interactions;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.DiscordExtensions;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules;

public class MoveChannelsInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<MoveChannelsInteractionModule> _logger;
    private readonly Settings _settings;
    private readonly IRCONWrapperFactory _rconFactory;
    private readonly IDiscordChatMover _chatMover;

    public MoveChannelsInteractionModule(ILogger<MoveChannelsInteractionModule> logger,
        Settings settings, IRCONWrapperFactory rconFactory, IDiscordChatMover chatMover) : base()
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _rconFactory = rconFactory ?? throw new ArgumentNullException(nameof(rconFactory));
        _chatMover = chatMover ?? throw new ArgumentNullException(nameof(chatMover));
    }

    [SlashCommand("divorce", "Moves users into respective voice channels based on game team.")]
    [RequireUserPermission(GuildPermission.MoveMembers)]
    public async Task HandleDivorceAsync()
    {
        if (Context.Guild == null) return;

        try
        {
            await DeferAsync();

            using var rcon = _rconFactory.GetRcon();

            await rcon.ConnectAsync();

            var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == Context.Guild.Id);

            var moveResult = await _chatMover.MovePlayersToCorrectChannelsAsync(
                rcon,
                Context.Client,
                Context.Guild,
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
                    whoShouldFix = string.Join(", ", guildSettings.ConfigMaintainers.Select(u =>
                        DiscordMessageExtensions.ToDiscordUserIdMessageRef(u.DiscordId)));
                }
                else
                {
                    whoShouldFix = "someone";
                }

                replyMessage +=
                    $"\n\nSorry, I couldn't move these people because I don't know enough about them: " +
                    $"\n{string.Join(", ", moveResult.UnmappedSteamUsers.Select(u => u.Name))}. Bother {whoShouldFix} to fix it.";
            }

            await FollowupAsync(replyMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Got an error trying to move players :(");
        }
    }

    [SlashCommand("remarry", "Moves users from the configured secondary channel into the primary channel.")]
    [RequireUserPermission(GuildPermission.MoveMembers)]
    public async Task HandleRemarryAsync()
    {
        try
        {
            if (Context.Guild == null) return;

            await DeferAsync();
            await Task.Delay(Constants.DelayAfterCommand);

            var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == Context.Guild.Id);

            var moveResult = await _chatMover.RenuitePlayersAsync(
                Context.Client,
                Context.Guild,
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

            await FollowupAsync(replyMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error trying to reuninte users :(");
        }
    }
}
