using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.Exceptions;
using Left4DeadHelper.Sprays.SaveProfiles;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    public class SprayModule : ModuleBase<SocketCommandContext>, ICommandModule
    {
        private const string CommandVtf = "sprayme";
        private const string CommandVtfHi = "sprayme1024";
        private const string CommandVtfAlpha = "sprayme512";
        private const string CommandTga = "spraymetga";
        private const string CommandFading = "sprayme-nearfar";

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
        [Summary("Converts an image into a Source engine-compatible spray in VTF format (1024x1020 or vice-versa; 1-bit alpha).\n" +
            "Use this version unless you have a specific need for a different one.\n" +
            "See the help for how to provide an image to convert.")]
        public Task ConvertVtfAsync(string? arg1 = null, string? arg2 = null)
        {
            return ConvertVtfHiAsync(arg1, arg2);
        }

        [Command(CommandVtfHi)]
        [Summary("Converts an image into a Source engine-compatible spray in VTF format (1024x1020 or vice-versa; 1-bit alpha).")]
        public async Task ConvertVtfHiAsync(string? arg1 = null, string? arg2 = null)
        {
            if (TryResolveArgs(arg1, arg2, out var result)) return;

            var saveProfile = new Vtf1024SaveProfile();
            await HandleAsync(saveProfile, result!.FileName, result!.SourceImageUri);
        }

        [Command(CommandVtfAlpha)]
        [Summary("Converts an image into a Source engine-compatible spray in VTF format (512x512 with 8-bit alpha).")]
        public async Task ConvertVtfAlphaAsync(string? arg1 = null, string? arg2 = null)
        {
            if (!TryResolveArgs(arg1, arg2, out var result)) return;

            var saveProfile = new Vtf512SaveProfile();
            await HandleAsync(saveProfile, result!.FileName, result!.SourceImageUri);
        }

        [Command(CommandTga)]
        [Summary("Converts an image into a Source engine-compatible spray in TGA format (256x256 with 8-bit alpha).\n" +
            "This is a legacy version that should only be used if you really need TGAs for a specific reason.")]
        public async Task ConvertTgaAsync(string? arg1 = null, string? arg2 = null)
        {
            if (!TryResolveArgs(arg1, arg2, out var result)) return;

            var saveProfile = new TgaSaveProfile();
            await HandleAsync(saveProfile, result!.FileName, result!.SourceImageUri);
        }

        [Command(CommandFading)]
        [Summary("Converts *two* images a fading Source engine-compatible spray. Both images should be the same size.\n" +
            "Args are `[filename optional] <near image URL> <far image URL>`")]
        public Task ConvertFadingAsync(string arg1, string arg2, string? arg3 = null)
        {
            string? fileName, nearImageUrl, farImageUrl;

            if (!string.IsNullOrEmpty(arg3))
            {
                fileName = arg1;
                nearImageUrl = arg2;
                farImageUrl = arg3!;
            }
            else
            {
                fileName = null;
                nearImageUrl = arg1;
                farImageUrl = arg2;
            }

            var nearImageUri = new Uri(nearImageUrl, UriKind.Absolute);
            var farImageUri = new Uri(farImageUrl, UriKind.Absolute);

            var saveProfile = new VtfFadingSaveProfile();
            return HandleAsync(saveProfile, fileName, nearImageUri, farImageUri);
        }

        private bool TryResolveArgs(string? arg1, string? arg2, out SprayModuleParseResult? result)
        {
            result = _resolver.Resolve(arg1, arg2, Context.Message);

            // No source found
            if (result == null)
            {
                _logger.LogInformation("Couldn't resolve an image; nothing to convert.");
                return false;
            }

            return true;
        }

        private async Task HandleAsync(ISaveProfile saveProfile, string? fileName, params Uri[] imageUris)
        {
            if (saveProfile is null) throw new ArgumentNullException(nameof(saveProfile));
            if (imageUris is null) throw new ArgumentNullException(nameof(imageUris));
            if (imageUris.Length == 0) throw new ArgumentException("At least one image URI is required.", nameof(imageUris));

            var dmChannel = Context.Channel as SocketDMChannel;
            if (dmChannel != null)
            {
                _logger.LogInformation(
                    "Spray requested from DM conversation {convoId} with recipients {recipients}.",
                    dmChannel.Id,
                    string.Join(", ", (dmChannel as IPrivateChannel).Recipients.Select(r => $"{r.Username}#{r.Discriminator} ({r.Id})")));
            }

            var replyToMessageRef = new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild?.Id);

            _logger.LogInformation("Triggered by message with ID {0}.", Context.Message.Id);

            var sourceStreams = new Stream[0];

            try
            {
                var tempMessage = await ReplyAsync("Working on it...", messageReference: replyToMessageRef);
                await Task.Delay(Constants.DelayAfterCommandMs);

                var sourceStreamTasks = imageUris.Select(async s =>
                {
                    var client = new WebClient();
                    return await client.OpenReadTaskAsync(s);
                });
                sourceStreams = await Task.WhenAll(sourceStreamTasks);

                var sprayTools = new SprayTools();

                try
                {
                    using var outputStream = new MemoryStream();

                    var conversionResult = await sprayTools.ConvertAsync(
                        sourceStreams, outputStream,
                        saveProfile, CancellationToken.None);

                    if (conversionResult.IsSuccessful)
                    {
                        outputStream.Position = 0;

                        if (string.IsNullOrEmpty(fileName))
                        {
                            fileName = imageUris[0].LocalPath;

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

                        if (!fileName.EndsWith(conversionResult.FileExtension!, StringComparison.CurrentCultureIgnoreCase))
                        {
                            fileName += conversionResult.FileExtension;
                        }

                        if (string.Equals(fileName, conversionResult.FileExtension))
                        {
                            fileName = "spray" + conversionResult.FileExtension;
                        }

                        var sprayMessage = await Context.Channel.SendFileAsync(
                            outputStream, fileName,
                            $"Here ya go!\n\n(If you requested this, react with {DeleteEmojiString} to delete this message." +
                            $"{(dmChannel != null ? "\nSince this is a DM, you may need to send me any other message first; it doesn't have to be a command." : "")})",
                            messageReference: replyToMessageRef);
                        await Task.Delay(Constants.DelayAfterCommandMs);

                        await sprayMessage.AddReactionAsync(DeleteEmote);
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
                    else
                    {
                        var sprayMessage = await Context.Channel.SendMessageAsync(
                            $"The conversion failed: {conversionResult.Message}.\n\n" +
                            $"(If you requested this, react with {DeleteEmojiString} to delete this message." +
                            $"{(dmChannel != null ? "\nSince this is a DM, you may need to send me any other message first; it doesn't have to be a command." : "")})",
                            messageReference: replyToMessageRef);
                        await Task.Delay(Constants.DelayAfterCommandMs);

                        await sprayMessage.AddReactionAsync(DeleteEmote);
                        await Task.Delay(Constants.DelayAfterCommandMs);
                    }
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
            finally
            {
                foreach (var sourceStream in sourceStreams)
                {
                    sourceStream.Dispose();
                }
            }
        }

        public string GetGeneralHelpMessage(HelpContext helpContext)
        {
            var c = helpContext.GenericCommandExample;
            var u = "https://placekitten.com/200/300";

            var isVtf = helpContext.Command != CommandTga;

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
                $"    6. Message is just the trigger and has an attachment.\n" +
                $"    7. Message is just the trigger and is a reply to a message with only a URL as its content.\n" +
                $"    8. Message is just the trigger and is a reply to a message with an attachment.";
        }
    }
}
