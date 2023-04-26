using System.Text.Json.Serialization;

namespace FatFamilyHelper.Games.Minecraft.Models;

public class VersionPayload
{
    [JsonPropertyName("protocol")]
    public int Protocol { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
