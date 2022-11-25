using System.Text.Json.Serialization;

namespace FatFamilyHelper.TeamSuggestions.RestModels.ISteamUserStats;

public class Achievenemnt
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("achieved")]
    public int AchievedInt { get; set; }

    public bool Achieved => AchievedInt == 1;
}
