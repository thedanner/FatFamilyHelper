using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Left4DeadHelper.Tests.Integration
{
    [TestFixture]
    [Explicit("Run manually")]
    public class DiscordIntegrationTests
    {
        private Settings _settings;

        private string _botToken;
        private ulong _guildId;
        private ulong _primaryChannelId;
        private ulong _secondaryChannelId;

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = config.Get<Settings>();

            _botToken = _settings.DiscordSettings.BotToken;
            var guildSettings = _settings.DiscordSettings.GuildSettings.First();
            _guildId = guildSettings.Id;

            _primaryChannelId = guildSettings.Channels.Primary.Id;
            _secondaryChannelId = guildSettings.Channels.Secondary.Id;
        }


        [Test]
        public async Task HowDoEmbedsWork()
        {
            using var client = await GetClientAsync();

            var guild = client.GetGuild(_guildId);
            var channel = guild.GetTextChannel(816907545280905266);
            var messages = (await channel.GetMessagesAsync(100)
                   .FlattenAsync())
                   .ToList();

            if (messages.Any())
            {
                //if (messages.Count == BatchSize)
                //{
                //    var olderMessages = (await simpleChannel.GetMessagesAsync(messages.Last(), Direction.Before, BatchSize)
                //        .FlattenAsync())
                //        .ToList();
                //
                //    ProcessListAsync(olderMessages);
                //}

                var messagesToDelete = GetMessagesWithExpiredCodes(messages);

                // Only messages < 14 days old can be bulk deleted.
                var bulkDeletableMessages = new List<IMessage>();
                var singleDeletableMessages = new List<IMessage>();

                foreach (var message in messagesToDelete)
                {
                    var bulkDeleteCutoff = DateTimeOffset.Now.AddMinutes(5).AddDays(-14);

                    if (message.Timestamp >= bulkDeleteCutoff)
                    {
                        bulkDeletableMessages.Add(message);
                    }
                    else
                    {
                        singleDeletableMessages.Add(message);
                    }
                }

                if (bulkDeletableMessages.Any())
                {
                    await channel.DeleteMessagesAsync(bulkDeletableMessages);
                    await Task.Delay(250);
                }
                
                foreach (var singleDeletableMessage in singleDeletableMessages)
                {
                    await channel.DeleteMessageAsync(singleDeletableMessage);
                    await Task.Delay(250);
                }
            }
        }

        private static readonly Regex ExpiresRegex = new Regex(
            @"Expires: (?<expires>(?<date>\d{1,2}) (?<month>[a-zA-Z]{3}) (?<time>(?<hours>\d{1,2}):(?<minutes>\d{2}))) (?<timezone>[a-zA-Z]{3})",
            RegexOptions.Multiline);
        private List<IMessage> GetMessagesWithExpiredCodes(List<IMessage> messages)
        {
            var messagesToDelete = new List<IMessage>();

            foreach (var message in messages)
            {
                if (message.Author.IsBot
                    && message.Embeds.Any())
                {
                    var embed = message.Embeds.First();
                    if (!string.IsNullOrEmpty(embed.Description))
                    {
                        // Look for:
                        // Expires: 24 JUN 15:00 UTC

                        var match = ExpiresRegex.Match(embed.Description);
                        if (match.Success
                            &&  DateTimeOffset.TryParseExact(
                                    match.Groups["expires"].Value,
                                    "d MMM H:mm",
                                    CultureInfo.CurrentCulture,
                                    DateTimeStyles.AssumeUniversal,
                                    out var givenExpiry))
                        {
                            if (!"UTC".Equals(match.Groups["timezone"].Value, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Debug.WriteLine("Non-UTC expiration found for message with ID {0}.", message.Id);
                                continue;
                            }

                            // Handles the case where the assumed year is incorrect.
                            // E.g., a code is posted in December that expires in January. If the year is always
                            // assumed to be current, that code will look invalid immediately.
                            // To fix this, if the expiration is before the message post date, we just add a year.
                            if (givenExpiry < message.Timestamp)
                            {
                                givenExpiry = givenExpiry.AddYears(1).AddDays(1);
                            }

                            if (givenExpiry <= DateTimeOffset.Now)
                            {
                                messagesToDelete.Add(message);
                            }
                        }
                    }
                }
            }

            return messagesToDelete;
        }

        [Test]
        public async Task Test()
        {
            using var client = await GetClientAsync();

            var guild = client.GetGuild(_guildId);

            var primaryChannel = guild.GetVoiceChannel(_primaryChannelId);
            var secondaryChannel = guild.GetVoiceChannel(_secondaryChannelId);
        }

        [Test]
        public async Task GetRoles()
        {
            using var client = await GetClientAsync();

            var guild = client.GetGuild(_guildId);
            var allRolesSorted = new List<SocketRole>(guild.Roles.OrderByDescending(r => r.Position));

            var gradientRoles = new List<SocketRole>();

            var adding = false;

            foreach (var role in allRolesSorted)
            {
                if (role.Name == "Parents") adding = true;

                if (adding)
                {
                    gradientRoles.Add(role);
                }

                if (role.Name == "Titanfall Pilots")
                {
                    adding = false;
                    break;
                }
            }
        }

        private async Task<DiscordSocketClient> GetClientAsync()
        {
            using var client = new DiscordSocketClient();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, _botToken);
            await client.StartAsync();

            var tsc = new TaskCompletionSource<bool>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            client.Ready += async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                tsc.SetResult(true);
            };

            await tsc.Task;

            return client;
        }
    }
}
