using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet;

public interface IDiscordSocketClientWrapper : IBaseSocketClientWrapper, IDiscordClient, IDisposable
{
    DiscordSocketClient WrappedClient { get; }

    IReadOnlyCollection<SocketGroupChannel> GroupChannels { get; }
    IReadOnlyCollection<SocketDMChannel> DMChannels { get; }
    int ShardId { get; }
    event Func<Task> Ready;
    event Func<Task> Connected;
    event Func<Exception, Task> Disconnected;
    event Func<int, int, Task> LatencyUpdated;
}
