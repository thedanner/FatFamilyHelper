using Discord;
using Discord.Commands;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Helpers.DiscordExtensions;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2, Constants.GroupLfd, Constants.GroupLfd2,
        GroupDivorce, GroupDontMarriage)]
    public class MoveChannelsModule : ModuleBase<SocketCommandContext>, ICommandModule
    {
        private const string Command = "vc";
        
        public const string GroupDivorce = "divorce";
        public const string GroupDontMarriage = "dontmarriage";

        private readonly ILogger<MoveChannelsModule> _logger;
        private readonly Settings _settings;
        private readonly IRCONWrapperFactory _rconFactory;
        private readonly IDiscordChatMover _mover;

        public MoveChannelsModule(ILogger<MoveChannelsModule> logger, Settings settings,
            IRCONWrapperFactory rconFactory, IDiscordChatMover mover) : base()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _rconFactory = rconFactory ?? throw new ArgumentNullException(nameof(rconFactory));
            _mover = mover ?? throw new ArgumentNullException(nameof(mover));
        }

        private static readonly object MoveLock = new object();
        private static bool _isMoving;
        private static bool IsMoving
        {
            get { lock (MoveLock) { return _isMoving; } }
            set { lock (MoveLock) { _isMoving = true; } }
        }

        [Command]
        [Alias(Command)]
        [Summary("Moves users into respective voice channels based on game team.")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task HandleVoiceChatAsync()
        {
            if (Context.Message == null) return;
            if (Context.Guild == null) return;

            try
            {
                using var rcon = _rconFactory.GetRcon();

                await rcon.ConnectAsync();

                var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == Context.Guild.Id);

                var moveResult = await _mover.MovePlayersToCorrectChannelsAsync(
                    rcon,
                    new DiscordSocketClientWrapper(Context.Client),
                    new SocketGuildWrapper(Context.Guild),
                    CancellationToken.None);

                string replyMessage;

                if (moveResult.MoveCount == 0)
                {
                    replyMessage = "Nobody was playing.";
                }
                else if (moveResult.MoveCount == 1)
                {
                    replyMessage = "1 player moved.";
                }
                else
                {
                    replyMessage = $"{moveResult.MoveCount} players moved.";
                }

                if (moveResult.UnmappedSteamUsers.Any())
                {
                    string whoShouldFix;
                    if (guildSettings != null && guildSettings.ConfigMaintainers.Any())
                    {
                        whoShouldFix = string.Join(", ", guildSettings.ConfigMaintainers.Select(u =>
                            DiscordMessageExtensions.ToDiscordUserIdMessageRef(u.DiscordId)));
                    }
                    else
                    {
                        whoShouldFix = "someone";
                    }

                    replyMessage +=
                        $"\n\nSorry, I couldn't move these people because I don't know enough about them: " +
                        $"\n{string.Join(", ", moveResult.UnmappedSteamUsers.Select(u => u.Name))}. Bother {whoShouldFix} to fix it.";
                }

                await ReplyAsync(replyMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Got an error trying to move players :(");
            }
            finally
            {
                IsMoving = false;
            }
        }

        public string GetGeneralHelpMessage(HelpContext helpContext) =>
            $"  - `{helpContext.GenericCommandExample}`:\n" +
            $"    Moves players on our Left 4 Dead server into separate Discord voice channels channels per team.\n" +
            $"    Base command aliases: `{Constants.GroupL4d2}`, `{Constants.GroupLfd}`, `{Constants.GroupLfd2}`, `{GroupDivorce}`;\n" +
            $"    the `{Command}` sub-command is optional on all aliases.";
    }
}
