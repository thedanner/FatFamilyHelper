using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.TeamSuggestions;

public class TeamSuggester : ITeamSuggester
{
    private const int AppIdLeft4Dead2 = 550;
    private const string VersusStatsGameWon = "Stat.GamesWon.Versus";
    private const string VersusStatsGameLost = "Stat.GamesLost.Versus";

    private readonly ISteamWebApiCaller _steamCaller;

    public TeamSuggester(ISteamWebApiCaller steamCaller)
    {
        _steamCaller = steamCaller;
    }

    public async Task<List<List<SteamId>>> GetStatsAsync(IReadOnlyCollection<string> steamIds)
    {
        var maxTeamSize = steamIds.Count / 2;

        var statsTasks = steamIds.Select(steamId =>
            _steamCaller.GetUserStatsForGameAsync(SteamId.Create(steamId), AppIdLeft4Dead2));

        var statsResults = await Task.WhenAll(statsTasks);

        var stats = statsTasks.Select(task => new StatsResult(
                task.Result!.PlayerStats.SteamId,
                (int)task.Result.PlayerStats.Stats.First(s => s.Name == VersusStatsGameWon).Value,
                (int)task.Result.PlayerStats.Stats.First(s => s.Name == VersusStatsGameLost).Value
            )).ToList();

        var totalGames = stats.Aggregate(0, (src, r) => src + r.WinCount + r.LossCount);

        var orderedWeights = stats
            .OrderBy(s => s.GetWeightedValue(totalGames))
            .ToList();

        var team1Players = new List<StatsResult>(maxTeamSize);
        var team1TotalWeight = 0.0;
        var team2Players = new List<StatsResult>(maxTeamSize);
        var team2TotalWeight = 0.0;

        team1Players.Add(orderedWeights[0]);
        team1TotalWeight += orderedWeights[0].GetWeightedValue(totalGames);

        foreach (var player in orderedWeights.Skip(1))
        {
            if (team1TotalWeight < team2TotalWeight && team1Players.Count < maxTeamSize)
            {
                team1Players.Add(player);
                team1TotalWeight += player.GetWeightedValue(totalGames);
            }
            else
            {
                team2Players.Add(player);
                team2TotalWeight += player.GetWeightedValue(totalGames);
            }
        }

        return new()
        {
            team1Players.Select(p => p.SteamId).ToList(),
            team2Players.Select(p => p.SteamId).ToList()
        };
    }
}

public readonly record struct StatsResult(SteamId SteamId, int WinCount, int LossCount)
{
    public int TotalGames => WinCount + LossCount;
    public double WinRate => (double)WinCount / TotalGames;
    public double GetWeightedValue(int totalGames) => WinRate * (TotalGames / totalGames);
}

public record class Tree<T>(Tree<T>? Left, Tree<T>? Right, T Data) where T : struct;
