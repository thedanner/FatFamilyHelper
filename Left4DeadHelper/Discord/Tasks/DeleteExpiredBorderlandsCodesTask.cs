using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class DeleteExpiredBorderlandsCodesTask : ITask
    {
        private static readonly Regex ExpiresRegex = new Regex(
            @"Expires: (?<expires>(?<date>\d{1,2}) (?<month>[a-zA-Z]{3}) (?<time>(?<hours>\d{1,2}):(?<minutes>\d{2}))) (?<timezone>[a-zA-Z]{3})",
            RegexOptions.Multiline|RegexOptions.Compiled);

        private const int BatchSize = 100;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeleteExpiredBorderlandsCodesTask> _logger;

        public DeleteExpiredBorderlandsCodesTask(ILogger<DeleteExpiredBorderlandsCodesTask> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunTaskAsync(DiscordSocketClient client, IReadOnlyDictionary<string, object> taskSettings,
            CancellationToken cancellationToken)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (taskSettings is null) throw new ArgumentNullException(nameof(taskSettings));

            try
            {
                var guildIdStr = (string) taskSettings["guildId"]
                    ?? throw new Exception($"guildId missing from {nameof(DeleteExpiredBorderlandsCodesTask)} settings.");
                var guildId = ulong.Parse(guildIdStr);
                var guild = client.GetGuild(guildId);

                var channelIdStr = (string) taskSettings["channelId"]
                    ?? throw new Exception($"guildId missing from {nameof(DeleteExpiredBorderlandsCodesTask)} settings.");
                var channelId = ulong.Parse(channelIdStr);
                var channel = guild.GetTextChannel(channelId);

                var messages = (await channel.GetMessagesAsync(BatchSize)
                       .FlattenAsync())
                       .ToList();

                if (messages.Any())
                {
                    var messagesToDelete = GetMessagesWithExpiredCodes(messages, _logger);

                    _logger.LogInformation("Found {count} messages with expired codes to delete.", messagesToDelete.Count);

                    // Only messages < 14 days old can be bulk deleted.
                    var bulkDeletableMessages = new List<IMessage>();
                    var singleDeletableMessages = new List<IMessage>();

                    foreach (var message in messagesToDelete)
                    {
                        var bulkDeleteCutoff = DateTimeOffset.Now.AddMinutes(5).AddDays(-14);

                        if (message.Timestamp >= bulkDeleteCutoff)
                        {
                            bulkDeletableMessages.Add(message);
                        }
                        else
                        {
                            singleDeletableMessages.Add(message);
                        }
                    }

                    if (bulkDeletableMessages.Any())
                    {
                        await channel.DeleteMessagesAsync(bulkDeletableMessages);
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }

                    foreach (var singleDeletableMessage in singleDeletableMessages)
                    {
                        await channel.DeleteMessageAsync(singleDeletableMessage);
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to prune expired messages :(");
            }
        }
        
        public static List<IMessage> GetMessagesWithExpiredCodes(List<IMessage> messages, ILogger logger)
        {
            var messagesToDelete = new List<IMessage>();

            foreach (var message in messages)
            {
                if (message.Author.IsBot
                    && message.Embeds.Any())
                {
                    var embed = message.Embeds.First();
                    if (!string.IsNullOrEmpty(embed.Description))
                    {
                        // Look for:
                        // Expires: 24 JUN 15:00 UTC

                        var match = ExpiresRegex.Match(embed.Description);
                        if (match.Success
                            && DateTimeOffset.TryParseExact(
                                    match.Groups["expires"].Value,
                                    "d MMM H:mm",
                                    CultureInfo.CurrentCulture,
                                    DateTimeStyles.AssumeUniversal,
                                    out var givenExpiry))
                        {
                            if (!"UTC".Equals(match.Groups["timezone"].Value, StringComparison.CurrentCultureIgnoreCase))
                            {
                                logger.LogWarning("Non-UTC expiration found for message with ID {messageId}.", message.Id);
                                continue;
                            }

                            // Handles the case where the assumed year is incorrect.
                            // E.g., a code is posted in December that expires in January. If the year is always
                            // assumed to be current, that code will look invalid immediately.
                            // To fix this, if the expiration is before the message post date, we just add a year.
                            if (givenExpiry < message.Timestamp)
                            {
                                logger.LogInformation(
                                    "Found one of those special cases where we have to add an expiration year in message with ID {messageId}.",
                                    message.Id);
                                givenExpiry = givenExpiry.AddYears(1).AddDays(1);
                            }

                            if (givenExpiry <= DateTimeOffset.Now)
                            {
                                messagesToDelete.Add(message);
                            }
                        }
                    }
                }
            }

            return messagesToDelete;
        }
    }
}
