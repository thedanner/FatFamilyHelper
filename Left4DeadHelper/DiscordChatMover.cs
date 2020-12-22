using Discord;
using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models;
using Left4DeadHelper.Rcon;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper
{
    public class DiscordChatMover : IDiscordChatMover
    {
        private readonly ILogger<DiscordChatMover> _logger;
        private readonly Settings _settings;

        internal IDiscordSocketClientWrapper? _client;
        internal ISocketGuildWrapper? _guild;
        internal ISocketVoiceChannelWrapper? _primaryVoiceChannel;
        internal ISocketVoiceChannelWrapper? _secondaryVoiceChannel;

        public DiscordChatMover(ILogger<DiscordChatMover> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task StartAsync(IDiscordSocketClientWrapper client, CancellationToken cancellationToken)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            var readyComplete = new TaskCompletionSource<bool>();
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            _client.Connected += async () => _logger.LogInformation("Discord client event: Connected");

            async Task initalReadyAsync() => await ReadyHandlerWithSignalAsync(readyComplete);
            _client.Ready += initalReadyAsync;

            _client.Disconnected += async (ex) => _logger.LogError(ex, "Discord client event: Disconnected");
            _client.GuildAvailable += async (guild) => _logger.LogInformation("Discord client event: GuildAvailable");
            _client.GuildMembersDownloaded += async (ex) => _logger.LogInformation("Discord client event: GuildMembersDownloaded");
            _client.Log += async (logMessage) =>
            {
                var level = logMessage.Severity.ToLogLevel();

                _logger.Log(
                    level,
                    logMessage.Exception,
                    "Discord client event: Log: (Source: {0}): {1}",
                    logMessage.Source, logMessage.Message);
            };
            _client.LoggedIn += async () => _logger.LogInformation("Discord client event: LoggedIn");
            _client.LoggedOut += async () => _logger.LogInformation("Discord client event: LoggedOut");

            await _client.LoginAsync(TokenType.Bot, _settings.DiscordSettings.BotToken);
            await _client.StartAsync();

            await readyComplete.Task;

            _client.Ready -= initalReadyAsync;
            _client.Ready += async () =>
            {
                _logger.LogInformation("Discord client event: Ready");
            };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            _guild = _client.GetGuild(_settings.DiscordSettings.GuildId);
            if (_guild == null) throw new Exception("guild is null");

            // TODO listen for voice channel changes from the server, or config file changes.
            _primaryVoiceChannel = _guild.GetVoiceChannel(_settings.DiscordSettings.Channels["primary"].Id);
            if (_primaryVoiceChannel == null) throw new Exception("Bad ChannelIdPrimary");
            _secondaryVoiceChannel = _guild.GetVoiceChannel(_settings.DiscordSettings.Channels["secondary"].Id);
            if (_secondaryVoiceChannel == null) throw new Exception("Bad ChannelIdSecondary");
        }

        private Task ReadyHandlerWithSignalAsync(TaskCompletionSource<bool> readyComplete)
        {
            _logger.LogInformation("Discord client event: Ready");
            readyComplete.SetResult(true);
            return Task.FromResult(0);
        }

        public async Task<int> MovePlayersToCorrectChannelsAsync(IRCONWrapper rcon, CancellationToken cancellationToken)
        {
            if (rcon is null) throw new ArgumentNullException(nameof(rcon));

            if (_client is null) throw new Exception(nameof(_client) + " not set. Run StartAsync() first.");
            if (_guild is null) throw new Exception(nameof(_guild) + " not set. Run StartAsync() first.");
            if (_primaryVoiceChannel is null) throw new Exception(nameof(_primaryVoiceChannel) + " not set. Run StartAsync() first.");
            if (_secondaryVoiceChannel is null) throw new Exception(nameof(_secondaryVoiceChannel) + " not set. Run StartAsync() first.");

            var printInfo = await rcon.SendCommandAsync<PrintInfo>("sm_printinfo");

            var currentPlayersOnServer = printInfo.Players
                 .Where(p => !"BOT".Equals(p.SteamId, StringComparison.CurrentCultureIgnoreCase))
                 .ToList();

            _logger.LogDebug("Current players per PrintInfo results ({0}):", currentPlayersOnServer.Count);
            for (var i = 0; i < currentPlayersOnServer.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i, currentPlayersOnServer[i].SteamId, currentPlayersOnServer[i].Name);
            }
            if (!currentPlayersOnServer.Any()) return -1; // TODO better return status object.

            var currentlyPlayingSteamIds = currentPlayersOnServer
                .Select(p => p.SteamId)
                .ToList();

            var currentPlayerMappings = _settings.UserMappings
                .Where(d => currentlyPlayingSteamIds.Contains(d.SteamId, StringComparer.CurrentCultureIgnoreCase))
                .ToList();

            _logger.LogDebug("Current players found in mapping data ({0}):", currentPlayerMappings.Count);
            for (var i = 0; i < currentPlayerMappings.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2} - {3}", i,
                    currentPlayerMappings[i].SteamId, currentPlayerMappings[i].DiscordId, currentPlayerMappings[i].Name);
            }

            var allSteamIdsFromUserMappings = _settings.UserMappings.Select(um => um.SteamId).ToList();
            var missingMappings = currentPlayersOnServer
                .Where(p => !allSteamIdsFromUserMappings.Contains(p.SteamId, StringComparer.CurrentCultureIgnoreCase))
                .ToList();

            _logger.LogDebug("Current players MISSING from mapping data ({0}):", missingMappings.Count);
            for (var i = 0; i < missingMappings.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i, missingMappings[i].SteamId, missingMappings[i].Name);
            }

            var currentPlayerDiscordSnowflakes = currentPlayerMappings
                .Select(p => p.DiscordId)
                .ToList();

            var discordAccountsForCurrentPlayers = _guild.Users
                .Where(u => currentPlayerDiscordSnowflakes.Contains(u.Id))
                .ToList();

            _logger.LogDebug("Discord accounts found from mappings ({0}):", discordAccountsForCurrentPlayers.Count);
            for (var i = 0; i < discordAccountsForCurrentPlayers.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i,
                    discordAccountsForCurrentPlayers[i].Id, discordAccountsForCurrentPlayers[i].Username);
            }

            _logger.LogDebug("Getting current voice channel users.");

            var getPrimaryChannelTask = _guild.GetVoiceChannelAsync(_primaryVoiceChannel.Id,
                options: new RequestOptions { CancelToken = cancellationToken });
            var getSecondaryChannelTask = _guild.GetVoiceChannelAsync(_secondaryVoiceChannel.Id,
                options: new RequestOptions { CancelToken = cancellationToken });

            await Task.WhenAll(getPrimaryChannelTask, getSecondaryChannelTask);

            var priamryChannel = getPrimaryChannelTask.Result;
            var secondaryChannel = getSecondaryChannelTask.Result;

            var getPrimaryChannelUsersTask = priamryChannel.GetUsersAsync(
                    options: new RequestOptions { CancelToken = cancellationToken })
                .FlattenAsync();
            var getSecondaryChannelUsersTask = secondaryChannel.GetUsersAsync(
                    options: new RequestOptions { CancelToken = cancellationToken })
                .FlattenAsync();

            await Task.WhenAll(getPrimaryChannelTask, getSecondaryChannelUsersTask);

            List<IGuildUser> usersInPrimaryChannel = getPrimaryChannelUsersTask.Result.ToList();
            List<IGuildUser> usersInSecondaryChannel = getSecondaryChannelUsersTask.Result.ToList();

            _logger.LogDebug("Discord accounts found from mappings ({0}):", discordAccountsForCurrentPlayers.Count);
            for (var i = 0; i < discordAccountsForCurrentPlayers.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i,
                    discordAccountsForCurrentPlayers[i].Id, discordAccountsForCurrentPlayers[i].Username);
            }

            var moveCount = 0;

            foreach (var discordAccount in discordAccountsForCurrentPlayers)
            {
                ISocketVoiceChannelWrapper currentVoiceChannel;

                if (usersInPrimaryChannel != null && usersInPrimaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = _primaryVoiceChannel;

                    _logger.LogDebug("{0} ({1}) found in primary channel.", discordAccount.Username, discordAccount.Id);
                }
                else if (usersInSecondaryChannel != null && usersInSecondaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = _secondaryVoiceChannel;

                    _logger.LogDebug("{0} ({1}) found in secondary channel.", discordAccount.Username, discordAccount.Id);
                }
                else
                {
                    _logger.LogDebug("Skipping {0} ({1}): not in voice chat.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                if (_primaryVoiceChannel.Id != currentVoiceChannel.Id && _secondaryVoiceChannel.Id != currentVoiceChannel.Id)
                {
                    _logger.LogDebug("Skipping {0} ({1}): not in a designated voice channel.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                var userMapping = _settings.UserMappings.FirstOrDefault(um => um.DiscordId == discordAccount.Id);
                if (userMapping == null)
                {
                    _logger.LogDebug("Skipping {0} ({1}): Couldn't find user mapping.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                var usersPrintInfo = printInfo.Players.FirstOrDefault(pi => userMapping.SteamId.Equals(pi.SteamId, StringComparison.CurrentCultureIgnoreCase));
                if (usersPrintInfo == null)
                {
                    _logger.LogDebug("Skipping {0} ({1}): Couldn't find user's Steam ID in PrintInfo results.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                ISocketVoiceChannelWrapper intendedChannel;
                if (usersPrintInfo.TeamIndex == PrintInfo.TeamIndexSurvivor)
                {
                    intendedChannel = _primaryVoiceChannel;
                }
                else if (usersPrintInfo.TeamIndex == PrintInfo.TeamIndexInfected)
                {
                    intendedChannel = _secondaryVoiceChannel;
                }
                else
                {
                    continue;
                }

                if (currentVoiceChannel.Id != intendedChannel.Id)
                {
                    _logger.LogDebug("Moving {0} ({1}) to other voice channel (from {2} to {3}).",
                        discordAccount.Username, discordAccount.Id,
                        discordAccount.VoiceChannel.Name, intendedChannel.Name);

                    await discordAccount.ModifyAsync(p => p.ChannelId = intendedChannel.Id);

                    // Some people lost incoming audio from other Discord users when moving.
                    // Hopefully a delay will allow the connections to correctly re-establish themselves.
                    // This issue never came up on manual moves so it could be a race condition.
                    await Task.Delay(TimeSpan.FromMilliseconds(250));

                    moveCount++;
                }
            }

            return moveCount;
        }
    }
}
