using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.EventInterfaces;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.DiscordEventHandlers
{
    [Group(Constants.GroupSprayMe)]
    public class SprayEventHandlers : IHandleReactionAddedAsync
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<SprayEventHandlers> _logger;
        private readonly Settings _settings;

        public SprayEventHandlers(DiscordSocketClient client, ILogger<SprayEventHandlers> logger, Settings settings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel simpleChannel, SocketReaction reaction)
        {
            IUser? reactingUser;
            if (reaction.User.IsSpecified)
            {
                reactingUser = reaction.User.Value;
            }
            else
            {
                reactingUser = await _client.Rest.GetUserAsync(reaction.UserId);
                await Task.Delay(500);
            }

            var removeEmote = await TryHandleDeleteReactionAsync(reactingUser, simpleChannel, reaction);

            if (removeEmote)
            {
                var reactedMessage = await cachedMessage.GetOrDownloadAsync();
                await Task.Delay(500);

                await reactedMessage.RemoveReactionAsync(reaction.Emote, reactingUser);
                await Task.Delay(500);
            }
        }

        private async Task<bool> TryHandleDeleteReactionAsync(IUser? reactingUser, ISocketMessageChannel simpleChannel, SocketReaction reaction)
        {
            var removeEmote = true;

            if (!SprayModule.DeleteEmote.Equals(reaction.Emote)) return removeEmote;

            if (reactingUser == null) return removeEmote;
            if (reactingUser.Id == _client.CurrentUser.Id)
            {
                removeEmote = false;
                return removeEmote;
            }

            var channel = (SocketGuildChannel)simpleChannel;

            var message = await simpleChannel.GetMessageAsync(reaction.MessageId);
            await Task.Delay(500);

            if (message.Author.Id != _client.CurrentUser.Id) return removeEmote;

            // Make sure we have a reference. Don't allow cross-posts.
            if (message.Reference == null
                // And that it's to a message
                || !message.Reference.MessageId.IsSpecified
                // in the same guild
                || !message.Reference.GuildId.IsSpecified
                || message.Reference.GuildId.Value != channel.Guild.Id
                // and same channel
                || message.Reference.ChannelId != message.Channel.Id)
            {
                return removeEmote;
            }

            var referencedIMessage = await simpleChannel.GetMessageAsync(message.Reference.MessageId.Value);
            await Task.Delay(500);

            if (!(referencedIMessage is IUserMessage referencedMessage)) return removeEmote;

            var argPos = 0;
            var referencedMessageIsBotCommand = !(
                !(_settings.DiscordSettings.Prefixes.Any(p => referencedMessage.HasCharPrefix(p, ref argPos)) ||
                    referencedMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                || referencedMessage.Author.IsBot);

            if (reactingUser.Id == referencedMessage.Author.Id
                && referencedMessageIsBotCommand)
            {
                await message.DeleteAsync();
            }

            removeEmote = false;
            return removeEmote;
        }
    }
}
