using Discord;
using System.Collections.Generic;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketUserWrapper: ISocketEntityWrapper<ulong>, IUser, ISnowflakeEntity, IEntity<ulong>, IMentionable, IPresence
    {
        public IReadOnlyCollection<ISocketGuildWrapper>? MutualGuilds { get; }
    }
}
