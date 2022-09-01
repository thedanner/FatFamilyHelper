using System.Text.Json.Serialization;

namespace Left4DeadHelper.Minecraft.Models;

public class ForgeDataMod
{
    [JsonPropertyName("modId")]
    public string ModId { get; set; } = "";

    [JsonPropertyName("modmarker")]
    public string ModMarker { get; set; } = "";
}
