using Discord;
using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Minecraft;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules;

public class MinecraftPlayersModule : ModuleBase<SocketCommandContext>, ICommandModule
{
    private const string Command = "players";
    private const string Alias1 = "who";

    private readonly ILogger<RconModule> _logger;
    private readonly Settings _settings;
    private readonly IMinecraftPingService _minecraftPingService;

    public MinecraftPlayersModule(ILogger<RconModule> logger, Settings settings,
        IMinecraftPingService minecraftPingService) : base()
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _minecraftPingService = minecraftPingService ?? throw new ArgumentNullException(nameof(minecraftPingService));
    }

    [Command(Command)]
    [Alias(Alias1)]
    [Summary("Gets who's playing on our Minecraft server.")]
    public async Task HandleCommandAsync()
    {
        if (_settings.Minecraft == null) return;

        try
        {
            var message = Context.Message;
            if (message == null) return;

            if (_settings.Minecraft.ChannelIdFilter?.Any() == true)
            {
                var channelFilter = _settings.Minecraft.ChannelIdFilter!;
                if (!channelFilter.Contains(Context.Channel.Id)) return;
            }

            var defaultServerName = _settings.Minecraft.DefaultServerName ?? "";
            var server = _settings.Minecraft.Servers.FirstOrDefault(s => s.Name == defaultServerName)
                ?? _settings.Minecraft.Servers.FirstOrDefault();

            if (server == null) return;

            var response = await _minecraftPingService.PingAsync(server.Hostname, server.Port);

            if (response == null)
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
                playersMessage.Append("*Nobody is palying right now.*");
            }

            embedBuilder
                .AddField("Players", playersMessage.ToString())
                .WithColor(Color.Green) // TODO Minecraft grass
                .WithFooter($"{server.Hostname}:{server.Port}");

            await ReplyAsync(null, embed: embedBuilder.Build());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in {className}.{methodName}().", nameof(RconModule), nameof(HandleCommandAsync));

            await ReplyAsync($"Sorry, there was an error. My logs have more information.");
        }
    }

    public string GetGeneralHelpMessage(HelpContext helpContext) =>
        $"  - `{helpContext.GenericCommandExample}`: Gets who's playing on our Minecraft server.";
}
