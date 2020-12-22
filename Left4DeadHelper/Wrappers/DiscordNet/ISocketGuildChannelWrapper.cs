using Discord;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketGuildChannelWrapper : ISocketChannelWrapper, IGuildChannel, IChannel, ISnowflakeEntity, IEntity<ulong>, IDeletable
    {
    }
}
