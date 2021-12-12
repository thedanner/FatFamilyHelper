using Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;
using System.Threading.Tasks;

namespace Left4DeadHelper.TeamSuggestions;

public interface ISteamWebApiCaller
{
    Task<GetUserStatsForGame> GetUserStatsForGameAsync(SteamId steamId, int appId);
}
