using CoreRCON;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Handlers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Left4DeadHelper
{
    public class Program
    {
        public enum ExitCode
        {
            Success = 0,
            ErrorUnknown = 10,
            InvalidArgs = 20,
            ErrorException = 30,
        }

        public static async Task<int> Main(string[] args)
        {
            if (args.Length >= 1 && "--interactive".Equals(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return (int) await InteractiveMainAsync();
            }

            try
            {
                // When run as a service, the working directory is wrong,
                // and it can't be set frffom the service configuration.
                var exeLocation = Assembly.GetExecutingAssembly().Location;
                var exeDirectory = Path.GetDirectoryName(exeLocation);
                if (exeDirectory != null)
                {
                    Environment.CurrentDirectory = exeDirectory;
                }

                CreateHostBuilder(args).Build().Run();
            }
            catch (ObjectDisposedException ex) when (ex.ObjectName == "System.Net.Sockets.Socket") { }

            // TODO some thread is probably still running somewhere preventing a normal shutdown. Find out where.
            // Environment.Exit() doesn't even cut it here.
            Environment.ExitCode = 0;
            Process.GetCurrentProcess().Kill();
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .Build();

                    ConfigureServices(services, config);
                }).UseWindowsService();

            return hostBuilder;
        }

        public static async Task<ExitCode> InteractiveMainAsync()
        {
            ExitCode? exitCode = null;
            try
            {
                exitCode = await RunAsync();

                return exitCode.Value;
            }
            finally
            {
                LogManager.Shutdown();

                // TODO some thread is probably still running somewhere preventing a normal shutdown. Find out where.
                Environment.Exit((int)(exitCode ?? ExitCode.ErrorUnknown));
            }
        }

        public static async Task<ExitCode> RunAsync()
        {
            ILogger<Program>? logger = null;

            try
            {
                var config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .Build();

                var serviceCollection = new ServiceCollection();

                ConfigureServices(serviceCollection, config);

                var serviceProvider = serviceCollection.BuildServiceProvider();

                logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                var ctSource = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, e) =>
                {
                    logger.LogInformation("^C received. Shutting down.");

                    ctSource.Cancel();
                    e.Cancel = false;
                };

                var settings = serviceProvider.GetRequiredService<Settings>();
                var guildSettings = settings.DiscordSettings.GuildSettings.First();

                var mover = serviceProvider.GetRequiredService<IDiscordChatMover>();

                using var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
                using var commandHandler = serviceProvider.GetRequiredService<CommandHandler>();

                await commandHandler.InstallCommandsAsync();

                var wrappedClient = new DiscordSocketClientWrapper(client);

                var bootstrapper = serviceProvider.GetRequiredService<IDiscordConnectionBootstrapper>();
                await bootstrapper.StartAsync(wrappedClient, ctSource.Token);

                using var rcon = serviceProvider.GetRequiredService<IRCONWrapper>();

                await rcon.ConnectAsync();

                var wrappedGuild = new SocketGuildWrapper(client.GetGuild(guildSettings.Id));

                var moveCount = await mover.MovePlayersToCorrectChannelsAsync(
                    rcon, wrappedClient, wrappedGuild, ctSource.Token);

                return ExitCode.Success;
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Error :(");

                var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                var textNodes = template.GetElementsByTagName("text");
                textNodes[0].InnerText = string.Join(Environment.NewLine,
                    "Error running Left4DeadHelper",
                    e.Message,
                    "See logs for details.");

                var toast = new ToastNotification(template);

                var notifier = ToastNotificationManager.CreateToastNotifier("Left4DeadHelper");
                notifier.Show(toast);

                return ExitCode.ErrorException;
            }
        }

        public static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration config)
        {
            var settings = config.Get<Settings>();

            if (settings.DiscordSettings.Prefixes.Length == 0)
            {
                throw new Exception("At least one prefix must be set in DiscordSettings.Prefixes.");
            }

            serviceCollection.AddSingleton(settings);

            serviceCollection.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                loggerBuilder.AddNLog(config);
            });

            serviceCollection.AddHostedService<Worker>()
                .Configure<EventLogSettings>(config =>
                {
                    config.LogName = "Left4DeadHelper";
                    config.SourceName = "Left4DeadHelper - Discord Bot";
                });

            // This type is thread-safe per MSDN.
            serviceCollection.AddSingleton<RNGCryptoServiceProvider>();

            serviceCollection.AddTransient<IDiscordConnectionBootstrapper, DiscordConnectionBootstrapper>();

            serviceCollection.AddTransient<IDiscordChatMover, DiscordChatMover>();

            serviceCollection.AddSingleton<CommandService>();

            serviceCollection.AddSingleton(sp =>
            {
                return new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds
                        | GatewayIntents.GuildIntegrations
                        | GatewayIntents.GuildVoiceStates
                        | GatewayIntents.GuildPresences
                        | GatewayIntents.GuildMessages
                });
            });
            
            serviceCollection.AddSingleton<CommandHandler>();

            var serverInfo = settings.Left4DeadSettings.ServerInfo;

            serviceCollection.AddTransient(sp => new RCON(new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port), serverInfo.RconPassword));
            serviceCollection.AddTransient<IRCONWrapper, RCONWrapper>();
        }
    }
}
