﻿using System.Text.Json.Serialization;

namespace Left4DeadHelper.Minecraft.Models;

public class VersionPayload
{
    [JsonPropertyName("protocol")]
    public int Protocol { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
