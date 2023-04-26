using Discord;
using Discord.Interactions;
using FatFamilyHelper.Helpers;
using FatFamilyHelper.Games.Minecraft;
using FatFamilyHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Modules;

public class MinecraftPlayersInteractiveModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string CommandName = "who";

    private readonly ILogger<MinecraftPlayersInteractiveModule> _logger;
    private readonly IMinecraftPingService _minecraftPingService;
    private readonly MinecraftSettings? _minecraftSettings;

    public MinecraftPlayersInteractiveModule(ILogger<MinecraftPlayersInteractiveModule> logger,
        IOptions<MinecraftSettings>? minecraftSettings,
        IMinecraftPingService minecraftPingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minecraftPingService = minecraftPingService ?? throw new ArgumentNullException(nameof(minecraftPingService));
        _minecraftSettings = minecraftSettings?.Value;
    }

    //[SlashCommand(CommandName, "Gets who's playing on our Minecraft server.")]
    public async Task HandleCommandAsync()
    {
        await DeferAsync();
        await Task.Delay(Constants.DelayAfterCommand);

        if (_minecraftSettings is null)
        {
            await DeleteOriginalResponseAsync();
            await Task.Delay(Constants.DelayAfterCommand);
            return;
        }

        try
        {
            if (_minecraftSettings.ChannelIdFilter?.Any() == true)
            {
                var channelFilter = _minecraftSettings.ChannelIdFilter!;
                if (!channelFilter.Contains(Context.Channel.Id)) return;
            }

            var defaultServerName = _minecraftSettings.DefaultServerName ?? "";
            var server = _minecraftSettings.Servers.FirstOrDefault(s => s.Name == defaultServerName)
                ?? _minecraftSettings.Servers.FirstOrDefault();

            if (server is null) return;

            var response = await _minecraftPingService.PingAsync(server.Hostname, server.Port);

            if (response is null)
            {
                _logger.LogInformation("No payload returned from the ping method; it may be throttled.");
                return;
            }

            var plural = response.Players.Online == 1 ? "" : "s";
            var embedBuilder = new EmbedBuilder
            {
                Title = response.Description?.Text ?? server.Name,
                Description = string.Format("{0} of {1} player{2}",
                    response.Players.Online, response.Players.Max, plural)
            };


            var playersMessage = new StringBuilder();

            if (response.Players.Online != 0)
            {
                foreach (var player in response.Players.Sample)
                {
                    playersMessage.Append('\n').Append(player.Name);
                }
            }
            else
            {
                playersMessage.Append("*Nobody is playing right now.*");
            }

            embedBuilder
                .AddField("Players", playersMessage.ToString())
                .WithColor(Color.Green) // TODO Minecraft grass
                .WithFooter($"{server.Hostname}:{server.Port}");

            await FollowupAsync(null, embed: embedBuilder.Build());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in {className}.{methodName}().", nameof(MinecraftPlayersInteractiveModule), nameof(HandleCommandAsync));

            await FollowupAsync($"Sorry, there was an error. My logs have more information.");
        }
    }
}
