using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketGuildUserWrapper : ISocketUserWrapper, IGuildUser, IUser, ISnowflakeEntity, IEntity<ulong>, IMentionable, IPresence, IVoiceState
    {
        AudioInStream AudioStream { get; }
        SocketVoiceState? VoiceState { get; }
        IReadOnlyCollection<SocketRole> Roles { get; }
    }
}
