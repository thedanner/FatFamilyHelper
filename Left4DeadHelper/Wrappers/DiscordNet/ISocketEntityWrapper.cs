using Discord;
using System;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketEntityWrapper<T> : IEntity<T>
        where T : IEquatable<T>
    {
    }
}
