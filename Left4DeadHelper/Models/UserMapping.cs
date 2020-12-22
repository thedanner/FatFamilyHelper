namespace Left4DeadHelper.Models
{
    public class UserMapping
    {
        public UserMapping()
        {
            Name = "";
            SteamId = "";
        }

        public string Name { get; set; }

        public ulong DiscordId { get; set; }

        public string SteamId { get; set; }
    }
}
