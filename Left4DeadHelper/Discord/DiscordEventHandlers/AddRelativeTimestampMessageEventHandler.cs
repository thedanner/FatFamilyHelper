using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.DiscordExtensions;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Support.ExpiredCodes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.DiscordEventHandlers
{
    public class AddRelativeTimestampMessageEventHandler : IHandleMessageReceivedAsync
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<AddRelativeTimestampMessageEventHandler> _logger;
        private readonly Settings _settings;

        public AddRelativeTimestampMessageEventHandler(DiscordSocketClient client, ILogger<AddRelativeTimestampMessageEventHandler> logger,
            Settings settings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot
                && ExpiredCodesHelpers.TryGetExpirationDateFromMessage(message, _logger, out var expiry))
            {
                var guild = (message.Channel as SocketGuildChannel)?.Guild;
                var replyToMessageRef = new MessageReference(message.Id, message.Channel.Id, guild?.Id);

                await message.Channel.SendMessageAsync(
                    "Expires " + expiry.ToMessageTs(TimestampFormat.RelativeTime) + ".",
                    messageReference: replyToMessageRef);
                await Task.Delay(Constants.DelayAfterCommandMs);
            }
        }
    }
}
