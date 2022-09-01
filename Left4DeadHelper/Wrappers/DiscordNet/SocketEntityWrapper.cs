using Discord.WebSocket;
using System;

namespace Left4DeadHelper.Wrappers.DiscordNet;

public class SocketEntityWrapper<T> : ISocketEntityWrapper<T>
    where T : IEquatable<T>
{
    private readonly SocketEntity<T> _socketEntity;

    public SocketEntityWrapper(SocketEntity<T> socketEntity)
    {
        _socketEntity = socketEntity ?? throw new ArgumentNullException(nameof(socketEntity));
    }

    public virtual T Id => _socketEntity.Id;
}
