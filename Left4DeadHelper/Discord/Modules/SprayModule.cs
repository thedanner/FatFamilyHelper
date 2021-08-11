using Discord;
using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.Exceptions;
using Left4DeadHelper.Sprays.SaveProfiles;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class SprayModule : ModuleBase<SocketCommandContext>, ICommandModule
    {
        private const string CommandVtf = "sprayme";
        private const string CommandVtfHi = "sprayme_hi";
        private const string CommandTga = "sprayme_tga";

        private readonly ILogger<SprayModule> _logger;
        private readonly Settings _settings;
        private readonly ISprayModuleCommandResolver _resolver;

        // CROSS MARK emoji https://www.fileformat.info/info/emoji/x/index.htm
        private const string DeleteEmojiString = "\u274C";
        public static Emoji DeleteEmote => new Emoji(DeleteEmojiString);

        public SprayModule(ILogger<SprayModule> logger, Settings settings, ISprayModuleCommandResolver resolver)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        [Command(CommandVtf)]
        [Summary("Converts an image into a Source engine-compatible spray in VTF format.")]
        public Task ConvertVtfAsync(string? arg1 = null, string? arg2 = null)
        {
            var saveProfile = new VtfSaveProfile();
            return HandleAsync(saveProfile, arg1, arg2);
        }

        [Command(CommandVtfHi)]
        [Summary("Converts an image into a Source engine-compatible spray in VTF format.")]
        public Task ConvertVtfHiAsync(string? arg1 = null, string? arg2 = null)
        {
            var saveProfile = new VtfHiResSaveProfile();
            return HandleAsync(saveProfile, arg1, arg2);
        }

        [Command(CommandTga)]
        [Summary("Converts an image into a Source engine-compatible spray in TGA format.")]
        public Task ConvertTgaAsync(string? arg1 = null, string? arg2 = null)
        {
            var saveProfile = new TgaSaveProfile();
            return HandleAsync(saveProfile, arg1, arg2);
        }

        private async Task HandleAsync(ISaveProfile saveProfile, string? arg1 = null, string? arg2 = null)
        {
            var client = new WebClient();
            var replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild?.Id);

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
                await Task.Delay(Constants.DelayAfterCommandMs);

                using var sourceStream = await client.OpenReadTaskAsync(result.SourceImageUri);

                var sprayTools = new SprayTools();

                try
                {
                    var outputStream = new MemoryStream();

                    var conversionResult = await sprayTools.ConvertAsync(
                        sourceStream, outputStream,
                        saveProfile, CancellationToken.None);

                    outputStream.Position = 0;

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

                    var isDm = Context.Channel is IPrivateChannel;
                    var sprayMessage = await Context.Channel.SendFileAsync(
                        outputStream, fileName,
                        $"Here ya go!\n\n(If you requested the conversion, react with {DeleteEmojiString} to delete this message." +
                        $"{(isDm ? "\nSince this is a DM, you may need to send me any other message first; it doesn't have to be a command." : "")})",
                        messageReference: replyToMessageRef);
                    await Task.Delay(Constants.DelayAfterCommandMs);

                    await sprayMessage.AddReactionAsync(DeleteEmote);
                    await Task.Delay(Constants.DelayAfterCommandMs);
                }
                catch (UnsupportedImageFormatException e)
                {
                    _logger.LogError(e, "Got an image format I don't support.");

                    var sorryMessage = await Context.Channel.SendMessageAsync(
                        "Sorry, I don't support that type of image :(",
                        messageReference: replyToMessageRef);
                    await Task.Delay(Constants.DelayAfterCommandMs);

                    await sorryMessage.AddReactionAsync(DeleteEmote);
                    await Task.Delay(Constants.DelayAfterCommandMs);
                }

                await tempMessage.DeleteAsync();
                await Task.Delay(Constants.DelayAfterCommandMs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Got an error converting image to a spray :(");
            }
        }

        public string GetGeneralHelpMessage(HelpContext helpContext)
        {
            var c = helpContext.GenericCommandExample;
            var u = "https://placekitten.com/200/300";

            var isVtf = helpContext.Command == CommandVtf || helpContext.Command == CommandVtfHi;

            return
                $"  - `{c} <string filenameOrSource>? <string sourceIfFilenameGiven>?`:\n" +
                $"    Creates a spray for use with Source-engine games, in {(isVtf ? "VTF" : "TGA")} fomat.\n" +
                $"    The spray can be specified in any of these ways:\n" +
                $"    1. Message starts with a URL\n" +
                $"    `{c} {u}`\n" +
                $"    2. Message starts with a filename and URL.\n" +
                $"    `{c} KITTEN {u}` or `{c} \"OMG A KITTEN\" {u}`\n" +
                $"    3. Message starts with a filename and has an image attachment.\n" +
                $"    `{c} KITTEN` or `{c} \"OMG A KITTEN\"` (must have an attached image)\n" +
                $"    4. Message starts with a filename and is a reply to a message with only a URL as its content.\n" +
                $"    `{c} KITTEN` or `{c} \"OMG A KITTEN\"` (must reply to an image-only message)\n" +
                $"    5. Message starts with a filename and is a reply to a message with an attachement.\n" +
                $"    `{c} KITTEN` or `{c} \"OMG A KITTEN\"` (must reply to a message with an attached image)\n" +
                $"    6. Message is empty and has an attachment.\n" +
                $"    7. Message is empty and is a reply to a message with only a URL as its content.\n" +
                $"    8. Message is empty and is a reply to a message with an attachment.";
        }
    }
}
