namespace Left4DeadHelper.Models
{
    public class DiscordSettings
    {
        public DiscordSettings()
        {
            BotToken = "";
            Prefixes = new char[0];
            Channels = new DiscordVoiceChannels();
        }

        public string BotToken { get; set; }
        public ulong GuildId { get; set; }
        public char[] Prefixes { get; set; }
        public DiscordVoiceChannels Channels { get; set; }
    }
}
