using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Modules;

public class SprayDeleteRequestCommandModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ILogger<SprayInteractionModule> _logger;

    public SprayDeleteRequestCommandModule(ILogger<SprayInteractionModule> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [ComponentInteraction(SprayInteractionModule.ButtonCustomIdForDeleteRequest)]
    public async Task HandleDeleteRequestAsync()
    {
        var conversionMessage = Context.Interaction.Message;

        if (await ShouldDeleteMessageAsync(conversionMessage))
        {
            await conversionMessage.DeleteAsync();
            return;
        }

        await RespondAsync("You didn't post the image or request the conversion, so you can't delete it. Sorry.", ephemeral: true);
    }

    private async Task<bool> ShouldDeleteMessageAsync(SocketUserMessage conversionMessage)
    {
        // The conversion result mesage will be a reply to the source. Allow the conversion result to be deleted by any one of the following:
        // - the user who requested the conversion
        // - the user who posted the thing that was converted
        // - anybody if the message that had the original thing that was converted was deleted.

        if (conversionMessage.Author == Context.Interaction.User)
        {
            return true;
        }

        // Make sure we have a reference. Don't allow cross-posts.
        if (conversionMessage.Reference is not null
            // And that it's to a message
            && conversionMessage.Reference.MessageId.IsSpecified
            // in the same guild (or there is no guild, so it's a DM)
            && (
                ( !conversionMessage.Reference.GuildId.IsSpecified
                    && conversionMessage.Channel is SocketDMChannel
                )
                || (
                    Context.Interaction.GuildId.HasValue
                    && conversionMessage.Reference.GuildId.Value == Context.Interaction.GuildId.Value
                )
            )
            // and same channel
            && conversionMessage.Channel is not null && conversionMessage.Reference.ChannelId == conversionMessage.Channel.Id)
        {
            var referencedMessage = await conversionMessage.Channel.GetMessageAsync(conversionMessage.Reference.MessageId.Value);

            // If the referenced message was deleted (referencedIMessage is null), let anyone remove the conversion
            // message. This will allow users to delete their own requested conversions if they accidentally delete
            // the original message first.
            // I don't really care if anyone else deletes these either.

            if (referencedMessage is null)
            {
                return true;
            }

            if (referencedMessage is not IUserMessage)
            {
                return true;
            }
        }

        return false;
    }
}
