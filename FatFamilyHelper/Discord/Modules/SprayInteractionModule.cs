using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FatFamilyHelper.Helpers;
using FatFamilyHelper.Sprays;
using FatFamilyHelper.Sprays.Exceptions;
using FatFamilyHelper.Sprays.SaveProfiles;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Modules;

[Group("sprayme", "Converts an image into a Source engine-compatible spary.")]
public class SprayInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    public const string ButtonCustomIdForDeleteRequest = "sprayme_deleteRequest";

    private readonly ILogger<SprayInteractionModule> _logger;
    private readonly ISprayModuleCommandResolver _resolver;
    private readonly HttpClient _httpClient;

    // CROSS MARK emoji https://www.fileformat.info/info/emoji/x/index.htm
    private const string DeleteEmojiString = "\u274C";
    public static Emoji DeleteEmote => new(DeleteEmojiString);

    public SprayInteractionModule(ILogger<SprayInteractionModule> logger, ISprayModuleCommandResolver resolver, HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    [SlashCommand("high-res-url",
        "Converts to VTF format (1024x1020 or vice-versa; 1-bit alpha).")]
    public async Task ConvertVtfHighResUrlAsync(
        [Summary("sourceUrl", "URL of image to convert.")] string sourceUrl,
        [Summary("fileName", "Optional; use this to customize the file name.")] string? fileName = null
    )
    {
        if (Uri.TryCreate(sourceUrl, UriKind.Absolute, out var sourceImageUri))
        {
            var saveProfile = new Vtf1024SaveProfile();
            await HandleAsync(saveProfile, fileName, sourceImageUri);
            return;
        }

        await RespondAsync("The given source doesn't look like a URL.");
    }

    [SlashCommand("high-res-attachment",
        "Converts to VTF format (1024x1020 or vice-versa; 1-bit alpha).")]
    public Task ConvertVtfHighResAttachmentAsync(
        [Summary("image", "Image to convert.")] IAttachment attachment,
        [Summary("fileName", "Optional; use this to customize the file name.")] string? fileName = null
    )
    {
        return ConvertVtfHighResUrlAsync(attachment.ProxyUrl, !string.IsNullOrEmpty(fileName) ? fileName : attachment.Filename);
    }

    [MessageCommand("sprayme: high-res")]
    public async Task ConvertVtfHighResMessageAsync(IMessage message) // SocketMessage
    {
        if (_resolver.TryHandleReferencedMessage(message, null, out var result))
        {
            var saveProfile = new Vtf1024SaveProfile();
            await HandleAsync(saveProfile, result.FileName, result.SourceImageUri);
            return;
        }

        await RespondAsync("That message doesn't look like it has an image in it.");
    }

    [SlashCommand("8-bit-alpha-url",
        "Converts to VTF format (512x512 with 8-bit alpha).")]
    public async Task ConvertVtf8BitAlphaUrlAsync(
        [Summary("sourceUrl", "URL of image to convert.")] string sourceUrl,
        [Summary("fileName", "Optional; use this to customize the file name.")] string? fileName = null
    )
    {
        if (Uri.TryCreate(sourceUrl, UriKind.Absolute, out var sourceImageUri))
        {
            var saveProfile = new Vtf512SaveProfile();
            await HandleAsync(saveProfile, fileName, sourceImageUri);
            return;
        }

        await RespondAsync("The given source doesn't look like a URL.");
    }

    [SlashCommand("8-bit-alpha-attachment",
        "Converts to VTF format (512x512 with 8-bit alpha).")]
    public Task ConvertVtf8BitAlphaAttachmentAsync(
        [Summary("image", "Image to convert.")] IAttachment attachment,
        [Summary("fileName", "Optional; use this to customize the file name.")] string? fileName = null
    )
    {
        return ConvertVtf8BitAlphaUrlAsync(attachment.ProxyUrl, !string.IsNullOrEmpty(fileName) ? fileName : attachment.Filename);
    }

    [MessageCommand("sprayme: 8-bit-alpha")]
    public async Task ConvertVtfAlphaMessageAsync(IMessage message) // SocketMessage
    {
        if (_resolver.TryHandleReferencedMessage(message, null, out var result))
        {
            var saveProfile = new Vtf512SaveProfile();
            await HandleAsync(saveProfile, result.FileName, result.SourceImageUri);
            return;
        }

        await RespondAsync("That message doesn't look like it has an image in it.");
    }

    [SlashCommand("near-far-urls",
        "Converts *two* images a fading spray. Both images should be the same size.")]
    public async Task ConvertNearFarUrlsAsync(string nearImageUrl, string farImageUrl, string? fileName = null)
    {
        if (!Uri.TryCreate(nearImageUrl, UriKind.Absolute, out var nearImageUri))
        {
            await RespondAsync("The near image source doesn't look like a URL.");
            return;
        }

        if (!Uri.TryCreate(farImageUrl, UriKind.Absolute, out var farImageUri))
        {
            await RespondAsync("The far image source doesn't look like a URL.");
            return;
        }

        var saveProfile = new VtfFadingSaveProfile();
        await HandleAsync(saveProfile, fileName, nearImageUri, farImageUri);
    }

    [SlashCommand("near-far-attachments",
        "Converts two images a fading spray. Both images should be the same size.")]
    public Task ConvertNearFarAttachmentsAsync(IAttachment nearImage, IAttachment farImage, string? fileName = null)
    {
        return ConvertNearFarUrlsAsync(nearImage.ProxyUrl, farImage.ProxyUrl, !string.IsNullOrEmpty(fileName) ? fileName : nearImage.Filename);
    }

    [MessageCommand("sprayme: near-far")]
    public async Task ConvertFadingMessageAsync(IMessage message) // SocketMessage
    {
        var saveProfile = new VtfFadingSaveProfile();

        if (message.Attachments.Count == 2)
        {
            using var enumerator = message.Attachments.GetEnumerator();

            enumerator.MoveNext();
            var nearAttachment = enumerator.Current;
            enumerator.MoveNext();
            var farAttachment = enumerator.Current;

            var nearImageUri = new Uri(nearAttachment.Url);
            var farImageUri = new Uri(farAttachment.Url);

            await HandleAsync(saveProfile, null, nearImageUri, farImageUri);
            return;
        }

        var messageTokens = message.Content.Split(' ', 3,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (messageTokens.Length == 2)
        {
            if (!Uri.TryCreate(messageTokens[0], UriKind.Absolute, out var nearImageUri))
            {
                await RespondAsync("The near image source doesn't look like a URL.");
                return;
            }

            if (!Uri.TryCreate(messageTokens[1], UriKind.Absolute, out var farImageUri))
            {
                await RespondAsync("The far image source doesn't look like a URL.");
                return;
            }

            await HandleAsync(saveProfile, null, nearImageUri, farImageUri);
        }

        await RespondAsync("I couldn't find two attachments or two valid image URLs in that message.");
    }

    private async Task HandleAsync(ISaveProfile saveProfile, string? fileName, params Uri[] imageUris)
    {
        if (saveProfile is null) throw new ArgumentNullException(nameof(saveProfile));
        if (imageUris is null) throw new ArgumentNullException(nameof(imageUris));
        if (imageUris.Length == 0) throw new ArgumentException("At least one image URI is required.", nameof(imageUris));

        var cancellationToken = CancellationToken.None;

        if (Context.Channel is SocketDMChannel dmChannel)
        {
            _logger.LogInformation(
                "Spray requested from DM conversation {dmChannelId} with recipients {recipients}.",
                dmChannel.Id,
                string.Join(", ", (dmChannel as IPrivateChannel).Recipients.Select(r => $"{r.Username}#{r.Discriminator} ({r.Id})")));
        }

        _logger.LogInformation("Triggered by slash or message command.");

        var sourceStreams = Array.Empty<Stream>();

        try
        {
            await DeferAsync();

            var sourceStreamTasks = imageUris.Select(async s =>
            {
                return await _httpClient.GetStreamAsync(s, cancellationToken);
            });
            sourceStreams = await Task.WhenAll(sourceStreamTasks);

            var sprayTools = new SprayTools();

            try
            {
                using var outputStream = new MemoryStream();

                var conversionResult = await sprayTools.ConvertAsync(
                    sourceStreams, outputStream,
                    saveProfile, cancellationToken);

                var buttonBuilder = new ButtonBuilder()
                    .WithEmote(DeleteEmote)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId(ButtonCustomIdForDeleteRequest);

                var componentBuilder = new ComponentBuilder()
                    .WithButton(buttonBuilder);

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

                    var sprayMessage = await FollowupWithFileAsync(
                        outputStream, fileName,
                        $"Here ya go!\n\n(If you requested this, use the {DeleteEmojiString} button to delete this message.)",
                        components: componentBuilder.Build()
                        );
                    await Task.Delay(Constants.DelayAfterCommandMs);
                }
                else
                {
                    var sprayMessage = await FollowupAsync(
                        $"The conversion failed: {conversionResult.Message}.",
                        components: componentBuilder.Build(),
                        ephemeral: true
                        );
                    await Task.Delay(Constants.DelayAfterCommandMs);
                }
            }
            catch (UnsupportedImageFormatException e)
            {
                _logger.LogError(e, "Got an image format I don't support.");

                var sorryMessage = await FollowupAsync(
                    "Sorry, I don't support that type of image :(",
                    ephemeral: true);
                await Task.Delay(Constants.DelayAfterCommandMs);
            }
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
}
