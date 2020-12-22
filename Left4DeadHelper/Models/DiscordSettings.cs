using System.Collections.Generic;
using System.Linq;

namespace Left4DeadHelper.Models
{
    public class DiscordSettings
    {
        public DiscordSettings()
        {
            BotToken = "";
            Prefixes = new char[0];
            Channels = new Dictionary<string, DiscordEntity>();
        }

        public string BotToken { get; set; }
        public ulong GuildId { get; set; }
        public char[] Prefixes { get; set; }
        public Dictionary<string, DiscordEntity> Channels { get; set; }
    }
}
