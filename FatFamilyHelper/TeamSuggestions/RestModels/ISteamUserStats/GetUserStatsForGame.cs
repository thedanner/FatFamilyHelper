using System.Text.Json.Serialization;

namespace FatFamilyHelper.TeamSuggestions.RestModels.ISteamUserStats;

public class GetUserStatsForGame
{
    [JsonPropertyName("playerstats")]
    public PlayerStats PlayerStats { get; set; } = new PlayerStats();
}
