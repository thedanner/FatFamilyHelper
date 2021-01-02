namespace Left4DeadHelper.Models
{
    public class Settings
    {
        public Settings()
        {
            DiscordSettings = new DiscordSettings();
            Left4DeadSettings = new Left4DeadSettings();
            UserMappings = new UserMapping[0];
        }

        public DiscordSettings DiscordSettings { get; set; }
        public Left4DeadSettings Left4DeadSettings { get; set;}
        public UserMapping[] UserMappings { get; set; }
    }
}
