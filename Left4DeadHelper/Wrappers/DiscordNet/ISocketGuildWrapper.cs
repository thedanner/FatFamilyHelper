using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface ISocketGuildWrapper : ISocketEntityWrapper<ulong>, IGuild, IDeletable, ISnowflakeEntity, IEntity<ulong>, IDisposable
    {
        SocketGuildUser Owner { get; }
        bool IsConnected { get; }
        int DownloadedMemberCount { get; }
        int MemberCount { get; }
        IReadOnlyCollection<SocketGuildChannel> Channels { get; }
        SocketGuildUser CurrentUser { get; }
        IReadOnlyCollection<SocketCategoryChannel> CategoryChannels { get; }
        IReadOnlyCollection<SocketVoiceChannel> VoiceChannels { get; }
        IReadOnlyCollection<SocketTextChannel> TextChannels { get; }
        SocketTextChannel SystemChannel { get; }
        SocketVoiceChannel AFKChannel { get; }
        SocketTextChannel DefaultChannel { get; }
        Task DownloaderPromise { get; }
        Task SyncPromise { get; }
        bool IsSynced { get; }
        bool HasAllMembers { get; }
        IReadOnlyCollection<ISocketGuildUserWrapper>? Users { get; }

        new IReadOnlyCollection<SocketRole> Roles { get; }

        Task<RestCategoryChannel> CreateCategoryChannelAsync(string name, Action<GuildChannelProperties>? func = null, RequestOptions? options = null);
        IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(int limit, RequestOptions? options = null, ulong? beforeId = null, ulong? userId = null, ActionType? actionType = null);
        SocketCategoryChannel GetCategoryChannel(ulong id);
        ISocketGuildChannelWrapper? GetChannel(ulong id);
        SocketTextChannel GetTextChannel(ulong id);
        SocketGuildUser GetUser(ulong id);
        ISocketVoiceChannelWrapper? GetVoiceChannel(ulong id);
    }
}
