using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class DeleteExpiredBorderlandsCodesModule : ModuleBase<SocketCommandContext>, ICommandModule
    {
        private const string Command = "prune";


        private const int BatchSize = 100;

        private static readonly Regex ChannelIdRefRegex = new Regex(@"<#(?<id>\d+)>", RegexOptions.Compiled);

        private readonly ILogger<DeleteExpiredBorderlandsCodesModule> _logger;

        public DeleteExpiredBorderlandsCodesModule(ILogger<DeleteExpiredBorderlandsCodesModule> logger)
            : base()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(Command)]
        [Summary("Deletes expired Borderlands codes.")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteExpiredBorderlandsCodesAsync(string channelToken)
        {
            if (Context.Message == null) return;
            if (Context.Guild == null) return;

            ulong? channelId;
            if (ulong.TryParse(channelToken, out var channelIdValue))
            {
                channelId = channelIdValue;
            }
            else if (!string.IsNullOrEmpty(channelToken))
            {
                var matches = ChannelIdRefRegex.Match(channelToken);
                if (matches.Success)
                {
                    if (ulong.TryParse(matches.Groups["id"].Value, out channelIdValue))
                    {
                        channelId = channelIdValue;
                    }
                    else
                    {
                        // Meh, shorthand for invlid ref.
                        channelId = 0;
                    }
                }
                else
                {
                    // Meh, shorthand for invlid ref.
                    channelId = 0;
                }
            }
            else
            {
                // No channel specified, use the current one.
                channelId = null;
            }

            try
            {
                SocketTextChannel channel;

                if (channelId.HasValue)
                {
                    var otherChannel = Context.Guild.GetTextChannel(channelId.Value);

                    if (otherChannel == null)
                    {
                        await ReplyAsync("Couldn't find a channel with that ID or name.");
                        return;
                    }

                    channel = otherChannel;
                }
                else
                {
                    channel = (SocketTextChannel)Context.Channel;
                }

                var messages = (await channel.GetMessagesAsync(BatchSize)
                    .FlattenAsync())
                    .ToList();

                if (messages.Any())
                {
                    var messagesToDelete = DeleteExpiredBorderlandsCodesTask.GetMessagesWithExpiredCodes(messages, _logger);

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

                    if (messagesToDelete.Count > 0)
                    {
                        await ReplyAsync($"Deleted {messagesToDelete.Count} expired code{plural} in <#{channel.Id}>.");
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
                    else
                    {
                        await ReplyAsync($"No messages with epxired codes found to delete in <#{channel.Id}>.");
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to prune expired messages :(");
            }
        }

        public string GetGeneralHelpMessage(HelpContext helpContext) =>
            $"  - `{helpContext.GenericCommandExample} <@channelReference|channelId>`:\n" +
            $"    Scans the given channel (or the channel the command was run in) for messages posted by bots with\n" +
            $"    an embed with text \"Expires:\" and a timestamp. Those messages are deleted if that timestamp is in the past.\n" +
            $"    To work, the bot must have the Manage Messages permission for the channel.";
    }
}
