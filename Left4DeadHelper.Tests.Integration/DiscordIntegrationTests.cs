using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
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
            _guildId = _settings.DiscordSettings.GuildId;

            _primaryChannelId = _settings.DiscordSettings.Channels["primary"].Id;
            _secondaryChannelId = _settings.DiscordSettings.Channels["secondary"].Id;
        }

        [Test]
        public async Task Test()
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

            var guild = client.GetGuild(_guildId);

            var primaryChannel = guild.GetVoiceChannel(_primaryChannelId);
            var secondaryChannel = guild.GetVoiceChannel(_secondaryChannelId);
        }

        [Test]
        public async Task GetRoles()
        {
            using var client = new DiscordSocketClient();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, _settings.DiscordSettings.BotToken);
            await client.StartAsync();

            var tsc = new TaskCompletionSource<bool>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            client.Ready += async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                tsc.SetResult(true);
            };

            await tsc.Task;

            var guild = client.GetGuild(_guildId);
            var roles = guild.Roles;
        }
    }
}
