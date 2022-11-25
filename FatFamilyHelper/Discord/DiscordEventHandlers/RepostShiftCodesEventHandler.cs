using Discord.WebSocket;
using FatFamilyHelper.Discord.Interfaces.Events;
using FatFamilyHelper.Helpers;
using FatFamilyHelper.Helpers.DiscordExtensions;
using FatFamilyHelper.Models.Configuration;
using FatFamilyHelper.Support.ExpiredCodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.DiscordEventHandlers;

public class RepostShiftCodesEventHandler : IHandleMessageReceivedAsync
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

    public async Task HandleMessageReceivedAsync(SocketMessage message)
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
