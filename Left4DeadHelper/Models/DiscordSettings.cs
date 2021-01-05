namespace Left4DeadHelper.Models
{
    public class DiscordSettings
    {
        public DiscordSettings()
        {
            BotToken = "";
            Prefixes = new char[0];
            GuildSettings = new GuildSettings[0];
        }

        public string BotToken { get; set; }
        public char[] Prefixes { get; set; }
        public GuildSettings[] GuildSettings { get; set; }
    }
}
