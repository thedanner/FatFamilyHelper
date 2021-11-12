using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.DiscordExtensions;
using Left4DeadHelper.Support.ExpiredCodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class DeleteExpiredShiftCodesTask : ITask
    {
        private const string SettingsKeyGuildId = "guildId";
        private const string SettingsKeyChannelId = "channelId";
        private const string SettingsKeyReportToChannelId = "reportToChannelId";

        private const int BatchSize = 100;


        private readonly ILogger<DeleteExpiredShiftCodesTask> _logger;

        public DeleteExpiredShiftCodesTask(ILogger<DeleteExpiredShiftCodesTask> logger)
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
                        $"Setting with key \"{nameof(SettingsKeyGuildId)}\" missing from {nameof(DeleteExpiredShiftCodesTask)} settings.");
                var guildId = ulong.Parse(guildIdStr);
                var guild = client.GetGuild(guildId);

                var channelIdStr = (string)taskSettings[SettingsKeyChannelId]
                    ?? throw new Exception(
                        $"Setting with key \"{nameof(SettingsKeyChannelId)}\" missing from {nameof(DeleteExpiredShiftCodesTask)} settings.");
                var channelId = ulong.Parse(channelIdStr);
                var channel = guild.GetTextChannel(channelId);

                taskSettings.TryGetValue(SettingsKeyReportToChannelId, out var reportToChannelIdVal);

                var reportToChannelIdStr = reportToChannelIdVal as string;
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
                    var messagesToDelete = ExpiredCodesHelpers.GetMessagesWithExpiredCodes(messages, _logger);

                    var plural = messagesToDelete.Count == 1 ? "" : "s";
                    _logger.LogInformation("Found {count} message{s} with expired codes to delete.",
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
                        await reportToChannel.SendMessageAsync($"Deleted {messagesToDelete.Count} expired code{plural} in {channel.ToMessageRef()}.");
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to prune expired messages :(");
            }
        }
    }
}
