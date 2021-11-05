using Discord;
using Left4DeadHelper.Models.Configuration;
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
            IDiscordSocketClientWrapper client,
            ISocketGuildWrapper guild,
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

            var primaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Primary.Id);
            if (primaryVoiceChannel == null) throw new Exception("Bad primary channel ID in config.");
            var secondaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Secondary.Id);
            if (secondaryVoiceChannel == null) throw new Exception("Bad secondary channel ID in config.");
            
            var result = new MoveResult();

            var printInfo = await rcon.SendCommandAsync<PrintInfo>("sm_printinfo");

            var currentPlayersOnServer = printInfo.Players
                 .Where(p => !"BOT".Equals(p.SteamId, StringComparison.CurrentCultureIgnoreCase))
                 .ToList();

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

            var usersInPrimaryChannel = getPrimaryChannelUsersTask.Result.ToList();
            var usersInSecondaryChannel = getSecondaryChannelUsersTask.Result.ToList();

            _logger.LogDebug("Current players per PrintInfo results ({playerCount}):", currentPlayersOnServer.Count);
            for (var i = 0; i < currentPlayersOnServer.Count; i++)
            {
                _logger.LogDebug("  {index}: {steamId} - {name}", i, currentPlayersOnServer[i].SteamId, currentPlayersOnServer[i].Name);
            }

            var discordUsersToMove = new List<(ISocketGuildUserWrapper user, ISocketVoiceChannelWrapper intendedChannel)>();

            var currentlyPlayingSteamIds = currentPlayersOnServer
            .Select(p => p.SteamId)
            .ToList();

            var currentPlayerMappings = _settings.UserMappings
                .Where(d => currentlyPlayingSteamIds.Intersect(d.SteamIds, StringComparer.CurrentCultureIgnoreCase).Any())
                .ToList();

            _logger.LogDebug("Current players found in mapping data ({playerCount}):", currentPlayerMappings.Count);
            for (var i = 0; i < currentPlayerMappings.Count; i++)
            {
                _logger.LogDebug("  {index}: {steamIds} - {discordId} - {name}",
                    i, string.Join(",", currentPlayerMappings[i].SteamIds), currentPlayerMappings[i].DiscordId, currentPlayerMappings[i].Name);
            }

            var allSteamIdsFromUserMappings = _settings.UserMappings.SelectMany(um => um.SteamIds).ToList();
            var missingSteamMappings = currentPlayersOnServer
                .Where(p => !allSteamIdsFromUserMappings.Contains(p.SteamId, StringComparer.CurrentCultureIgnoreCase))
                .ToList();

            var currentPlayerDiscordSnowflakes = currentPlayerMappings
                .Select(p => p.DiscordId)
                .ToList();

            var discordAccountsForCurrentPlayers = guild.Users
                .Where(u => currentPlayerDiscordSnowflakes.Contains(u.Id))
                .ToList();

            _logger.LogDebug("Discord accounts found from mappings ({matchCount}):", discordAccountsForCurrentPlayers.Count);
            for (var i = 0; i < discordAccountsForCurrentPlayers.Count; i++)
            {
                _logger.LogDebug("  {index}: {discordId} - {username}",
                    i, discordAccountsForCurrentPlayers[i].Id, discordAccountsForCurrentPlayers[i].Username);
            }

            var discordSnowflakesInVoice = usersInPrimaryChannel.Select(u => u.Id)
                .Concat(usersInSecondaryChannel.Select(u => u.Id))
                .ToList();

            var missingDiscordMappings = discordAccountsForCurrentPlayers.Where(d =>
                    !discordSnowflakesInVoice.Contains(d.Id))
                .ToList();

            if (missingSteamMappings.Any())
            {
                _logger.LogDebug("Current Steam users MISSING from mapping ({missingUserCount}):", missingSteamMappings.Count);
                for (var i = 0; i < missingSteamMappings.Count; i++)
                {
                    result.UnmappedSteamUsers.Add(new UnmappedSteamUser(missingSteamMappings[i].Name, missingSteamMappings[i].SteamId));
                    _logger.LogDebug("  {index}: {steamId} - {name}", i, missingSteamMappings[i].SteamId, missingSteamMappings[i].Name);
                }
            }

            if (missingDiscordMappings.Any())
            {
                _logger.LogDebug("Current Discord users MISSING from mapping ({missingUserCount}):", missingDiscordMappings.Count);
                for (var i = 0; i < missingDiscordMappings.Count; i++)
                {
                    _logger.LogDebug("  {index}: {id} - \"{nickname}\" (\"{username}\")",
                        i,
                        missingDiscordMappings[i].Id,
                        missingDiscordMappings[i].Nickname,
                        missingDiscordMappings[i].Username);
                }
            }

            foreach (var discordAccount in discordAccountsForCurrentPlayers)
            {
                ISocketVoiceChannelWrapper currentVoiceChannel;

                if (usersInPrimaryChannel != null && usersInPrimaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = primaryVoiceChannel;

                    _logger.LogDebug("{username} ({id}) found in primary channel.", discordAccount.Username, discordAccount.Id);
                }
                else if (usersInSecondaryChannel != null && usersInSecondaryChannel.Any(u => u.Id == discordAccount.Id))
                {
                    currentVoiceChannel = secondaryVoiceChannel;

                    _logger.LogDebug("{username} ({id}) found in secondary channel.", discordAccount.Username, discordAccount.Id);
                }
                else
                {
                    _logger.LogDebug("Skipping {username} ({id}): not in voice chat.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                if (primaryVoiceChannel.Id != currentVoiceChannel.Id && secondaryVoiceChannel.Id != currentVoiceChannel.Id)
                {
                    _logger.LogDebug("Skipping {username} ({id}): not in a designated voice channel.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                var userMappingsFromDiscordId = _settings.UserMappings.Where(um => um.DiscordId == discordAccount.Id).ToList();
                if (!userMappingsFromDiscordId.Any())
                {
                    _logger.LogDebug("Skipping {username} ({id}): Couldn't find user mapping.",
                        discordAccount.Username, discordAccount.Id);
                    continue;
                }

                var allSteamIdsFromDiscordId = userMappingsFromDiscordId.SelectMany(s => s.SteamIds);
                var usersPrintInfo = printInfo.Players.FirstOrDefault(pi =>
                    allSteamIdsFromDiscordId.Any(sid => sid.Equals(pi.SteamId, StringComparison.CurrentCultureIgnoreCase)));
                if (usersPrintInfo == null)
                {
                    _logger.LogDebug("Skipping {username} ({id}): Couldn't find user's Steam ID ({userMappingSteamIds}) in PrintInfo results.",
                        discordAccount.Username, discordAccount.Id, string.Join(", ", allSteamIdsFromDiscordId));
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

                discordUsersToMove.Add((discordAccount, intendedChannel));
            }

            foreach (var (user, intendedChannel) in discordUsersToMove)
            {
                ISocketVoiceChannelWrapper currentVoiceChannel;

                if (usersInPrimaryChannel != null && usersInPrimaryChannel.Any(u => u.Id == user.Id))
                {
                    currentVoiceChannel = primaryVoiceChannel;
                }
                else if (usersInSecondaryChannel != null && usersInSecondaryChannel.Any(u => u.Id == user.Id))
                {
                    currentVoiceChannel = secondaryVoiceChannel;
                }
                else
                {
                    // Not in voice chat
                    continue;
                }

                if (currentVoiceChannel.Id != intendedChannel.Id)
                {
                    _logger.LogDebug("Moving {username} ({id}) to other voice channel (from {fromChannelName} to {toChannelName}).",
                        user.Username, user.Id,
                        currentVoiceChannel.Name, intendedChannel.Name);

                    await user.ModifyAsync(p => p.ChannelId = intendedChannel.Id);

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    result.MoveCount++;
                }
            }

            return result;
        }

        public async Task<ReuniteResult> RenuitePlayersAsync(IDiscordSocketClientWrapper client, ISocketGuildWrapper guild,
            CancellationToken cancellationToken)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (guild is null) throw new ArgumentNullException(nameof(guild));

            var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == guild.Id);
            if (guildSettings == null)
            {
                throw new Exception($"Unable to find guild setting with ID {guild.Id} in the configuration.");
            }

            var primaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Primary.Id);
            if (primaryVoiceChannel == null) throw new Exception("Bad primary channel ID in config.");
            var secondaryVoiceChannel = guild.GetVoiceChannel(guildSettings.Channels.Secondary.Id);
            if (secondaryVoiceChannel == null) throw new Exception("Bad secondary channel ID in config.");

            var result = new ReuniteResult();

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

            var usersInPrimaryChannel = getPrimaryChannelUsersTask.Result.ToList();
            var usersInSecondaryChannel = getSecondaryChannelUsersTask.Result.ToList();

            if (!usersInPrimaryChannel.Any() || !usersInSecondaryChannel.Any())
            {
                return result;
            }

            foreach (var user in usersInSecondaryChannel)
            {
                await user.ModifyAsync(p => p.ChannelId = primaryVoiceChannel.Id);
                await Task.Delay(TimeSpan.FromSeconds(1));

                result.MoveCount++;
            }

            return result;
        }
    }
}
