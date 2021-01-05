namespace Left4DeadHelper.Models
{
    public class GuildSettings
    {
        public GuildSettings()
        {
            Channels = new DiscordVoiceChannels();
        }

        public ulong Id { get; set; }
        public DiscordVoiceChannels Channels { get; set; }
    }
}
