using Discord;
using Discord.WebSocket;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketVoiceChannelWrapper : ISocketGuildChannelWrapper, IVoiceChannel, INestedChannel, IGuildChannel, IChannel, ISnowflakeEntity, IEntity<ulong>, IDeletable, IAudioChannel, ISocketAudioChannel
    {
        public ICategoryChannel Category { get; }
    }
}
