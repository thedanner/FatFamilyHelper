namespace Left4DeadHelper.Models
{
    public class DiscordVoiceChannels
    {
        public DiscordVoiceChannels()
        {
            Primary = new DiscordEntity();
            Secondary = new DiscordEntity();
        }

        public DiscordEntity Primary { get; set; }
        public DiscordEntity Secondary { get; set; }
    }
}
