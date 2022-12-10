using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using FatFamilyHelper.Discord.Interfaces.Events;
using FatFamilyHelper.Helpers;
using FatFamilyHelper.Helpers.DiscordExtensions;
using FatFamilyHelper.Models.Configuration;
using FatFamilyHelper.Support.ExpiredCodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.DiscordEventHandlers;

public class RepostShiftCodesEventHandler : InteractionModuleBase<SocketInteractionContext>, IHandleMessageReceivedAsync
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<RepostShiftCodesEventHandler> _logger;
    private readonly ShiftCodeSettings? _shiftCodesSettings;

    public RepostShiftCodesEventHandler(DiscordSocketClient client, ILogger<RepostShiftCodesEventHandler> logger,
        IOptions<ShiftCodeSettings>? shiftCodesSettings)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _shiftCodesSettings = shiftCodesSettings?.Value;
    }

    [SlashCommand("process", "Reprocesses posted codes, in case I messed up :(")]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    public async Task HandleReprocessCommandAsync()
    {
        if (_shiftCodesSettings is not null
            && _shiftCodesSettings.IsRepostEnabled
            && _shiftCodesSettings.SourceChannelId.GetValueOrDefault() == Context.Channel.Id)
        {
            await DeferAsync(ephemeral: true);

            var pastMessagePages = await Context.Channel.GetMessagesAsync(10).ToListAsync();

            foreach (var pastMessagePage in pastMessagePages)
            {
                var pastMessagesOnPage = pastMessagePage.ToList();

                foreach (var pastMessageOnPage in pastMessagesOnPage)
                {
                    if (pastMessageOnPage is RestMessage pastMessage
                        && pastMessage.Content != "test")
                    {
                        await HandleMessageImplAsync(pastMessage);
                    }
                }
            }

            await FollowupAsync("Codes shold have been reposted now.", ephemeral: true);
        }
        else
        {
            await RespondAsync("This only works in a very particular scenario.", ephemeral: true);
        }
    }

    public async Task HandleMessageReceivedAsync(SocketMessage message)
    {
        await HandleMessageImplAsync(message);
    }

    private async Task HandleMessageImplAsync(IMessage message)
    {
        if (_shiftCodesSettings is not null
            && _shiftCodesSettings.IsRepostEnabled
            && _shiftCodesSettings.SourceChannelId.GetValueOrDefault() == message.Channel.Id
            && message.Author.IsBot
            && _shiftCodesSettings.SourceUserId == message.Author.Id)
        {
            var guild = (message.Channel as SocketGuildChannel)?.Guild;
            if (guild is null)
            {
                throw new Exception("Can't get the Guild from the message's channel.");
            }

            var destinationChannel = guild.GetTextChannel(_shiftCodesSettings.RepostChannelId.GetValueOrDefault());
            if (destinationChannel is null)
            {
                _logger.LogError(
                    "Can't find a channel with ID {repostChannelId} to repost the SHiFT code message. Check the ID in the config.",
                    _shiftCodesSettings.RepostChannelId.GetValueOrDefault());
                return;
            }

            var hasExpiry = ExpiredCodesHelpers.TryGetExpirationDateFromMessage(message, _logger, out var embed, out var expiry);

            var content = message.Content;

            if (hasExpiry)
            {
                content += 
                    $"\nThis code expires {expiry.ToDiscordMessageTs(TimestampFormat.LongDateTime)} " +
                    $"({expiry.ToDiscordMessageTs(TimestampFormat.RelativeTime)}).";
            }
            
            await destinationChannel.SendMessageAsync(content, embed: embed);
            
            if (_shiftCodesSettings.DeleteMessageInSourceChannelAfterRepost.GetValueOrDefault())
            {
                await Task.Delay(Constants.DelayAfterCommand);
                await message.DeleteAsync();
            }
        }
    }
}
