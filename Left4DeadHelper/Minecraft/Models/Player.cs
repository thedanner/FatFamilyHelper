using System.Text.Json.Serialization;

namespace Left4DeadHelper.Minecraft.Models;

public class Player
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
}
