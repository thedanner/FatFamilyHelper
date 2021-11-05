using System.Collections.Generic;
using System.Linq;

namespace Left4DeadHelper.Models.Configuration
{
    public class UserMapping
    {
        public UserMapping()
        {
            Name = "";
            SteamIds = new List<string>();
        }

        public string Name { get; set; }

        public ulong DiscordId { get; set; }

        public string SteamId
        {
            private get => SteamIds.First() ?? "";
            set => SteamIds.Add(value);
        }
        public IList<string> SteamIds { get; set; }

        public override string ToString() => $"{Name} [SteamId:{SteamId}, DiscordId:{DiscordId}]";
    }
}
