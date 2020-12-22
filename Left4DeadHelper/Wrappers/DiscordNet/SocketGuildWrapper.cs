using Discord;
using Discord.Audio;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class SocketGuildWrapper : SocketEntityWrapper<ulong>, ISocketGuildWrapper
    {
        private readonly SocketGuild _socketGuild;

        public SocketGuildWrapper(SocketGuild socketGuild)
            : base(socketGuild)
        {
            _socketGuild = socketGuild ?? throw new ArgumentNullException(nameof(socketGuild));
        }

        public virtual SocketGuildUser Owner => _socketGuild.Owner;

        public virtual bool IsConnected => _socketGuild.IsConnected;

        public virtual int DownloadedMemberCount => _socketGuild.DownloadedMemberCount;

        public virtual int MemberCount => _socketGuild.MemberCount;

        public virtual IReadOnlyCollection<SocketGuildChannel> Channels => _socketGuild.Channels;

        public virtual SocketGuildUser CurrentUser => _socketGuild.CurrentUser;

        public virtual IReadOnlyCollection<SocketCategoryChannel> CategoryChannels => _socketGuild.CategoryChannels;

        public virtual IReadOnlyCollection<SocketVoiceChannel> VoiceChannels => _socketGuild.VoiceChannels;

        public virtual IReadOnlyCollection<SocketTextChannel> TextChannels => _socketGuild.TextChannels;

        public virtual SocketTextChannel SystemChannel => _socketGuild.SystemChannel;

        public virtual SocketVoiceChannel AFKChannel => _socketGuild.AFKChannel;

        public virtual SocketTextChannel DefaultChannel => _socketGuild.DefaultChannel;

        public virtual Task DownloaderPromise => _socketGuild.DownloaderPromise;

        public virtual Task SyncPromise => _socketGuild.SyncPromise;

        public virtual bool IsSynced => _socketGuild.IsSynced;

        public virtual bool HasAllMembers => _socketGuild.HasAllMembers;

        public virtual ISocketGuildChannelWrapper? EmbedChannel
        {
            get
            {
                var rawChannel = _socketGuild.EmbedChannel;
                return rawChannel != null ? new SocketGuildChannelWrapper(rawChannel) : null;
            }
        }

        public virtual IReadOnlyCollection<ISocketGuildUserWrapper>? Users
        {
            get
            {
                var rawUsers = _socketGuild.Users;
                if (rawUsers != null)
                {
                    return _socketGuild.Users.Select(u => new SocketGuildUserWrapper(u)).ToList().AsReadOnly();
                }
                return null;
            }
        }

        public virtual string Name => _socketGuild.Name;

        public virtual int AFKTimeout => _socketGuild.AFKTimeout;

        public virtual bool IsEmbeddable => _socketGuild.IsEmbeddable;

        public virtual DefaultMessageNotifications DefaultMessageNotifications => _socketGuild.DefaultMessageNotifications;

        public virtual MfaLevel MfaLevel => _socketGuild.MfaLevel;

        public virtual VerificationLevel VerificationLevel => _socketGuild.VerificationLevel;

        public virtual ExplicitContentFilterLevel ExplicitContentFilter => _socketGuild.ExplicitContentFilter;

        public virtual string IconId => _socketGuild.IconId;

        public virtual string IconUrl => _socketGuild.IconUrl;

        public virtual string SplashId => _socketGuild.SplashId;

        public virtual string SplashUrl => _socketGuild.SplashUrl;

        public virtual bool Available => ((IGuild)_socketGuild).Available;

        public virtual ulong? AFKChannelId => ((IGuild)_socketGuild).AFKChannelId;

        public virtual ulong DefaultChannelId => ((IGuild)_socketGuild).DefaultChannelId;

        public virtual ulong? EmbedChannelId => ((IGuild)_socketGuild).EmbedChannelId;

        public virtual ulong? SystemChannelId => ((IGuild)_socketGuild).SystemChannelId;

        public virtual ulong OwnerId => _socketGuild.OwnerId;

        public virtual ulong? ApplicationId => _socketGuild.ApplicationId;

        public virtual string VoiceRegionId => _socketGuild.VoiceRegionId;

        public virtual IAudioClient AudioClient => _socketGuild.AudioClient;

        public virtual IRole EveryoneRole => _socketGuild.EveryoneRole;

        public virtual IReadOnlyCollection<GuildEmote> Emotes => _socketGuild.Emotes;

        public virtual IReadOnlyCollection<string> Features => _socketGuild.Features;

        public virtual IReadOnlyCollection<IRole> Roles => _socketGuild.Roles;

        public virtual PremiumTier PremiumTier => _socketGuild.PremiumTier;

        public virtual string BannerId => _socketGuild.BannerId;

        public virtual string BannerUrl => _socketGuild.BannerUrl;

        public virtual string VanityURLCode => _socketGuild.VanityURLCode;

        public virtual SystemChannelMessageDeny SystemChannelFlags => _socketGuild.SystemChannelFlags;

        public virtual string Description => _socketGuild.Description;

        public virtual int PremiumSubscriptionCount => _socketGuild.PremiumSubscriptionCount;

        public virtual string PreferredLocale => _socketGuild.PreferredLocale;

        public virtual CultureInfo PreferredCulture => _socketGuild.PreferredCulture;

        public virtual DateTimeOffset CreatedAt => _socketGuild.CreatedAt;

        public virtual Task AddBanAsync(IUser user, int pruneDays = 0, string? reason = null, RequestOptions? options = null)
        {
            return _socketGuild.AddBanAsync(user, pruneDays, reason, options);
        }

        public virtual Task AddBanAsync(ulong userId, int pruneDays = 0, string? reason = null, RequestOptions? options = null)
        {
            return _socketGuild.AddBanAsync(userId, pruneDays, reason, options);
        }

        public virtual Task<RestGuildUser> AddGuildUserAsync(ulong userId, string accessToken, Action<AddGuildUserProperties>? func = null, RequestOptions? options = null)
        {
            return _socketGuild.AddGuildUserAsync(userId, accessToken, func, options);
        }

        Task<IGuildUser> IGuild.AddGuildUserAsync(ulong userId, string accessToken, Action<AddGuildUserProperties>? func, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).AddGuildUserAsync(userId, accessToken, func, options);
        }

        public virtual Task<ICategoryChannel> CreateCategoryAsync(string name, Action<GuildChannelProperties>? func = null, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).CreateCategoryAsync(name, func, options);
        }

        public virtual Task<RestCategoryChannel> CreateCategoryChannelAsync(string name, Action<GuildChannelProperties>? func = null, RequestOptions? options = null)
        {
            return _socketGuild.CreateCategoryChannelAsync(name, func, options);
        }

        public virtual Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default, RequestOptions? options = null)
        {
            return _socketGuild.CreateEmoteAsync(name, image, roles, options);
        }

        public virtual Task<RestGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions? options = null)
        {
            return _socketGuild.CreateIntegrationAsync(id, type, options);
        }

        Task<IGuildIntegration> IGuild.CreateIntegrationAsync(ulong id, string type, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).CreateIntegrationAsync(id, type, options);
        }

        public virtual Task<RestRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, RequestOptions? options = null)
        {
            return _socketGuild.CreateRoleAsync(name, permissions, color, isHoisted, options);
        }

        Task<IRole> IGuild.CreateRoleAsync(string name, GuildPermissions? permissions, Color? color, bool isHoisted, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).CreateRoleAsync(name, permissions, color, isHoisted, options);
        }

        public virtual Task<RestRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false, bool isMentionable = false, RequestOptions? options = null)
        {
            return _socketGuild.CreateRoleAsync(name, permissions, color, isHoisted, isMentionable, options);
        }

        Task<IRole> IGuild.CreateRoleAsync(string name, GuildPermissions? permissions, Color? color, bool isHoisted, bool isMentionable, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).CreateRoleAsync(name, permissions, color, isHoisted, isMentionable, options);
        }

        public virtual Task<RestTextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties>? func = null, RequestOptions? options = null)
        {
            return _socketGuild.CreateTextChannelAsync(name, func, options);
        }

        Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, Action<TextChannelProperties>? func, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).CreateTextChannelAsync(name, func, options);
        }

        public virtual Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties>? func = null, RequestOptions? options = null)
        {
            return _socketGuild.CreateVoiceChannelAsync(name, func, options);
        }

        Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties>? func, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).CreateVoiceChannelAsync(name, func, options);
        }

        public virtual Task DeleteAsync(RequestOptions? options = null)
        {
            return _socketGuild.DeleteAsync(options);
        }

        public virtual Task DeleteEmoteAsync(GuildEmote emote, RequestOptions? options = null)
        {
            return _socketGuild.DeleteEmoteAsync(emote, options);
        }

        public virtual void Dispose()
        {
            ((IDisposable)_socketGuild).Dispose();
        }

        public virtual Task DownloadUsersAsync()
        {
            return _socketGuild.DownloadUsersAsync();
        }

        public virtual Task<IVoiceChannel> GetAFKChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetAFKChannelAsync(mode, options);
        }

        public virtual IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(int limit, RequestOptions? options = null, ulong? beforeId = null, ulong? userId = null, ActionType? actionType = null)
        {
            return _socketGuild.GetAuditLogsAsync(limit, options, beforeId, userId, actionType);
        }

        Task<IReadOnlyCollection<IAuditLogEntry>> IGuild.GetAuditLogsAsync(int limit, CacheMode mode, RequestOptions? options, ulong? beforeId, ulong? userId, ActionType? actionType)
        {
            return ((IGuild)_socketGuild).GetAuditLogsAsync(limit, mode, options, beforeId, userId, actionType);
        }

        public virtual Task<RestBan> GetBanAsync(IUser user, RequestOptions? options)
        {
            return _socketGuild.GetBanAsync(user, options);
        }

        Task<IBan> IGuild.GetBanAsync(IUser user, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetBanAsync(user, options);
        }

        public virtual Task<RestBan> GetBanAsync(ulong userId, RequestOptions? options)
        {
            return _socketGuild.GetBanAsync(userId, options);
        }

        Task<IBan> IGuild.GetBanAsync(ulong userId, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetBanAsync(userId, options);
        }

        public virtual Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetBansAsync(options);
        }

        Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetBansAsync(options);
        }

        public virtual Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetCategoriesAsync(mode, options);
        }

        public virtual SocketCategoryChannel GetCategoryChannel(ulong id)
        {
            return _socketGuild.GetCategoryChannel(id);
        }

        public virtual ISocketGuildChannelWrapper GetChannel(ulong id)
        {
            return new SocketGuildChannelWrapper(_socketGuild.GetChannel(id));
        }

        public virtual Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetChannelAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetChannelsAsync(mode, options);
        }

        public virtual Task<IGuildUser> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetCurrentUserAsync(mode, options);
        }

        public virtual Task<ITextChannel> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetDefaultChannelAsync(mode, options);
        }

        public virtual Task<IGuildChannel> GetEmbedChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetEmbedChannelAsync(mode, options);
        }

        public virtual Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions? options = null)
        {
            return _socketGuild.GetEmoteAsync(id, options);
        }

        public virtual Task<IReadOnlyCollection<RestGuildIntegration>> GetIntegrationsAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetIntegrationsAsync(options);
        }

        Task<IReadOnlyCollection<IGuildIntegration>> IGuild.GetIntegrationsAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetIntegrationsAsync(options);
        }

        public virtual Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetInvitesAsync(options);
        }

        Task<IReadOnlyCollection<IInviteMetadata>> IGuild.GetInvitesAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetInvitesAsync(options);
        }

        public virtual Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetOwnerAsync(mode, options);
        }

        public virtual IRole GetRole(ulong id)
        {
            return _socketGuild.GetRole(id);
        }

        public virtual Task<ITextChannel> GetSystemChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetSystemChannelAsync(mode, options);
        }

        public virtual SocketTextChannel GetTextChannel(ulong id)
        {
            return _socketGuild.GetTextChannel(id);
        }

        public virtual Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetTextChannelAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetTextChannelsAsync(mode, options);
        }

        public virtual SocketGuildUser GetUser(ulong id)
        {
            return _socketGuild.GetUser(id);
        }

        public virtual Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetUserAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetUsersAsync(mode, options);
        }

        public virtual Task<RestInviteMetadata> GetVanityInviteAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetVanityInviteAsync(options);
        }

        Task<IInviteMetadata> IGuild.GetVanityInviteAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetVanityInviteAsync(options);
        }

        public virtual ISocketVoiceChannelWrapper? GetVoiceChannel(ulong id)
        {
            var rawChannel = _socketGuild.GetVoiceChannel(id);
            return rawChannel != null ? new SocketVoiceChannelWrapper(rawChannel) : null;
        }

        public virtual Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetVoiceChannelAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IGuild)_socketGuild).GetVoiceChannelsAsync(mode, options);
        }

        public virtual Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetVoiceRegionsAsync(options);
        }

        Task<IReadOnlyCollection<IVoiceRegion>> IGuild.GetVoiceRegionsAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetVoiceRegionsAsync(options);
        }

        public virtual Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null)
        {
            return _socketGuild.GetWebhookAsync(id, options);
        }

        Task<IWebhook> IGuild.GetWebhookAsync(ulong id, RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetWebhookAsync(id, options);
        }

        public virtual Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions? options = null)
        {
            return _socketGuild.GetWebhooksAsync(options);
        }

        Task<IReadOnlyCollection<IWebhook>> IGuild.GetWebhooksAsync(RequestOptions? options)
        {
            return ((IGuild)_socketGuild).GetWebhooksAsync(options);
        }

        public virtual Task LeaveAsync(RequestOptions? options = null)
        {
            return _socketGuild.LeaveAsync(options);
        }

        public virtual Task ModifyAsync(Action<GuildProperties> func, RequestOptions? options = null)
        {
            return _socketGuild.ModifyAsync(func, options);
        }

        public virtual Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions? options = null)
        {
            return _socketGuild.ModifyEmbedAsync(func, options);
        }

        public virtual Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions? options = null)
        {
            return _socketGuild.ModifyEmoteAsync(emote, func, options);
        }

        public virtual Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions? options = null)
        {
            return _socketGuild.PruneUsersAsync(days, simulate, options);
        }

        public virtual Task RemoveBanAsync(IUser user, RequestOptions? options = null)
        {
            return _socketGuild.RemoveBanAsync(user, options);
        }

        public virtual Task RemoveBanAsync(ulong userId, RequestOptions? options = null)
        {
            return _socketGuild.RemoveBanAsync(userId, options);
        }

        public virtual Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions? options = null)
        {
            return _socketGuild.ReorderChannelsAsync(args, options);
        }

        public virtual Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions? options = null)
        {
            return _socketGuild.ReorderRolesAsync(args, options);
        }
    }
}
