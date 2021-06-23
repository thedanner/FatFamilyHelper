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
    public class DeleteExpiredCodesModule : ModuleBase<SocketCommandContext>
    {
        private const int BatchSize = 100;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeleteExpiredCodesModule> _logger;

        public DeleteExpiredCodesModule(ILogger<DeleteExpiredCodesModule> logger, IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(Constants.CommandPruneExpiredCodes)]
        [Summary("Deletes expired codes.")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task HandleVoiceChatAsync()
        {
            try
            {
                if (Context.Message == null) return;
                if (Context.Guild == null) return;

                ISocketMessageChannel simpleChannel = Context.Channel;

                var channel = (SocketGuildChannel)simpleChannel;

                var messages = (await simpleChannel.GetMessagesAsync(BatchSize)
                    .FlattenAsync())
                    .ToList();

                if (messages.Any())
                {
                    ProcessListAsync(messages);

                    if (messages.Count == BatchSize)
                    {
                        var olderMessages = (await simpleChannel.GetMessagesAsync(messages.Last(), Direction.Before, BatchSize)
                            .FlattenAsync())
                            .ToList();

                        ProcessListAsync(olderMessages);
                    }
                }

                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error trying to prune expired messages :(");
            }
        }

        private void ProcessListAsync(List<IMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message.Author.IsBot
                    && message.Embeds.Any())
                {
                    var embed = message.Embeds.First();
                }
            }
        }
    }
}
