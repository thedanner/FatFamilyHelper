using System.Text.Json.Serialization;

namespace FatFamilyHelper.Games.Minecraft.Models;

public class DescriptionPayload
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}
