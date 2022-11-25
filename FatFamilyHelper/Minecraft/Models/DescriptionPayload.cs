using System.Text.Json.Serialization;

namespace FatFamilyHelper.Minecraft.Models;

public class DescriptionPayload
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}
