namespace Left4DeadHelper.Models.Configuration
{
    public class UserMapping : IDiscordUser, ISteamUser
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
