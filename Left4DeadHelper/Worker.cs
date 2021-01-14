using Discord;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Handlers;
using Left4DeadHelper.Wrappers.DiscordNet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDiscordConnectionBootstrapper _bootstrapper;

        // Singleton IDisposables
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly RNGCryptoServiceProvider _random;

        public Worker(
            ILogger<Worker> logger,
            IDiscordConnectionBootstrapper bootstrapper,
            // Singleton IDisposables
            DiscordSocketClient client, CommandHandler commandHandler, RNGCryptoServiceProvider random)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bootstrapper = bootstrapper ?? throw new ArgumentNullException(nameof(bootstrapper));

            // Singleton IDisposables
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commandHandler.InstallCommandsAsync();

                // Try every 15 seconds (4 times a minute) for 15 minutes.
                const int maxAttempts = 4 * 15;

                var attempts = 0;
                var retry = true;
                while (retry)
                {
                    try
                    {
                        await _bootstrapper.StartAsync(
                            new DiscordSocketClientWrapper(_client),
                            cancellationToken);

                        retry = false;
                    }
                    catch (HttpRequestException e)
                    {
                        if (attempts >= maxAttempts)
                        {
                            _logger.LogError("Out of retries; stopping.");
                            throw;
                        }

                        _logger.LogWarning(e, "SocketException while trying to connect. Sleeping for a bit.");

                        await Task.Delay(TimeSpan.FromSeconds(15));

                        attempts++;
                    }
                }
                
                _logger.LogInformation("Client ready; waiting for commands.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error :(. Exiting.");
                throw;
            }

            await base.StartAsync(cancellationToken);

            _logger.LogInformation("Startup complete at: {time}", DateTimeOffset.Now);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop requested at: {time}", DateTimeOffset.Now);

            try
            {
                await _client.SetStatusAsync(UserStatus.Offline);
            }
            catch { } // don't care, shutting down.

            // Clean up Singleton IDisposables.
            _commandHandler.Dispose();
            _client.Dispose();
            _random.Dispose();

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
