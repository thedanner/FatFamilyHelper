using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Left4DeadHelper.Wrappers.DiscordNet;

public interface ISocketChannelWrapper : ISocketEntityWrapper<ulong>, IChannel, ISnowflakeEntity, IEntity<ulong>
{
    IReadOnlyCollection<SocketUser> Users { get; }
    SocketUser GetUser(ulong id);
}
