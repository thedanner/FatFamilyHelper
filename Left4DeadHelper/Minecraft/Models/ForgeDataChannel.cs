using System.Text.Json.Serialization;

namespace Left4DeadHelper.Minecraft.Models;

public class ForgeDataChannel
{
    [JsonPropertyName("res")]
    public string Resource { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("required")]
    public bool Required { get; set; } = false;
}
