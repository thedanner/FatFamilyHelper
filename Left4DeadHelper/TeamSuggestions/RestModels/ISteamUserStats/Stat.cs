using System.Text.Json.Serialization;

namespace Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;

public class Stat
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}
