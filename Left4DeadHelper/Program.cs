using CoreRCON;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Handlers;
using Left4DeadHelper.Discord.Interfaces;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Scheduling;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using NLog.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

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

        public static int Main(string[] args)
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

                CreateHostBuilder(args).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(hostingContext, config, args))
                .ConfigureServices(ConfigureServices)
                .UseWindowsService();

            return hostBuilder;
        }

        private static void ConfigureAppConfiguration(HostBuilderContext hostContext, IConfigurationBuilder config, string[] args)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#file-configuration-provider
            config.Sources.Clear();

            var env = hostContext.HostingEnvironment;

            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "LEFT4DEADHELPER_")
                .AddCommandLine(args);
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection serviceCollection)
        {
            var config = hostContext.Configuration;
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
                        | GatewayIntents.DirectMessages
                        | GatewayIntents.DirectMessageReactions
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

            ConfigureScheduler(serviceCollection, config);
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

        private static void ConfigureScheduler(IServiceCollection serviceCollection, IConfiguration config)
        {
            var settings = config.Get<Settings>();
            var logger = serviceCollection.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            var tasksGroupedByName = settings.Tasks.GroupBy(t => t.Name);
            var replicatedNamed = tasksGroupedByName.Where(g => g.Count() > 1);
            if (replicatedNamed.Any())
            {
                throw new Exception("Multiple tasks were found with each of the following names: " +
                    string.Join(", ", replicatedNamed.Select(n => $"\"{n.Key}\"")));
            }

            serviceCollection.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 1;
                });

                foreach (var taskSetting in settings.Tasks)
                {
                    if (string.IsNullOrEmpty(taskSetting.Name))
                    {
                        throw new Exception($"A task is missing a {nameof(taskSetting.Name)}.");
                    }

                    logger.LogInformation("Creating task in scheduler for \"{taskName}\".", taskSetting.Name);

                    var jobKey = new JobKey("JobRunner");

                    q.AddJob<JobRunner>(jobKey, j => j
                        .WithDescription("Job runner")
                    );

                    if (string.IsNullOrEmpty(taskSetting.ClassName))
                    {
                        throw new Exception($"The task definition for the task \"{0}\" is missing {nameof(taskSetting.ClassName)}.");
                    }

                    var jobData = new JobDataMap();
                    jobData.Put(JobRunner.KeyTaskName, taskSetting.Name);
                    jobData.Put(JobRunner.KeyClassName, taskSetting.ClassName);

                    q.AddTrigger(t => t
                        .WithIdentity(taskSetting.Name)
                        .WithCronSchedule(taskSetting.CronSchedule)
                        .ForJob(jobKey)
                        .UsingJobData(jobData)
                    );
                }
            });
            serviceCollection.AddTransient<JobRunner>();

            serviceCollection.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
