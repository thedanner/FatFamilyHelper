using Discord;
using Discord.Commands;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupSprayMe)]
    public class SprayModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<SprayModule> _logger;
        private readonly Settings _settings;
        private readonly ISprayModuleCommandResolver _resolver;

        private const string DeleteEmojiString = "\u274C";
        public static Emoji DeleteEmote => new Emoji(DeleteEmojiString);

        public SprayModule(ILogger<SprayModule> logger, Settings settings, ISprayModuleCommandResolver resolver)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        // CROSS MARK emoji https://www.fileformat.info/info/emoji/x/index.htm

        [Command]
        [Summary("Converts an image into a Source engine-compatible spray")]
        public async Task HandleCommandAsync(string? arg1 = null, string? arg2 = null)
        {
            var client = new WebClient();
            var replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild.Id);

            _logger.LogInformation("Triggered by message with ID {0}.", Context.Message.Id);

            // Accepted values: "filename url ..."
            // Accepted values: "url ..."

            // If the message is a reply:
            // Accepted values: "filename ..."
            // Accepted values: ""

            var result = _resolver.Resolve(arg1, arg2, Context.Message);

            // No source found
            if (result == null)
            {
                _logger.LogInformation("Couldn't resolve an image; nothing to convert.");
                return;
            }

            try
            {
                var tempMessage = await ReplyAsync("Working on it...", messageReference: replyToMessageRef);
                await Task.Delay(TimeSpan.FromMilliseconds(250));

                using var sourceStream = await client.OpenReadTaskAsync(result.SourceImageUri);

                var sprayTools = new SprayTools();

                try
                {
                    var conversionResult = await sprayTools.ConvertAsync(sourceStream, CancellationToken.None);

                    string fileName;

                    if (!string.IsNullOrEmpty(result.FileName))
                    {
                        fileName = result.FileName;
                    }
                    else
                    {
                        fileName = result.SourceImageUri.LocalPath;

                        if (fileName.Contains('/'))
                        {
                            fileName = fileName[(fileName.LastIndexOf('/') + 1)..];
                            if (string.IsNullOrEmpty(fileName)
                                || string.Equals(fileName, conversionResult.FileExtension))
                            {
                                fileName = $"spray{conversionResult.FileExtension}";
                            }
                            else
                            {
                                fileName = Path.ChangeExtension(fileName, conversionResult.FileExtension);
                            }
                        }

                        if (string.IsNullOrEmpty(fileName))
                        {
                            fileName = $"spray{conversionResult.FileExtension}";
                        }
                    }

                    fileName = StringHelpers.SanitizeFileNameForDiscordAttachment(fileName);

                    if (!fileName.EndsWith(conversionResult.FileExtension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        fileName += conversionResult.FileExtension;
                    }

                    if (string.Equals(fileName, conversionResult.FileExtension))
                    {
                        fileName = "spray" + conversionResult.FileExtension;
                    }    

                    var sprayMessage = await Context.Channel.SendFileAsync(
                        conversionResult.Stream, fileName,
                        $"Here ya go!\n\n(If you requested the conversion, react with {DeleteEmojiString} to delete this message.)",
                        messageReference: replyToMessageRef);
                    await Task.Delay(TimeSpan.FromMilliseconds(250));

                    await sprayMessage.AddReactionAsync(DeleteEmote);
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
                catch (UnsupportedImageFormatException e)
                {
                    _logger.LogError(e, "Got an image format I don't support.");
                }

                await tempMessage.DeleteAsync();
                await Task.Delay(250);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Got an error converting image to a spray :(");
                throw;
            }
        }
    }
}
