using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet;

public class SocketChannelWrapper : SocketEntityWrapper<ulong>, ISocketChannelWrapper
{
    private readonly SocketChannel _socketChannel;

    public SocketChannelWrapper(SocketChannel socketChannel)
        :base(socketChannel)
    {
        _socketChannel = socketChannel ?? throw new ArgumentNullException(nameof(socketChannel));
    }

    public virtual IReadOnlyCollection<SocketUser> Users => _socketChannel.Users;

    public virtual string Name => ((IChannel)_socketChannel).Name;

    public virtual DateTimeOffset CreatedAt => _socketChannel.CreatedAt;

    public virtual SocketUser GetUser(ulong id)
    {
        return _socketChannel.GetUser(id);
    }

    public virtual Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
    {
        return ((IChannel)_socketChannel).GetUserAsync(id, mode, options);
    }

    public virtual IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
    {
        return ((IChannel)_socketChannel).GetUsersAsync(mode, options);
    }
}
