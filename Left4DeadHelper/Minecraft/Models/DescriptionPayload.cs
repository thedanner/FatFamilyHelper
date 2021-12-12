using System.Text.Json.Serialization;

namespace Left4DeadHelper.Minecraft.Models;

public class DescriptionPayload
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}
