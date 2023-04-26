﻿using System.Text.Json.Serialization;

namespace FatFamilyHelper.Games.Minecraft.Models;

public class Player
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
}
