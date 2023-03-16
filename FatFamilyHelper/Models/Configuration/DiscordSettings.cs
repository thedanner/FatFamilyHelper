using System;

namespace FatFamilyHelper.Models.Configuration;

public class DiscordSettings
{
    public DiscordSettings()
    {
        BotToken = "";
        GuildSettings = Array.Empty<GuildSettings>();
    }

    public string BotToken { get; set; }
    public GuildSettings[] GuildSettings { get; set; }
}
