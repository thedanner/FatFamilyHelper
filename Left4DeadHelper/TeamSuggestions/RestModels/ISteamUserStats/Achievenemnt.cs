﻿using System.Text.Json.Serialization;

namespace Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;

public class Achievenemnt
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("achieved")]
    public int AchievedInt { get; set; }

    public bool Achieved => AchievedInt == 1;
}
