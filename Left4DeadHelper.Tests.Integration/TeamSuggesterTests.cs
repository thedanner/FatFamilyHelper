using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.TeamSuggestions;
using Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Left4DeadHelper.Tests.Integration;

[TestFixture]
[Explicit("Run manually")]
public class TeamSuggesterTests
{
    private IServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.AddDebug());

        serviceCollection.AddSingleton(new HttpClient());

        serviceCollection.AddTransient(sp => config.Get<Settings>());
        //serviceCollection.AddTransient<ISteamWebApiAccessKeyProvider, SteamWebApiAccessKeyProvider>();
        serviceCollection.AddTransient<ISteamWebApiCaller, SteamWebApiCaller>();
        serviceCollection.AddTransient<ITeamSuggester, TeamSuggester>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public void TestDeserialize()
    {
        const string payload = @"{""playerstats"": {
        ""steamID"": ""76561197969591558"",
        ""gameName"": """",
        ""achievements"": [{
            ""name"": ""ACH_HONK_A_CLOWNS_NOSE"",
            ""achieved"": 1
        }, {
            ""name"": ""ACH_INCENDIARY_CLOWN_POSSE"",
            ""achieved"": 1
        }, {
            ""name"": ""ACH_MELEE_A_CHARGER"",
            ""achieved"": 1
        }],
        ""stats"": [{
            ""name"": ""Stat.GamesPlayed.Total"",
            ""value"": 1905
        }, {
            ""name"": ""Stat.FinaleFinished.Total"",
            ""value"": 214
        }, {
            ""name"": ""Stat.TotalPlayTime.Total"",
            ""value"": 6433348
        }]
    }
}";
        var result = JsonSerializer.Deserialize<GetUserStatsForGame>(payload);
    }

    [Test]
    public async Task TestTeamSuggester()
    {
        var suggester = _serviceProvider.GetRequiredService<ITeamSuggester>();

        var steamIds = new[] {
                "STEAM_1:0:4662915", // TheDanner
                "STEAM_1:1:413562", // Lonegun
                "STEAM_1:1:6063400", // Shane/FilthyCausal
                "STEAM_1:1:27058429", // Nushaa
                
                "STEAM_1:0:18975559", // President
                "STEAM_0:1:25659798", // Dub
                "STEAM_1:1:3932392", // Novocaine
                "STEAM_0:0:9264804", // Carl / lamenralus
            };

        var teams = await suggester.GetStatsAsync(steamIds);
    }
}
