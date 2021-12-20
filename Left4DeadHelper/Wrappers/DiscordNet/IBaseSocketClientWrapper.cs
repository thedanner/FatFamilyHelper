using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface IBaseSocketClientWrapper : IBaseDiscordClientWrapper, IDiscordClient, IDisposable
    {
        IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels { get; }
        IReadOnlyCollection<SocketGuild> Guilds { get; }
        DiscordSocketRestClient Rest { get; }
        IActivity Activity { get; }
        UserStatus Status { get; }
        int Latency { get; }
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;
        event Func<SocketRole, Task> RoleCreated;
        event Func<SocketRole, Task> RoleDeleted;
        event Func<SocketRole, SocketRole, Task> RoleUpdated;
        event Func<SocketGuild, Task> JoinedGuild;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;
        event Func<SocketGuild, Task> LeftGuild;
        event Func<SocketGuild, Task> GuildUnavailable;
        event Func<SocketGuild, Task> GuildMembersDownloaded;
        event Func<SocketGuild, SocketGuild, Task> GuildUpdated;
        event Func<SocketGuildUser, Task> UserJoined;
        event Func<SocketGuildUser, Task> UserLeft;
        event Func<SocketUser, SocketGuild, Task> UserBanned;
        event Func<SocketUser, SocketGuild, Task> UserUnbanned;
        event Func<SocketUser, SocketUser, Task> UserUpdated;
        event Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;
        event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;
        event Func<SocketVoiceServer, Task> VoiceServerUpdated;
        event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;
        event Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping;
        event Func<SocketGuild, Task> GuildAvailable;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;
        event Func<SocketMessage, Task> MessageReceived;
        event Func<IReadOnlyCollection<Cacheable<IMessage, ulong>>, ISocketMessageChannel, Task> MessagesBulkDeleted;
        event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;
        event Func<SocketChannel, Task> ChannelCreated;
        event Func<SocketChannel, Task> ChannelDestroyed;
        event Func<SocketGroupUser, Task> RecipientRemoved;
        event Func<SocketGroupUser, Task> RecipientAdded;
        event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
        event Func<SocketChannel, SocketChannel, Task> ChannelUpdated;
        
        Task DownloadUsersAsync(IEnumerable<IGuild> guilds);
        ISocketChannelWrapper? GetChannel(ulong id);
        ISocketGuildWrapper? GetGuild(ulong id);
        ISocketUserWrapper? GetUser(ulong id);
        ISocketUserWrapper? GetUser(string username, string discriminator);
        RestVoiceRegion GetVoiceRegion(string id);
        Task SetActivityAsync(IActivity activity);
        Task SetGameAsync(string name, string? streamUrl = null, ActivityType type = ActivityType.Playing);
        Task SetStatusAsync(UserStatus status);
    }
}
