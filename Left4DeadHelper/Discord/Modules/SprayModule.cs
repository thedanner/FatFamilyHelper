using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Exceptions;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Sprays;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly ILogger<SprayModule> _logger;
        private readonly Settings _settings;

        private const string DeleteEmojiString = "\u274C";
        public static Emoji DeleteEmote => new Emoji(DeleteEmojiString);

        public SprayModule(ILogger<SprayModule> logger, Settings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        // CROSS MARK emoji https://www.fileformat.info/info/emoji/x/index.htm

        [Command]
        [Summary("Converts an image into a Source engine-compatible spray")]
        public async Task HandleCommandAsync(string? url = null)
        {
            var client = new WebClient();
            var replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild.Id);

            _logger.LogInformation("Triggered by message with ID {0}.", Context.Message.Id);

            Uri? imageUri = null;
            var imageUris = new List<Uri>();

            if (url != null
                && Uri.TryCreate(url, UriKind.Absolute, out imageUri)
                && imageUri != null)
            {
                imageUris.Add(imageUri);
                _logger.LogInformation("Got URL as first token in message.");
            }
            
            if (!imageUris.Any() && Context.Message.Attachments.Any())
            {
                foreach (var attachement in Context.Message.Attachments)
                {
                    if (Uri.TryCreate(attachement.Url, UriKind.Absolute, out imageUri)
                        && imageUri != null)
                    {
                        imageUris.Add(imageUri);
                    }
                }

                
                _logger.LogInformation("Got {count} URL(s) as attachment message.", imageUris.Count);
            }

            if (!imageUris.Any() && Context.Message.ReferencedMessage != null)
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
                        imageUris.Add(imageUri);
                        _logger.LogInformation("Got URL as first token in referenced message.");
                    }
                }

                if (imageUri == null && originalMessage.Attachments.Any())
                {
                    foreach (var attachement in originalMessage.Attachments)
                    {
                        if (Uri.TryCreate(attachement.Url, UriKind.Absolute, out imageUri)
                            && imageUri != null)
                        {
                            imageUris.Add(imageUri);
                        }
                    }

                    _logger.LogInformation("Got {count} URL(s) as first attachment in referenced message.", imageUris.Count);
                }
            }

            // No source found
            if (imageUri == null)
            {
                _logger.LogInformation("Couldn't resolve an image URI; nothing to convert.");
                return;
            }

            try
            {
                var tempMessage = await ReplyAsync("Working on it...", messageReference: replyToMessageRef);
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                using var sourceStream = await client.OpenReadTaskAsync(imageUri);

                var sprayTools = new SprayTools();

                foreach (var sourceImageUri in imageUris)
                {
                    try
                    {
                        var convertedStream = await sprayTools.ConvertAsync(sourceStream, CancellationToken.None);

                        var fileName = sourceImageUri.LocalPath;

                        if (fileName.Contains('/'))
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                            if (string.IsNullOrEmpty(fileName))
                            {
                                fileName = $"spray.tga";
                            }
                            else
                            {
                                fileName = Path.ChangeExtension(fileName, ".tga");
                            }
                        }
                        else if (string.IsNullOrEmpty(fileName))
                        {
                            fileName = $"spray.tga";
                        }

                        var sprayMessage = await Context.Channel.SendFileAsync(
                            convertedStream, fileName,
                            $"Here ya go!\n\n(If you requested the conversion, react with {DeleteEmojiString} to delete this message.)",
                            messageReference: replyToMessageRef);
                        await Task.Delay(TimeSpan.FromMilliseconds(500));

                        await sprayMessage.AddReactionAsync(DeleteEmote);
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                    catch (UnsupportedImageFormatException)
                    {
                        continue;
                    }
                }

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
