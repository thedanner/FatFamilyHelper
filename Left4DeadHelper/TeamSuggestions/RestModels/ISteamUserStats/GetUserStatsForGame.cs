using System.Text.Json.Serialization;

namespace Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;

public class GetUserStatsForGame
{
    [JsonPropertyName("playerstats")]
    public PlayerStats PlayerStats { get; set; } = new PlayerStats();
}
