using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class SocketGuildChannelWrapper : SocketChannelWrapper, ISocketGuildChannelWrapper
    {
        private readonly SocketGuildChannel _socketGuildChannel;

        public SocketGuildChannelWrapper(SocketGuildChannel socketGuildChannel)
            : base(socketGuildChannel)
        {
            _socketGuildChannel = socketGuildChannel ?? throw new System.ArgumentNullException(nameof(socketGuildChannel));
        }

        public virtual int Position => _socketGuildChannel.Position;

        public virtual IGuild Guild => _socketGuildChannel.Guild;

        public virtual ulong GuildId => ((IGuildChannel)_socketGuildChannel).GuildId;

        public virtual IReadOnlyCollection<Overwrite> PermissionOverwrites => _socketGuildChannel.PermissionOverwrites;

        public virtual Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions? options = null)
        {
            return _socketGuildChannel.AddPermissionOverwriteAsync(role, permissions, options);
        }

        public virtual Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions? options = null)
        {
            return _socketGuildChannel.AddPermissionOverwriteAsync(user, permissions, options);
        }

        public virtual Task DeleteAsync(RequestOptions? options = null)
        {
            return _socketGuildChannel.DeleteAsync(options);
        }

        public virtual OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            return _socketGuildChannel.GetPermissionOverwrite(role);
        }

        public virtual OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            return _socketGuildChannel.GetPermissionOverwrite(user);
        }

        public virtual Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions? options = null)
        {
            return _socketGuildChannel.ModifyAsync(func, options);
        }

        public virtual Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null)
        {
            return _socketGuildChannel.RemovePermissionOverwriteAsync(role, options);
        }

        public virtual Task RemovePermissionOverwriteAsync(IUser user, RequestOptions? options = null)
        {
            return _socketGuildChannel.RemovePermissionOverwriteAsync(user, options);
        }

        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            return ((IGuildChannel)_socketGuildChannel).GetUserAsync(id, mode, options);
        }

        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return ((IGuildChannel)_socketGuildChannel).GetUsersAsync(mode, options);
        }
    }
}
