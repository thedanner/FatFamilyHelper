using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.DiscordEventHandlers
{
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

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel simpleChannel,
            SocketReaction reaction)
        {
            IUserMessage reactedMessage;
            if (cachedMessage.HasValue)
            {
                reactedMessage = cachedMessage.Value;
            }
            else
            {
                reactedMessage = await cachedMessage.DownloadAsync();
            }

            if (reactedMessage == null)
            {
                throw new Exception($"Couldn't get the reacted-to message with ID {cachedMessage.Id}.");
            }

            // I broke sesh RSVPs by missing this check :(
            // Only consider changing the reaction if it's on a message from this bot.
            if (reactedMessage.Author.Id != _client.CurrentUser.Id) return;

            // Skip reactions made by this bot itself (notably the one it immediately adds to indicate which emote will do the delete).
            if (reaction.UserId == _client.CurrentUser.Id) return;

            IUser? reactingUser;
            if (reaction.User.IsSpecified)
            {
                reactingUser = reaction.User.Value;
            }
            else
            {
                reactingUser = await _client.Rest.GetUserAsync(reaction.UserId);
            }

            if (reactingUser == null)
            {
                throw new Exception($"Couldn't get the reacting user with ID {reaction.UserId}.");
            }

            var removeEmote = await TryHandleDeleteReactionAsync(reactingUser, simpleChannel, reaction);

            if (removeEmote)
            {
                await reactedMessage.RemoveReactionAsync(reaction.Emote, reactingUser);
                await Task.Delay(Constants.DelayAfterCommandMs);
            }
        }

        private async Task<bool> TryHandleDeleteReactionAsync(IUser reactingUser, ISocketMessageChannel simpleChannel, SocketReaction reaction)
        {
            if (reactingUser == null) throw new ArgumentNullException(nameof(reactingUser));

            if (!SprayModule.DeleteEmote.Equals(reaction.Emote)) return true;

            var channel = (SocketGuildChannel)simpleChannel;

            var message = await simpleChannel.GetMessageAsync(reaction.MessageId);

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
                return true;
            }

            var referencedIMessage = await simpleChannel.GetMessageAsync(message.Reference.MessageId.Value);

            // If the referenced message was deleted (referencedIMessage is null), let anyone remove the conversion
            // message. This will allow users to delete their own requested conversions if they accidentally delete
            // the original message first.
            // I don't really care if anyone else deletes these either.

            if (referencedIMessage == null)
            {
                await message.DeleteAsync();
                await Task.Delay(Constants.DelayAfterCommandMs);
                return false;
            }

            if (!(referencedIMessage is IUserMessage referencedMessage))
            {
                return true;
            }

            var argPos = 0;
            var referencedMessageIsBotCommand = !(
                !(_settings.DiscordSettings.Prefixes.Any(p => referencedMessage.HasCharPrefix(p, ref argPos)) ||
                    referencedMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                || referencedMessage.Author.IsBot);

            if (reactingUser.Id == referencedMessage.Author.Id
                && referencedMessageIsBotCommand)
            {
                await message.DeleteAsync();
                await Task.Delay(Constants.DelayAfterCommandMs);
            }

            return false;
        }
    }
}
