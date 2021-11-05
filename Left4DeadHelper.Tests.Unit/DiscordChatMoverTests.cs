using Discord;
using FakeItEasy;
using Left4DeadHelper.Models;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Rcon;
using Left4DeadHelper.Services;
using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Tests.Unit
{
    [TestFixture]
    public class DiscordChatMoverTests
    {
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .Build();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection, config);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.SetMinimumLevel(LogLevel.Debug);
                loggerBuilder.AddNLog(config);
            });
        }

        [Test]
        public async Task MovePlayersToCorrectChannelsAsync_MovesNeeded_MovesHappened()
        {
            // Arrange
            var rcon = A.Fake<RCONWrapper>();
            var client = A.Fake<DiscordSocketClientWrapper>();
            var tcs = new TaskCompletionSource<bool>();
            var ctSource = new CancellationTokenSource();
            var settings = new Settings
            {
                DiscordSettings = new DiscordSettings
                {
                    BotToken = "THE_BOT_TOKEN",
                    Prefixes = new char[] { '!', '.' },
                    GuildSettings = new GuildSettings[]
                    {
                        new GuildSettings
                        {
                        Id = 42,
                        Channels = new DiscordVoiceChannels
                        {
                            Primary = new DiscordEntity
                            {
                                Id = 10,
                                Name = "Channel 1",
                            },
                            Secondary = new DiscordEntity
                            {
                                Id = 20,
                                Name = "Channel 2",
                            }
                        }
                        }
                    }
                },
                UserMappings = new UserMapping[]
                {
                    new UserMapping
                    {
                        Name = "Player 100",
                        SteamId = "STEAM_0:0_100",
                        DiscordId = 100,
                    },
                    new UserMapping
                    {
                        Name = "Player 200",
                        SteamId = "STEAM_0:0_200",
                        DiscordId = 200,
                    },
                    new UserMapping
                    {
                        Name = "Player 300",
                        SteamId = "STEAM_0:0_300",
                        DiscordId = 300,
                    },
                    new UserMapping
                    {
                        Name = "Player 400",
                        SteamId = "STEAM_0:0_400",
                        DiscordId = 400,
                    },
                    new UserMapping
                    {
                        Name = "Player 500",
                        SteamId = "STEAM_0:0_500",
                        DiscordId = 500,
                    },
                    new UserMapping
                    {
                        Name = "Player 600",
                        SteamId = "STEAM_0:0_600",
                        DiscordId = 600,
                    },
                    new UserMapping
                    {
                        Name = "Player 700",
                        SteamId = "STEAM_0:0_700",
                        DiscordId = 700,
                    },
                    new UserMapping
                    {
                        Name = "Player 800",
                        SteamId = "STEAM_0:0_800",
                        DiscordId = 800,
                    }
                },
            };
            var guildSettings = settings.DiscordSettings.GuildSettings.First();

            var guild = A.Fake<ISocketGuildWrapper>();

            A.CallTo(() => guild.Id)
                .Returns(guildSettings.Id);

            A.CallTo(() => client.GetGuild(guildSettings.Id))
                .Returns(guild);
            A.CallTo(() => rcon.SendCommandAsync<SmCvar>("sm_cvar mp_gamemode"))
                .Returns(new SmCvar
                {
                    Name = "mp_gamemode",
                    Value = "versus"
                });
            A.CallTo(() => rcon.SendCommandAsync<PrintInfo>("sm_printinfo"))
                .Returns(new PrintInfo
                {
                    Players = new List<PrintInfoPlayer>
                    {
                        // Survivor team
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[0].Name,
                            ClientIndex = 1,
                            SteamId = settings.UserMappings[0].SteamIds.First(),
                            TeamIndex = 2,
                            TeamName = "Survivor",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[1].Name,
                            ClientIndex = 2,
                            SteamId = settings.UserMappings[1].SteamIds.First(),
                            TeamIndex = 2,
                            TeamName = "Survivor",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[2].Name,
                            ClientIndex = 3,
                            SteamId = settings.UserMappings[2].SteamIds.First(),
                            TeamIndex = 2,
                            TeamName = "Survivor",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[3].Name,
                            ClientIndex = 4,
                            SteamId = settings.UserMappings[3].SteamIds.First(),
                            TeamIndex = 2,
                            TeamName = "Survivor",
                        },

                        // Infected team
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[4].Name,
                            ClientIndex = 5,
                            SteamId = settings.UserMappings[4].SteamIds.First(),
                            TeamIndex = 3,
                            TeamName = "Infected",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[5].Name,
                            ClientIndex = 6,
                            SteamId = settings.UserMappings[5].SteamIds.First(),
                            TeamIndex = 3,
                            TeamName = "Infected",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[6].Name,
                            ClientIndex = 7,
                            SteamId = settings.UserMappings[6].SteamIds.First(),
                            TeamIndex = 3,
                            TeamName = "Infected",
                        },
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[7].Name,
                            ClientIndex = 8,
                            SteamId = settings.UserMappings[7].SteamIds.First(),
                            TeamIndex = 3,
                            TeamName = "Infected",
                        },
                    }
                });

            // General / Survivor voice channel
            var primaryChannelId = guildSettings.Channels.Primary.Id;
            var primaryVoiceChannel = A.Fake<ISocketVoiceChannelWrapper>(ob => ob.Named("Primary (General\\Survivor)"));
            var primaryRawVoiceChannel = A.Fake<IVoiceChannel>();
            var primaryVoiceChannelUsers = 
            A.CallTo(() => primaryVoiceChannel.Id).Returns(primaryChannelId);
            A.CallTo(() => guild.GetVoiceChannel(primaryChannelId))
                .Returns(primaryVoiceChannel);
            A.CallTo(() => guild.GetVoiceChannelAsync(primaryChannelId, A<CacheMode>._, A<RequestOptions>._))
                .Returns(Task.FromResult(primaryRawVoiceChannel));

            // Infected voice channel
            var secondaryChannelId = guildSettings.Channels.Secondary.Id;
            var secondaryVoiceChannel = A.Fake<ISocketVoiceChannelWrapper>(ob => ob.Named("Secondary (Infected)"));
            var secondaryRawVoiceChannel = A.Fake<IVoiceChannel>();
            A.CallTo(() => secondaryVoiceChannel.Id).Returns(secondaryChannelId);
            A.CallTo(() => guild.GetVoiceChannel(secondaryChannelId))
                .Returns(secondaryVoiceChannel);
            A.CallTo(() => guild.GetVoiceChannelAsync(secondaryChannelId, A<CacheMode>._, A<RequestOptions>._))
                .Returns(Task.FromResult(secondaryRawVoiceChannel));

            // Maps to a user on the Infected team, but in the Survivor (primary) voice channel.
            var socketGuildUser1 = A.Fake<ISocketGuildUserWrapper>(ob => ob.Named("User 1 (on Infected team)"));
            A.CallTo(() => socketGuildUser1.Id)
                .Returns(settings.UserMappings[4].DiscordId);
            A.CallTo(() => socketGuildUser1.VoiceChannel)
                .Returns(primaryVoiceChannel);

            // Maps to a user on the Survivor team, but in the Infected voice channel.
            var socketGuildUser2 = A.Fake<ISocketGuildUserWrapper>(ob => ob.Named("User 2 (on Survivor team)"));
            A.CallTo(() => socketGuildUser2.Id)
                .Returns(settings.UserMappings[0].DiscordId);
            A.CallTo(() => socketGuildUser2.VoiceChannel)
                .Returns(secondaryVoiceChannel);

            A.CallTo(() => guild.Users)
                .Returns(new List<ISocketGuildUserWrapper>
                {
                    socketGuildUser1,
                    socketGuildUser2,
                }.AsReadOnly());

            // Set up the raw call results.
            var primaryChannelRawUser = A.Fake<IGuildUser>();
            A.CallTo(() => primaryChannelRawUser.Id)
                .Returns(socketGuildUser1.Id);
            A.CallTo(() => primaryRawVoiceChannel.GetUsersAsync(A<CacheMode>._, A<RequestOptions>._))
                .Returns(AsReadOnlyAsyncEnumerable(primaryChannelRawUser));

            var secondaryChannelRawUser = A.Fake<IGuildUser>();
            A.CallTo(() => secondaryChannelRawUser.Id)
                .Returns(socketGuildUser2.Id);
            A.CallTo(() => secondaryRawVoiceChannel.GetUsersAsync(A<CacheMode>._, A<RequestOptions>._))
                .Returns(AsReadOnlyAsyncEnumerable(secondaryChannelRawUser));

            var _mover = new DiscordChatMover(
                _serviceProvider.GetRequiredService<ILogger<DiscordChatMover>>(),
                settings);

            // Act
            await _mover.MovePlayersToCorrectChannelsAsync(rcon, client, guild, CancellationToken.None);

            // Assert
            A.CallTo(() => socketGuildUser1.ModifyAsync(A<Action<GuildUserProperties>>._, null))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => socketGuildUser2.ModifyAsync(A<Action<GuildUserProperties>>._, null))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task MovePlayersToCorrectChannelsAsync_DuplicateEntires_CorrectMovesHappened()
        {
            // Arrange
            var rcon = A.Fake<RCONWrapper>();
            var client = A.Fake<DiscordSocketClientWrapper>();
            var tcs = new TaskCompletionSource<bool>();
            var ctSource = new CancellationTokenSource();
            var settings = new Settings
            {
                DiscordSettings = new DiscordSettings
                {
                    BotToken = "THE_BOT_TOKEN",
                    Prefixes = new char[] { '!', '.' },
                    GuildSettings = new GuildSettings[]
                    {
                        new GuildSettings
                        {
                        Id = 42,
                        Channels = new DiscordVoiceChannels
                        {
                            Primary = new DiscordEntity
                            {
                                Id = 10,
                                Name = "Channel 1",
                            },
                            Secondary = new DiscordEntity
                            {
                                Id = 20,
                                Name = "Channel 2",
                            }
                        }
                        }
                    }
                },
                UserMappings = new UserMapping[]
                {
                    // Same Discord ID, different SteamIDs
                    // (e.g., lookup sites return the 1st steamid, 'rcon sm_printinfo' shows the 2nd steamid)
                    new UserMapping
                    {
                        Name = "Player 100",
                        SteamId = "STEAM_0:0_100",
                        DiscordId = 100,
                    },
                    new UserMapping
                    {
                        Name = "Player 100 #2",
                        SteamId = "STEAM_1:0_100",
                        DiscordId = 100,
                    },
                },
            };
            var guildSettings = settings.DiscordSettings.GuildSettings.First();

            var guild = A.Fake<ISocketGuildWrapper>();

            A.CallTo(() => guild.Id)
                .Returns(guildSettings.Id);

            A.CallTo(() => client.GetGuild(guildSettings.Id))
                .Returns(guild);
            A.CallTo(() => rcon.SendCommandAsync<SmCvar>("sm_cvar mp_gamemode"))
                .Returns(new SmCvar
                {
                    Name = "mp_gamemode",
                    Value = "versus"
                });
            A.CallTo(() => rcon.SendCommandAsync<PrintInfo>("sm_printinfo"))
                .Returns(new PrintInfo
                {
                    Players = new List<PrintInfoPlayer>
                    {
                        new PrintInfoPlayer
                        {
                            Name = settings.UserMappings[1].Name,
                            ClientIndex = 2,
                            SteamId = settings.UserMappings[1].SteamIds.First(),
                            TeamIndex = 3,
                            TeamName = "Infected",
                        },
                    }
                });

            // General / Survivor voice channel
            var primaryChannelId = guildSettings.Channels.Primary.Id;
            var primaryVoiceChannel = A.Fake<ISocketVoiceChannelWrapper>(ob => ob.Named("Primary (General\\Survivor)"));
            var primaryRawVoiceChannel = A.Fake<IVoiceChannel>();
            var primaryVoiceChannelUsers =
            A.CallTo(() => primaryVoiceChannel.Id).Returns(primaryChannelId);
            A.CallTo(() => guild.GetVoiceChannel(primaryChannelId))
                .Returns(primaryVoiceChannel);
            A.CallTo(() => guild.GetVoiceChannelAsync(primaryChannelId, A<CacheMode>._, A<RequestOptions>._))
                .Returns(Task.FromResult(primaryRawVoiceChannel));

            // Infected voice channel
            var secondaryChannelId = guildSettings.Channels.Secondary.Id;
            var secondaryVoiceChannel = A.Fake<ISocketVoiceChannelWrapper>(ob => ob.Named("Secondary (Infected)"));
            var secondaryRawVoiceChannel = A.Fake<IVoiceChannel>();
            A.CallTo(() => secondaryVoiceChannel.Id).Returns(secondaryChannelId);
            A.CallTo(() => guild.GetVoiceChannel(secondaryChannelId))
                .Returns(secondaryVoiceChannel);
            A.CallTo(() => guild.GetVoiceChannelAsync(secondaryChannelId, A<CacheMode>._, A<RequestOptions>._))
                .Returns(Task.FromResult(secondaryRawVoiceChannel));

            // Users is on the Infected team, but in the Survivor (primary) voice channel.
            var socketGuildUser = A.Fake<ISocketGuildUserWrapper>(ob => ob.Named("User (on Survivor team)"));
            A.CallTo(() => socketGuildUser.Id)
                .Returns(settings.UserMappings[0].DiscordId);
            A.CallTo(() => socketGuildUser.VoiceChannel)
                .Returns(primaryVoiceChannel);

            A.CallTo(() => guild.Users)
                .Returns(new List<ISocketGuildUserWrapper>
                {
                    socketGuildUser,
                }.AsReadOnly());

            // Set up the raw call results.
            var primaryChannelRawUser = A.Fake<IGuildUser>();
            A.CallTo(() => primaryChannelRawUser.Id)
                .Returns(socketGuildUser.Id);
            A.CallTo(() => primaryRawVoiceChannel.GetUsersAsync(A<CacheMode>._, A<RequestOptions>._))
                .Returns(AsReadOnlyAsyncEnumerable(primaryChannelRawUser));

            A.CallTo(() => secondaryRawVoiceChannel.GetUsersAsync(A<CacheMode>._, A<RequestOptions>._))
                .Returns(AsReadOnlyAsyncEnumerable<IGuildUser>());

            var _mover = new DiscordChatMover(
                _serviceProvider.GetRequiredService<ILogger<DiscordChatMover>>(),
                settings);

            // Act
            await _mover.MovePlayersToCorrectChannelsAsync(rcon, client, guild, CancellationToken.None);

            // Assert
            A.CallTo(() => socketGuildUser.ModifyAsync(A<Action<GuildUserProperties>>._, null))
                .MustHaveHappened(1, Times.Exactly);
        }

        private static IAsyncEnumerable<IReadOnlyCollection<T>> AsReadOnlyAsyncEnumerable<T>(params T[] values)
        {
            List<T> a = new List<T>(values);
            List<IReadOnlyCollection<T>> b = new List<IReadOnlyCollection<T>>(1)
            {
                a.AsReadOnly()
            };
            IAsyncEnumerable<IReadOnlyCollection<T>> c = b.ToAsyncEnumerable();
            return c;
        }
    }
}
