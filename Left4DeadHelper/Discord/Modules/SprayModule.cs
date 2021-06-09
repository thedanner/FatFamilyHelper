using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Sprays;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupSprayMe)]
    public class SprayModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<PickMapModule> _logger;
        private readonly Settings _settings;

        public SprayModule(ILogger<PickMapModule> logger, Settings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command]
        [Summary("Converts an image into a Source engine-compatible spray")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task HandleCommandAsync(string? url = null)
        {
            var client = new WebClient();
            var replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild.Id);

            _logger.LogInformation("Triggered by message with ID {0}.", Context.Message.Id);

            Uri? imageUri = null;
            if (url != null && Uri.TryCreate(url, UriKind.Absolute, out imageUri))
            {
                // All good here.
                _logger.LogInformation("Got URL as first token in message.");
            }
            else if (Context.Message.Attachments.Any())
            {
                _logger.LogInformation("Got URL as attachment message.");
                Uri.TryCreate(Context.Message.Attachments.First().Url, UriKind.Absolute, out imageUri);
            }
            else if (Context.Message.ReferencedMessage != null)
            {
                _logger.LogInformation("Checking referenced message.");

                var originalMessage = Context.Message.ReferencedMessage;
                var firstToken = originalMessage.Content?.Trim();

                if (!string.IsNullOrEmpty(firstToken))
                {
                    if (firstToken.Contains(' '))
                    {
                        firstToken = firstToken.Substring(0, firstToken.IndexOf(' '));
                    }

                    if (firstToken != null && Uri.TryCreate(firstToken, UriKind.Absolute, out imageUri))
                    {
                        _logger.LogInformation("Got URL as first token in referenced message.");
                        // All good here.
                    }
                }
                else if (originalMessage.Attachments.Any())
                {
                    _logger.LogInformation("Got URL as first attachment in referenced message.");
                    Uri.TryCreate(originalMessage.Attachments.First().Url, UriKind.Absolute, out imageUri);
                }
            }

            // No source found
            if (imageUri == null) return;

            try
            {
                var tempMessage = await ReplyAsync("Working on it...", messageReference: replyToMessageRef);
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                using var sourceStream = await client.OpenReadTaskAsync(imageUri);

                var sprayTools = new SprayTools();
                var convertedStream = await sprayTools.ConvertAsync(sourceStream, CancellationToken.None);

                var filePart = imageUri.LocalPath;
                if (filePart.Contains('/')) filePart = filePart.Substring(filePart.LastIndexOf('/') + 1);
                filePart = Path.ChangeExtension(filePart, ".tga");

                await Context.Channel.SendFileAsync(convertedStream, filePart, "Here ya go!", messageReference: replyToMessageRef);
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                await tempMessage.DeleteAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Got an error converting image to a spray :(");
                throw;
            }
        }
    }
}
