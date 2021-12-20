using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models.Configuration;
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

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> maybeCachedMessage,
            Cacheable<IMessageChannel, ulong> maybeCachedChannel, SocketReaction reaction)
        {
            var reactedMessage = await maybeCachedMessage.GetOrDownloadAsync();
            var messageChannel = await maybeCachedChannel.GetOrDownloadAsync();

            IMessage simpleMessage = reactedMessage;
            if (simpleMessage == null && messageChannel is SocketTextChannel textChannel)
            {
                simpleMessage = await textChannel.GetMessageAsync(maybeCachedMessage.Id);
            }

            if (simpleMessage == null)
            {
                throw new Exception($"Couldn't get the reacted-to message with ID {maybeCachedMessage.Id}.");
            }

            // I broke sesh RSVPs by missing this check :(
            // Only consider changing the reaction if it's on a message from this bot.
            if (simpleMessage.Author.Id != _client.CurrentUser.Id) return;

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

            var result = await TryHandleDeleteReactionAsync(simpleMessage, reactingUser, messageChannel, reaction);

            if (result.ShouldReactionBeRemoved)
            {
                await simpleMessage.RemoveReactionAsync(reaction.Emote, reactingUser);
                await Task.Delay(Constants.DelayAfterCommand);
            }
        }

        private async Task<TryHandleDeleteReactionResult> TryHandleDeleteReactionAsync(IMessage simpleMessage, IUser reactingUser,
            IMessageChannel simpleChannel, SocketReaction reaction)
        {
            if (reactingUser == null) throw new ArgumentNullException(nameof(reactingUser));

            if (!SprayModule.DeleteEmote.Equals(reaction.Emote))
            {
                return new TryHandleDeleteReactionResult(false, true);
            }

            // Support SocketGuildChannel and SocketDMChannel.
            var channel = (SocketChannel)simpleChannel;
            var guildChannel = simpleChannel as SocketGuildChannel;

            if (simpleMessage.Interaction != null
                && simpleMessage.Interaction.User.Id == reactingUser.Id)
            {
                await simpleMessage.DeleteAsync();
                await Task.Delay(Constants.DelayAfterCommand);

                return new TryHandleDeleteReactionResult(true, false);
            }
            
            // Make sure we have a reference. Don't allow cross-posts.
            if (simpleMessage.Reference != null
                // And that it's to a message
                && simpleMessage.Reference.MessageId.IsSpecified
                // in the same guild (or there is no guild, so it's a DM)
                && (
                    guildChannel == null
                    || (
                        simpleMessage.Reference.GuildId.IsSpecified
                        && simpleMessage.Reference.GuildId.Value == guildChannel.Guild.Id
                    )
                )
                // and same channel
                && simpleMessage.Channel != null && simpleMessage.Reference.ChannelId == simpleMessage.Channel.Id)
            {
                var referencedIMessage = await simpleChannel.GetMessageAsync(simpleMessage.Reference.MessageId.Value);

                // If the referenced message was deleted (referencedIMessage is null), let anyone remove the conversion
                // message. This will allow users to delete their own requested conversions if they accidentally delete
                // the original message first.
                // I don't really care if anyone else deletes these either.

                if (referencedIMessage == null)
                {
                    await simpleMessage.DeleteAsync();
                    await Task.Delay(Constants.DelayAfterCommand);

                    return new TryHandleDeleteReactionResult(true, false);
                }

                if (!(referencedIMessage is IUserMessage referencedMessage))
                {
                    return new TryHandleDeleteReactionResult(true, false);
                }

                var argPos = 0;
                var referencedMessageIsBotCommand =
                    guildChannel == null // In a DM, everything is assumed to be a bot command.
                    || !(
                        !(_settings.DiscordSettings.Prefixes.Any(p => referencedMessage.HasCharPrefix(p, ref argPos)) ||
                            referencedMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                        || referencedMessage.Author.IsBot
                    );

                if (reactingUser.Id == referencedMessage.Author.Id
                    && referencedMessageIsBotCommand)
                {
                    await simpleMessage.DeleteAsync();
                    await Task.Delay(Constants.DelayAfterCommand);

                    return new TryHandleDeleteReactionResult(true, false);
                }

                return new TryHandleDeleteReactionResult(false, false);
            }

            // Not an interaction, message was a response to someone else's spray request, or we couldn't figure out what it was a response to.
            return new TryHandleDeleteReactionResult(false, true);
        }
    }

    internal record TryHandleDeleteReactionResult(bool WasMessageDeleted, bool ShouldReactionBeRemoved);
}
