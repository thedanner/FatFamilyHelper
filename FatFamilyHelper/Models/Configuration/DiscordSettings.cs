namespace FatFamilyHelper.Models.Configuration;

public class DiscordSettings
{
    public DiscordSettings()
    {
        BotToken = "";
        GuildSettings = new GuildSettings[0];
    }

    public string BotToken { get; set; }
    public GuildSettings[] GuildSettings { get; set; }
}
