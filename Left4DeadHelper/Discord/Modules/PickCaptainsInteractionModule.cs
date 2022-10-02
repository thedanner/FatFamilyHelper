using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules;

public class PickCaptainsInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<PickCaptainsInteractionModule> _logger;
    private readonly Settings _settings;

    public PickCaptainsInteractionModule(ILogger<PickCaptainsInteractionModule> logger,
        Settings settings, IRCONWrapperFactory rconFactory, IDiscordChatMover chatMover) : base()
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    [SlashCommand("captains", "Picks two random users in the current user's voice chat.")]
    [RequireUserPermission(GuildPermission.MoveMembers)]
    public async Task HandleDivorceAsync(
        [Summary(description: "How many captains to pick?")] int teams = 2,
        [Summary(description: "User in the current channel to exlcude")] IUser? not1 = null,
        [Summary(description: "Another user in the current channel to exlcude (Discord doesn't support arrays here :(")] IUser? not2 = null,
        [Summary(description: "Another user in the current channel to exlcude (Discord doesn't support arrays here :(")] IUser? not3 = null,
        [Summary(description: "Another user in the current channel to exlcude (Discord doesn't support arrays here :(")] IUser? not4 = null,
        [Summary(description: "Another user in the current channel to exlcude (Discord doesn't support arrays here :(")] IUser? not5 = null
    )
    {
        if (Context.Guild == null) return;

        try
        {
            if (teams < 1)
            {
                await RespondAsync("If there are no teams, why do you need a captain? 🤔", ephemeral: true);
                return;
            }

            var genericVoiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
            if (genericVoiceChannel is null)
            {
                await RespondAsync("Please join a voice channel first. 📣", ephemeral: true);
                return;
            }

            if (genericVoiceChannel is not SocketVoiceChannel voiceChannel)
            {
                await RespondAsync("For some reason, the channel isn't the type I need. It's probably a bug, sorry. 😢", ephemeral: true);
                return;
            }

            if (voiceChannel.ConnectedUsers.Count < teams)
            {
                await RespondAsync("With how few players are here, everyone can be a captain! 🎉", ephemeral: true);
                return;
            }

            var potentialCaptains = new List<SocketGuildUser>(voiceChannel.ConnectedUsers);

#pragma warning disable CS8604 // Possible null reference argument.
            var excludes = new List<IUser>(5)
            {
                not1,
                not2,
                not3,
                not4,
                not5
            }.Where(u => u is not null);
#pragma warning restore CS8604 // Possible null reference argument.

            foreach (var excludedUser in excludes)
            {
                var captain = potentialCaptains.RemoveAll(u => u.Id == excludedUser.Id);
            }

            if (potentialCaptains.Count < teams)
            {
                await RespondAsync("After skipping some people, there's nobody left to be a captain. 🤷", ephemeral: true);
                return;
            }
            
            if (potentialCaptains.Count < teams)
            {
                await RespondAsync("After skipping some people, everyone can be a captain!", ephemeral: true);
                return;
            }

            var captains = new List<SocketGuildUser>(teams);

            var response = new StringBuilder();
            response.Append("RNJesus has determined the captains are:\n");
            var i = 1;

            while (teams > captains.Count)
            {
                var captain = RandomHelper.PickSecureRandom(potentialCaptains, out var index);
                captains.Add(captain);
                potentialCaptains.RemoveAt(index);

                response.Append(' ', 2).Append('#').Append(i++).Append(':').Append(' ')
                    .Append('*', 2).Append(captain.Nickname ?? captain.DisplayName).Append('*', 2);
            }

            await RespondAsync(response.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Got an error trying to pick captains :(");
        }
    }
}
