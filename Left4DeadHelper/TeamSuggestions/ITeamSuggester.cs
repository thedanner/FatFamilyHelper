using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.TeamSuggestions;

public interface ITeamSuggester
{
    Task<List<List<SteamId>>> GetStatsAsync(IReadOnlyCollection<string> steamIds);
}
