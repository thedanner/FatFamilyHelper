using Discord;
using Left4DeadHelper.Helpers.Extensions;
using Left4DeadHelper.Models;
using Left4DeadHelper.Wrappers.DiscordNet;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Services
{
    public class DiscordConnectionBootstrapper : IDiscordConnectionBootstrapper
    {
        private readonly ILogger<DiscordChatMover> _logger;
        private readonly Settings _settings;

        public DiscordConnectionBootstrapper(ILogger<DiscordChatMover> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task StartAsync(IDiscordSocketClientWrapper client, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var readyComplete = new TaskCompletionSource<bool>();
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            client.Connected += async () => _logger.LogInformation("Discord client event: Connected");

            async Task initalReadyAsync() => await ReadyHandlerWithSignalAsync(readyComplete);
            client.Ready += initalReadyAsync;

            client.Disconnected += async (ex) => _logger.LogError(ex, "Discord client event: Disconnected");
            client.GuildAvailable += async (guild) => _logger.LogInformation("Discord client event: GuildAvailable");
            client.GuildMembersDownloaded += async (ex) => _logger.LogInformation("Discord client event: GuildMembersDownloaded");
            client.Log += async (logMessage) =>
            {
                var level = logMessage.Severity.ToLogLevel();

                _logger.Log(
                    level,
                    logMessage.Exception,
                    "Discord client event: Log: (Source: {0}): {1}",
                    logMessage.Source, logMessage.Message);
            };
            client.LoggedIn += async () => _logger.LogInformation("Discord client event: LoggedIn");
            client.LoggedOut += async () => _logger.LogInformation("Discord client event: LoggedOut");

            await client.LoginAsync(TokenType.Bot, _settings.DiscordSettings.BotToken);
            await client.StartAsync();

            await readyComplete.Task;

            client.Ready -= initalReadyAsync;
            client.Ready += async () =>
            {
                _logger.LogInformation("Discord client event: Ready");
            };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            await client.SetStatusAsync(UserStatus.Online);
            // TODO read commands/prefixes?
            await client.SetGameAsync(".l4d / !l4d2", type: ActivityType.Listening);
        }

        private Task ReadyHandlerWithSignalAsync(TaskCompletionSource<bool> readyComplete)
        {
            _logger.LogInformation("Discord client event: Ready");
            readyComplete.SetResult(true);
            return Task.FromResult(0);
        }
    }
}
