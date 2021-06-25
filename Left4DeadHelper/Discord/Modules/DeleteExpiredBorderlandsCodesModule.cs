using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class DeleteExpiredBorderlandsCodesModule : ModuleBase<SocketCommandContext>
    {
        private const int BatchSize = 100;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeleteExpiredBorderlandsCodesModule> _logger;

        public DeleteExpiredBorderlandsCodesModule(ILogger<DeleteExpiredBorderlandsCodesModule> logger, IServiceProvider serviceProvider)
            : base()
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(Constants.CommandPruneExpiredCodes)]
        [Summary("Deletes expired Borderlands codes.")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteExpiredBorderlandsCodesAsync(ulong? channelId = null)
        {
            if (Context.Message == null) return;
            if (Context.Guild == null) return;

            try
            {
                SocketTextChannel channel;

                if (channelId.HasValue)
                {
                    var otherChannel = Context.Guild.GetTextChannel(channelId.Value);

                    if (otherChannel == null)
                    {
                        await ReplyAsync("Couldn't find a channel with that ID.");
                        return;
                    }

                    channel = otherChannel;
                }
                else
                {
                    channel = (SocketTextChannel)Context.Channel;
                }

                var messages = (await Context.Channel.GetMessagesAsync(BatchSize)
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
                        await Task.Delay(250);
                    }

                    foreach (var singleDeletableMessage in singleDeletableMessages)
                    {
                        await channel.DeleteMessageAsync(singleDeletableMessage);
                        await Task.Delay(250);
                    }

                    if (messagesToDelete.Count > 0)
                    {
                        await ReplyAsync($"Deleted {messagesToDelete.Count} message{plural} in \"{channel.Name}\".");
                    }
                    else
                    {
                        await ReplyAsync($"No messages with epxired codes found to delete in \"{channel.Name}\".");
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
