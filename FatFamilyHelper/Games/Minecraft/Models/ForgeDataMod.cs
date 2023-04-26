using System.Text.Json.Serialization;

namespace FatFamilyHelper.Games.Minecraft.Models;

public class ForgeDataMod
{
    [JsonPropertyName("modId")]
    public string ModId { get; set; } = "";

    [JsonPropertyName("modmarker")]
    public string ModMarker { get; set; } = "";
}
