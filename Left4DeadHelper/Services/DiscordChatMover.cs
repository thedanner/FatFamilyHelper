using Discord;
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

namespace Left4DeadHelper.Services
{
    public class DiscordChatMover : IDiscordChatMover
    {
        private readonly ILogger<DiscordChatMover> _logger;
        private readonly Settings _settings;

        public DiscordChatMover(ILogger<DiscordChatMover> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task<MoveResult> MovePlayersToCorrectChannelsAsync(
            IRCONWrapper rcon,
            IDiscordSocketClientWrapper client, ISocketGuildWrapper guild,
            CancellationToken cancellationToken)
        {
            if (rcon is null) throw new ArgumentNullException(nameof(rcon));
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (guild is null) throw new ArgumentNullException(nameof(guild));

            var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == guild.Id);
            if (guildSettings == null)
            {
                throw new Exception($"Unable to find guild setting with ID {guild.Id} in the configuration.");
            }

            var result = new MoveResult();

            var primaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Primary.Id);
            if (primaryVoiceChannel == null) throw new Exception("Bad primary channel ID in config.");
            var secondaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Secondary.Id);
            if (secondaryVoiceChannel == null) throw new Exception("Bad secondary channel ID in config.");

            var printInfo = await rcon.SendCommandAsync<PrintInfo>("sm_printinfo");

            var currentPlayersOnServer = printInfo.Players
                 .Where(p => !"BOT".Equals(p.SteamId, StringComparison.CurrentCultureIgnoreCase))
                 .ToList();

            _logger.LogDebug("Current players per PrintInfo results ({0}):", currentPlayersOnServer.Count);
            for (var i = 0; i < currentPlayersOnServer.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i, currentPlayersOnServer[i].SteamId, currentPlayersOnServer[i].Name);
            }
            if (!currentPlayersOnServer.Any()) return result;

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
                result.UnmappedSteamUsers.Add(new UnmappedSteamUser(missingMappings[i].Name, missingMappings[i].SteamId));
                _logger.LogDebug("  {0}: {1} - {2}", i, missingMappings[i].SteamId, missingMappings[i].Name);
            }

            var currentPlayerDiscordSnowflakes = currentPlayerMappings
                .Select(p => p.DiscordId)
                .ToList();

            var discordAccountsForCurrentPlayers = guild.Users
                .Where(u => currentPlayerDiscordSnowflakes.Contains(u.Id))
                .ToList();

            _logger.LogDebug("Discord accounts found from mappings ({0}):", discordAccountsForCurrentPlayers.Count);
            for (var i = 0; i < discordAccountsForCurrentPlayers.Count; i++)
            {
                _logger.LogDebug("  {0}: {1} - {2}", i,
                    discordAccountsForCurrentPlayers[i].Id, discordAccountsForCurrentPlayers[i].Username);
            }

            _logger.LogDebug("Getting current voice channel users.");

            var getPrimaryChannelTask = guild.GetVoiceChannelAsync(primaryVoiceChannel.Id,
                options: new RequestOptions { CancelToken = cancellationToken });
            var getSecondaryChannelTask = guild.GetVoiceChannelAsync(secondaryVoiceChannel.Id,
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

            result.Success = true;

            foreach (var discordAccount in discordAccountsForCurrentPlayers)
            {
                ISocketVoiceChannelWrapper currentVoiceChannel;

                if (usersInPrimaryChannel != null && usersInPrimaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = primaryVoiceChannel;

                    _logger.LogDebug("{0} ({1}) found in primary channel.", discordAccount.Username, discordAccount.Id);
                }
                else if (usersInSecondaryChannel != null && usersInSecondaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = secondaryVoiceChannel;

                    _logger.LogDebug("{0} ({1}) found in secondary channel.", discordAccount.Username, discordAccount.Id);
                }
                else
                {
                    _logger.LogDebug("Skipping {0} ({1}): not in voice chat.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                if (primaryVoiceChannel.Id != currentVoiceChannel.Id && secondaryVoiceChannel.Id != currentVoiceChannel.Id)
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
                    intendedChannel = primaryVoiceChannel;
                }
                else if (usersPrintInfo.TeamIndex == PrintInfo.TeamIndexInfected)
                {
                    intendedChannel = secondaryVoiceChannel;
                }
                else
                {
                    continue;
                }

                if (currentVoiceChannel.Id != intendedChannel.Id)
                {
                    _logger.LogDebug("Moving {0} ({1}) to other voice channel (from {2} to {3}).",
                        discordAccount.Username, discordAccount.Id,
                        currentVoiceChannel.Name, intendedChannel.Name);

                    await discordAccount.ModifyAsync(p => p.ChannelId = intendedChannel.Id);

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    result.MoveCount++;
                }
            }

            return result;
        }
    }
}
