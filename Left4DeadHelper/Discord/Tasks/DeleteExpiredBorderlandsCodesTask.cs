using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
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
        private const string SettingsKeyGuildId = "guildId";
        private const string SettingsKeyChannelId = "channelId";
        private const string SettingsKeyReportToChannelId = "reportToChannelId";

        private const int BatchSize = 100;

        private static readonly Regex ExpiresRegex = new Regex(
            @"Expires: (?<expires>(?<date>(?<dayOfMonth>\d{1,2}) (?<month>[a-zA-Z]{3}) (?<year>\d{4})) (?<time>(?<hours>\d{1,2}):(?<minutes>\d{2}))) (?<timezone>[a-zA-Z]{3})",
            RegexOptions.Multiline|RegexOptions.Compiled);


        private readonly ILogger<DeleteExpiredBorderlandsCodesTask> _logger;

        public DeleteExpiredBorderlandsCodesTask(ILogger<DeleteExpiredBorderlandsCodesTask> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunTaskAsync(DiscordSocketClient client, IReadOnlyDictionary<string, object> taskSettings,
            CancellationToken cancellationToken)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (taskSettings is null) throw new ArgumentNullException(nameof(taskSettings));

            try
            {
                var guildIdStr = (string) taskSettings[SettingsKeyGuildId]
                    ?? throw new Exception(
                        $"Setting with key \"{nameof(SettingsKeyGuildId)}\" missing from {nameof(DeleteExpiredBorderlandsCodesTask)} settings.");
                var guildId = ulong.Parse(guildIdStr);
                var guild = client.GetGuild(guildId);

                var channelIdStr = (string)taskSettings[SettingsKeyChannelId]
                    ?? throw new Exception(
                        $"Setting with key \"{nameof(SettingsKeyChannelId)}\" missing from {nameof(DeleteExpiredBorderlandsCodesTask)} settings.");
                var channelId = ulong.Parse(channelIdStr);
                var channel = guild.GetTextChannel(channelId);

                var reportToChannelIdStr = (string)taskSettings[SettingsKeyReportToChannelId];
                SocketTextChannel? reportToChannel = null;

                if (!string.IsNullOrEmpty(reportToChannelIdStr))
                {
                    var reportToChannelId = ulong.Parse(reportToChannelIdStr);
                    reportToChannel = guild.GetTextChannel(reportToChannelId);
                }

                var messages = (await channel.GetMessagesAsync(BatchSize)
                    .FlattenAsync())
                    .ToList();

                if (messages.Any())
                {
                    var messagesToDelete = GetMessagesWithExpiredCodes(messages, _logger);

                    var plural = messagesToDelete.Count == 1 ? "" : "s";
                    _logger.LogInformation("Found {count} message(s) with expired codes to delete.",
                        messagesToDelete.Count, plural);

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

                    if (messagesToDelete.Count > 0 && reportToChannel != null)
                    {
                        await reportToChannel.SendMessageAsync($"Deleted {messagesToDelete.Count} message{plural} in <#{channel.Id}>.");
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
                var embed = message.Embeds.FirstOrDefault();

                if (message.Author.IsBot
                    && embed != null
                    && !string.IsNullOrEmpty(embed.Description))
                {
                    // Look for:
                    // Expires: 24 JUN 2021 15:00 UTC

                    var match = ExpiresRegex.Match(embed.Description);
                    if (match.Success
                        && DateTimeOffset.TryParseExact(
                            match.Groups["expires"].Value,
                            "d MMM yyyy H:mm",
                            CultureInfo.CurrentCulture,
                            DateTimeStyles.AssumeUniversal,
                            out var givenExpiry))
                    {
                        if (!"UTC".Equals(match.Groups["timezone"].Value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            logger.LogWarning("Non-UTC expiration found for message with ID {messageId}.", message.Id);
                            continue;
                        }

                        if (givenExpiry <= DateTimeOffset.Now)
                        {
                            messagesToDelete.Add(message);
                        }
                    }
                }
            }

            return messagesToDelete;
        }
    }
}
