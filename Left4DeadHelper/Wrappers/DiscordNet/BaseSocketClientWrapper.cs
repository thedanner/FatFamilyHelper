using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet;

public class BaseSocketClientWrapper : BaseDiscordClientWrapper, IBaseSocketClientWrapper
{
    private readonly BaseSocketClient _baseSocketClient;

    public BaseSocketClientWrapper(BaseSocketClient baseSocketClient)
        : base(baseSocketClient)
    {
        _baseSocketClient = baseSocketClient ?? throw new ArgumentNullException(nameof(baseSocketClient));
    }

    public virtual IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels => _baseSocketClient.PrivateChannels;

    public virtual IReadOnlyCollection<SocketGuild> Guilds => _baseSocketClient.Guilds;

    public virtual IActivity Activity => _baseSocketClient.Activity;
    public virtual UserStatus Status => _baseSocketClient.Status;
    public virtual int Latency => _baseSocketClient.Latency;

    public virtual DiscordSocketRestClient Rest => _baseSocketClient.Rest;

    public virtual event Func<SocketRole, Task> RoleCreated
    {
        add { _baseSocketClient.RoleCreated += value; }
        remove { _baseSocketClient.RoleCreated -= value; }
    }

    public virtual event Func<SocketRole, Task> RoleDeleted
    {
        add { _baseSocketClient.RoleDeleted += value; }
        remove { _baseSocketClient.RoleDeleted -= value; }
    }

    public virtual event Func<SocketRole, SocketRole, Task> RoleUpdated
    {
        add { _baseSocketClient.RoleUpdated += value; }
        remove { _baseSocketClient.RoleUpdated -= value; }
    }

    public virtual event Func<SocketGuild, Task> JoinedGuild
    {
        add { _baseSocketClient.JoinedGuild += value; }
        remove { _baseSocketClient.JoinedGuild -= value; }
    }

    public virtual event Func<SocketGuild, Task> LeftGuild
    {
        add { _baseSocketClient.LeftGuild += value; }
        remove { _baseSocketClient.LeftGuild -= value; }
    }

    public virtual event Func<SocketGuild, Task> GuildUnavailable
    {
        add { _baseSocketClient.GuildUnavailable += value; }
        remove { _baseSocketClient.GuildUnavailable -= value; }
    }

    public virtual event Func<SocketGuild, Task> GuildMembersDownloaded
    {
        add { _baseSocketClient.GuildMembersDownloaded += value; }
        remove { _baseSocketClient.GuildMembersDownloaded -= value; }
    }

    public virtual event Func<SocketGuild, SocketGuild, Task> GuildUpdated
    {
        add { _baseSocketClient.GuildUpdated += value; }
        remove { _baseSocketClient.GuildUpdated -= value; }
    }

    public virtual event Func<SocketGuildUser, Task> UserJoined
    {
        add { _baseSocketClient.UserJoined += value; }
        remove { _baseSocketClient.UserJoined -= value; }
    }

    public virtual event Func<SocketUser, SocketGuild, Task> UserBanned
    {
        add { _baseSocketClient.UserBanned += value; }
        remove { _baseSocketClient.UserBanned -= value; }
    }

    public virtual event Func<SocketUser, SocketGuild, Task> UserUnbanned
    {
        add { _baseSocketClient.UserUnbanned += value; }
        remove { _baseSocketClient.UserUnbanned -= value; }
    }

    public virtual event Func<SocketUser, SocketUser, Task> UserUpdated
    {
        add { _baseSocketClient.UserUpdated += value; }
        remove { _baseSocketClient.UserUpdated -= value; }
    }

    public virtual event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated
    {
        add { _baseSocketClient.UserVoiceStateUpdated += value; }
        remove { _baseSocketClient.UserVoiceStateUpdated -= value; }
    }

    public virtual event Func<SocketVoiceServer, Task> VoiceServerUpdated
    {
        add { _baseSocketClient.VoiceServerUpdated += value; }
        remove { _baseSocketClient.VoiceServerUpdated -= value; }
    }

    public virtual event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
    {
        add { _baseSocketClient.CurrentUserUpdated += value; }
        remove { _baseSocketClient.CurrentUserUpdated -= value; }
    }

    public virtual event Func<SocketGuild, Task> GuildAvailable
    {
        add { _baseSocketClient.GuildAvailable += value; }
        remove { _baseSocketClient.GuildAvailable -= value; }
    }

    public virtual event Func<SocketMessage, Task> MessageReceived
    {
        add { _baseSocketClient.MessageReceived += value; }
        remove { _baseSocketClient.MessageReceived -= value; }
    }

    public virtual event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated
    {
        add { _baseSocketClient.MessageUpdated += value; }
        remove { _baseSocketClient.MessageUpdated -= value; }
    }

    public virtual event Func<SocketChannel, Task> ChannelCreated
    {
        add { _baseSocketClient.ChannelCreated += value; }
        remove { _baseSocketClient.ChannelCreated -= value; }
    }

    public virtual event Func<SocketChannel, Task> ChannelDestroyed
    {
        add { _baseSocketClient.ChannelDestroyed += value; }
        remove { _baseSocketClient.ChannelDestroyed -= value; }
    }

    public virtual event Func<SocketGroupUser, Task> RecipientRemoved
    {
        add { _baseSocketClient.RecipientRemoved += value; }
        remove { _baseSocketClient.RecipientRemoved -= value; }
    }

    public virtual event Func<SocketGroupUser, Task> RecipientAdded
    {
        add { _baseSocketClient.RecipientAdded += value; }
        remove { _baseSocketClient.RecipientAdded -= value; }
    }

    public virtual event Func<SocketChannel, SocketChannel, Task> ChannelUpdated
    {
        add { _baseSocketClient.ChannelUpdated += value; }
        remove { _baseSocketClient.ChannelUpdated -= value; }
    }

    event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> IBaseSocketClientWrapper.ReactionsCleared
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionsCleared += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionsCleared -= value; }
    }

    event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> IBaseSocketClientWrapper.ReactionRemoved
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionRemoved += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionRemoved -= value; }
    }

    event Func<SocketGuildUser, Task> IBaseSocketClientWrapper.UserLeft
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).UserLeft += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).UserLeft -= value; }
    }

    event Func<SocketGuildUser, SocketGuildUser, Task> IBaseSocketClientWrapper.GuildMemberUpdated
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).GuildMemberUpdated += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).GuildMemberUpdated -= value; }
    }

    event Func<SocketUser, ISocketMessageChannel, Task> IBaseSocketClientWrapper.UserIsTyping
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).UserIsTyping += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).UserIsTyping -= value; }
    }

    event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> IBaseSocketClientWrapper.ReactionAdded
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionAdded += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).ReactionAdded -= value; }
    }

    event Func<IReadOnlyCollection<Cacheable<IMessage, ulong>>, ISocketMessageChannel, Task> IBaseSocketClientWrapper.MessagesBulkDeleted
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).MessagesBulkDeleted += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).MessagesBulkDeleted -= value; }
    }

    event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> IBaseSocketClientWrapper.MessageDeleted
    {
        add { ((IBaseSocketClientWrapper)_baseSocketClient).MessageDeleted += value; }
        remove { ((IBaseSocketClientWrapper)_baseSocketClient).MessageDeleted -= value; }
    }

    public virtual Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
    {
        return _baseSocketClient.DownloadUsersAsync(guilds);
    }

    public virtual ISocketChannelWrapper? GetChannel(ulong id)
    {
        var rawChannel = _baseSocketClient.GetChannel(id);
        return rawChannel != null ? new SocketChannelWrapper(rawChannel) : null;
    }

    public virtual ISocketGuildWrapper? GetGuild(ulong id)
    {
        return new SocketGuildWrapper(_baseSocketClient.GetGuild(id));
    }

    public virtual ISocketUserWrapper? GetUser(ulong id)
    {
        var rawUser = _baseSocketClient.GetUser(id);
        return rawUser != null ? new SocketUserWrapper(rawUser) : null;
    }

    public virtual ISocketUserWrapper? GetUser(string username, string discriminator)
    {
        var rawUser = _baseSocketClient.GetUser(username, discriminator);
        return rawUser != null ? new SocketUserWrapper(rawUser) : null;
    }

    public RestVoiceRegion GetVoiceRegion(string id)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetActivityAsync(IActivity activity)
    {
        return _baseSocketClient.SetActivityAsync(activity);
    }

    public virtual Task SetGameAsync(string name, string? streamUrl = null, ActivityType type = ActivityType.Playing)
    {
        return _baseSocketClient.SetGameAsync(name, streamUrl, type);
    }

    public virtual Task SetStatusAsync(UserStatus status)
    {
        return _baseSocketClient.SetStatusAsync(status);
    }
}
