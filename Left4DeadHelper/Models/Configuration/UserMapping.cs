using System;
using System.Collections.Generic;
using System.Linq;

namespace Left4DeadHelper.Models.Configuration
{
    public class UserMapping
    {
        private readonly List<string> _steamIds;

        public UserMapping()
        {
            Name = "";

            _steamIds = new List<string>();
        }

        public string Name { get; set; }

        public ulong DiscordId { get; set; }

        public string SteamId
        {
            // This needs to be a public accessor because the settings framework skips it if it's not.
            [Obsolete("Use the SteamIds property.", true)]
            get => _steamIds.FirstOrDefault() ?? "<none>";
            set => _steamIds.Add(value);
        }

        public List<string> SteamIds
        {
            get => _steamIds.ToList(); // Copy so the underlying list isn't accidentally corrupted.
            set => _steamIds.AddRange(value);
        }

        public override string ToString() => $"{Name} [SteamIds:{string.Join(",", SteamIds)}, DiscordId:{DiscordId}]";
    }
}
