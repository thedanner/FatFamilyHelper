using CoreRCON;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Handlers;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

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
            try
            {
                // When run as a service, the working directory is wrong,
                // and it can't be set from the service configuration.
                var exeLocation = Assembly.GetExecutingAssembly().Location;
                var exeDirectory = Path.GetDirectoryName(exeLocation);
                if (exeDirectory != null)
                {
                    Environment.CurrentDirectory = exeDirectory;
                }

                if (args.Length >= 2
                    && "--task".Equals(args[0], StringComparison.CurrentCultureIgnoreCase))
                {
                    var ctSource = new CancellationTokenSource();
                    Console.CancelKeyPress += (sender, args) =>
                    {
                        ctSource.Cancel();
                        args.Cancel = true;
                    };

                    await RunTaskAsync(args[1], ctSource.Token);
                }
                else
                {
                    CreateHostBuilder(args).Build().Run();
                }
            }
            catch (ObjectDisposedException ex) when (ex.ObjectName == "System.Net.Sockets.Socket") { }
            catch (Exception ex)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddNLog();
                    builder.AddConsole();
                });
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error starting service.");
            }

            // TODO some thread is probably still running somewhere preventing a normal shutdown. Find out where.
            // Environment.Exit() doesn't even cut it here.
            Environment.ExitCode = 0;
            Process.GetCurrentProcess().Kill();
            return 0;
        }

        private static async Task RunTaskAsync(string taskName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                throw new ArgumentException($"'{nameof(taskName)}' cannot be null or whitespace.", nameof(taskName));
            }

            var config = CreateConfiguration();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection, config);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var taskResolver = serviceProvider.GetRequiredService<Func<string, ITask>>();
            var task = taskResolver(taskName);

            // DiscordSocketClient is a singleton. Using 'using' is safe because we're stopping once this task is complete.
            using var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            var clientWrapper = new DiscordSocketClientWrapper(client);

            var connectionBootstrapper = serviceProvider.GetRequiredService<IDiscordConnectionBootstrapper>();

            await connectionBootstrapper.StartAsync(clientWrapper, cancellationToken);

            var settings = serviceProvider.GetRequiredService<Settings>();

            var taskSettings = settings.TaskSettings[taskName];

            await task.RunTaskAsync(client, taskSettings, cancellationToken);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = CreateConfiguration();
                    ConfigureServices(services, config);
                }).UseWindowsService();

            return hostBuilder;
        }

        private static IConfigurationRoot CreateConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return config;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration config)
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
                loggerBuilder.SetMinimumLevel(LogLevel.Debug);
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

            serviceCollection.AddTransient<ISprayModuleCommandResolver, SprayModuleCommandResolver>();

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
                        | GatewayIntents.GuildMessageReactions
                });
            });
            
            serviceCollection.AddSingleton<CommandAndEventHandler>();

            var serverInfo = settings.Left4DeadSettings.ServerInfo;

            serviceCollection.AddTransient(sp => new RCON(new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port), serverInfo.RconPassword));
            serviceCollection.AddTransient<IRCONWrapper, RCONWrapper>();

            // More specific handlers
            var allLoadedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToList()
                .AsReadOnly();

            BindEventHandlers(allLoadedTypes, serviceCollection);

            BindTasks(allLoadedTypes, serviceCollection);

            BindModulesWithHelpSupport(allLoadedTypes, serviceCollection);
        }

        private static void BindEventHandlers(IReadOnlyList<Type> allLoadedTypes, IServiceCollection serviceCollection)
        {
            var handlerTypes = new List<Type>();
            var eventInterfaceTypes = new List<Type>();

            // Do all the filtering and sorting in one pass over the loaded types list.
            foreach (var type in allLoadedTypes)
            {
                if (typeof(IHandleDiscordEvents).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    handlerTypes.Add(type);
                }
                else if (type != typeof(IHandleDiscordEvents)
                    && typeof(IHandleDiscordEvents).IsAssignableFrom(type)
                    && type.IsInterface)
                {
                    eventInterfaceTypes.Add(type);
                }
            }

            foreach (var handlerType in handlerTypes)
            {
                foreach (var implmenetedHandlerInterface in handlerType.GetInterfaces().Intersect(eventInterfaceTypes))
                {
                    serviceCollection.AddTransient(implmenetedHandlerInterface, handlerType);
                }
            }
        }

        private static void BindTasks(IReadOnlyList<Type> allLoadedTypes, IServiceCollection serviceCollection)
        {
            var handlerTypes = new List<Type>();

            // Do all the filtering and sorting in one pass over the loaded types list.
            foreach (var type in allLoadedTypes)
            {
                if (typeof(ITask).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    handlerTypes.Add(type);
                }
            }

            var duplicatedNames = handlerTypes
                .GroupBy(t => t.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicatedNames.Any())
            {
                // If this comes up, we may need to create an attribute that allows a custom name to be specified or something.
                throw new Exception(
                    "Found tasks with duplicate names (note that only the class name and no part of the namespace is " +
                    "used as the task name, and therefore class names must be unique: " +
                    string.Join(", ", duplicatedNames));
            }

            foreach (var handlerType in handlerTypes)
            {
                serviceCollection.AddTransient(handlerType);
            }

            // Technique borrowed from
            // https://dejanstojanovic.net/aspnet/2018/december/registering-multiple-implementations-of-the-same-interface-in-aspnet-core/
            serviceCollection.AddTransient<Func<string, ITask>>(serviceProvider => key =>
            {
                var type = handlerTypes.FirstOrDefault(t => t.Name == key);
                if (type == null)
                {
                    throw new Exception($"Couldn't find an ITask with class name of '{key}'.");
                }
                var task = (ITask) serviceProvider.GetRequiredService(type);
                return task;
            });
        }

        private static void BindModulesWithHelpSupport(ReadOnlyCollection<Type> allLoadedTypes, IServiceCollection serviceCollection)
        {
            var modulesWithHelpSupport = new List<Type>();

            // Do all the filtering and sorting in one pass over the loaded types list.
            foreach (var type in allLoadedTypes)
            {
                if (!type.IsInterface && !type.IsAbstract
                    && typeof(ModuleBase<SocketCommandContext>).IsAssignableFrom(type)
                    && typeof(ICommandModule).IsAssignableFrom(type))
                {
                    modulesWithHelpSupport.Add(type);
                }
            }

            foreach (var implmenetedHandlerInterface in modulesWithHelpSupport)
            {
                serviceCollection.AddTransient(typeof(ICommandModule), implmenetedHandlerInterface);
            }
        }
    }
}
