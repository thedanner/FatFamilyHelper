using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.DiscordExtensions;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Support.ExpiredCodes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.DiscordEventHandlers;

public class RepostShiftCodesEventHandler : IHandleMessageReceivedAsync
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<RepostShiftCodesEventHandler> _logger;
    private readonly Settings _settings;

    public RepostShiftCodesEventHandler(DiscordSocketClient client, ILogger<RepostShiftCodesEventHandler> logger,
        Settings settings)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task HandleMessageReceivedAsync(SocketMessage message)
    {
        var shiftCodesSettings = _settings.ShiftCodes;

        if (shiftCodesSettings != null
            && shiftCodesSettings.IsRepostEnabled
            && shiftCodesSettings.SourceChannelId.GetValueOrDefault() == message.Channel.Id
            && message.Author.IsBot
            && shiftCodesSettings.SourceUserId == message.Author.Id)
        {
            var guild = (message.Channel as SocketGuildChannel)?.Guild;
            if (guild == null)
            {
                throw new Exception("Can't get the Guild from the message's channel.");
            }

            var destinationChannel = guild.GetTextChannel(_settings.ShiftCodes.RepostChannelId.GetValueOrDefault());
            if (destinationChannel == null)
            {
                _logger.LogError(
                    "Can't find a channel with ID {repostChannelId} to repost the SHiFT code message. Check the ID in the config.",
                    _settings.ShiftCodes.RepostChannelId.GetValueOrDefault());
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
            
            if (_settings.ShiftCodes.DeleteMessageInSourceChannelAfterRepost.GetValueOrDefault())
            {
                await Task.Delay(Constants.DelayAfterCommand);
                await message.DeleteAsync();
            }
        }
    }
}
