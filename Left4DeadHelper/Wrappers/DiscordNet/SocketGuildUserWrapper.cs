using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class SocketGuildUserWrapper : SocketUserWrapper, ISocketGuildUserWrapper
    {
        private readonly SocketGuildUser _socketGuildUser;

        public SocketGuildUserWrapper(SocketGuildUser socketGuildUser)
            : base(socketGuildUser)
        {
            _socketGuildUser = socketGuildUser ?? throw new ArgumentNullException(nameof(socketGuildUser));
        }

        public virtual AudioInStream AudioStream => _socketGuildUser.AudioStream;

        public virtual SocketVoiceState? VoiceState => _socketGuildUser.VoiceState;

        public virtual IReadOnlyCollection<SocketRole> Roles => _socketGuildUser.Roles;

        public virtual int Hierarchy => _socketGuildUser.Hierarchy;

        public virtual DateTimeOffset? JoinedAt => _socketGuildUser.JoinedAt;

        public virtual string Nickname => _socketGuildUser.Nickname;

        public virtual GuildPermissions GuildPermissions => _socketGuildUser.GuildPermissions;

        public virtual IGuild Guild => _socketGuildUser.Guild;

        public virtual ulong GuildId => ((IGuildUser)_socketGuildUser).GuildId;

        public virtual DateTimeOffset? PremiumSince => _socketGuildUser.PremiumSince;

        public virtual IReadOnlyCollection<ulong> RoleIds => ((IGuildUser)_socketGuildUser).RoleIds;

        public virtual bool IsDeafened => _socketGuildUser.IsDeafened;

        public virtual bool IsMuted => _socketGuildUser.IsMuted;

        public virtual bool IsSelfDeafened => _socketGuildUser.IsSelfDeafened;

        public virtual bool IsSelfMuted => _socketGuildUser.IsSelfMuted;

        public virtual bool IsSuppressed => _socketGuildUser.IsSuppressed;

        public virtual IVoiceChannel VoiceChannel => _socketGuildUser.VoiceChannel;

        public virtual string VoiceSessionId => _socketGuildUser.VoiceSessionId;

        public virtual bool IsStreaming => _socketGuildUser.IsStreaming;

        public virtual bool? IsPending => _socketGuildUser.IsPending;

        public string GuildAvatarId => _socketGuildUser.GuildAvatarId;

        public DateTimeOffset? RequestToSpeakTimestamp => _socketGuildUser.RequestToSpeakTimestamp;

        public DateTimeOffset? TimedOutUntil => _socketGuildUser.TimedOutUntil;

        public virtual Task AddRoleAsync(ulong roleId, RequestOptions? options = null)
        {
            return _socketGuildUser.AddRoleAsync(roleId, options);
        }

        public virtual Task AddRoleAsync(IRole role, RequestOptions? options = null)
        {
            return _socketGuildUser.AddRoleAsync(role, options);
        }

        public virtual Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions? options = null)
        {
            return _socketGuildUser.AddRolesAsync(roleIds, options);
        }

        public virtual Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null)
        {
            return _socketGuildUser.AddRolesAsync(roles, options);
        }

        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return GetGuildAvatarUrl(format, size);
        }

        public virtual ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            return _socketGuildUser.GetPermissions(channel);
        }

        public virtual Task KickAsync(string? reason = null, RequestOptions? options = null)
        {
            return _socketGuildUser.KickAsync(reason, options);
        }

        public virtual Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions? options = null)
        {
            return _socketGuildUser.ModifyAsync(func, options);
        }

        public virtual Task RemoveRoleAsync(ulong roleId, RequestOptions? options = null)
        {
            return _socketGuildUser.RemoveRoleAsync(roleId, options);
        }

        public virtual Task RemoveRoleAsync(IRole role, RequestOptions? options = null)
        {
            return _socketGuildUser.RemoveRoleAsync(role, options);
        }

        public virtual Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions? options = null)
        {
            return _socketGuildUser.RemoveRolesAsync(roleIds, options);
        }

        public virtual Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null)
        {
            return _socketGuildUser.RemoveRolesAsync(roles, options);
        }

        public Task RemoveTimeOutAsync(RequestOptions? options = null)
        {
            return _socketGuildUser.RemoveTimeOutAsync(options);
        }

        public Task SetTimeOutAsync(TimeSpan span, RequestOptions? options = null)
        {
            return _socketGuildUser.SetTimeOutAsync(span, options);
        }
    }
}
