using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class DiscordSocketClientWrapper : BaseSocketClientWrapper, IDiscordSocketClientWrapper
    {
        private readonly DiscordSocketClient _discordSocketClient;

        public DiscordSocketClientWrapper(DiscordSocketClient discordSocketClient)
            :base(discordSocketClient)
        {
            _discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        }

        public DiscordSocketClient WrappedClient => _discordSocketClient;

        public virtual IReadOnlyCollection<SocketGroupChannel> GroupChannels => _discordSocketClient.GroupChannels;

        public virtual IReadOnlyCollection<SocketDMChannel> DMChannels => _discordSocketClient.DMChannels;

        public virtual int ShardId => _discordSocketClient.ShardId;

        public virtual event Func<Task> Ready
        {
            add { _discordSocketClient.Ready += value; }
            remove { _discordSocketClient.Ready -= value; }
        }

        public virtual event Func<Task> Connected
        {
            add { _discordSocketClient.Connected += value; }
            remove { _discordSocketClient.Connected -= value; }
        }

        public virtual event Func<Exception, Task> Disconnected
        {
            add { _discordSocketClient.Disconnected += value; }
            remove { _discordSocketClient.Disconnected -= value; }
        }

        public virtual event Func<int, int, Task> LatencyUpdated
        {
            add { _discordSocketClient.LatencyUpdated += value; }
            remove { _discordSocketClient.LatencyUpdated -= value; }
        }

    }
}
