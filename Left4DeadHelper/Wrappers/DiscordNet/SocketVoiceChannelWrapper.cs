using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class SocketVoiceChannelWrapper : SocketGuildChannelWrapper, ISocketVoiceChannelWrapper
    {
        private readonly SocketVoiceChannel _socketVoiceChannel;

        public SocketVoiceChannelWrapper(SocketVoiceChannel socketVoiceChannel)
            : base(socketVoiceChannel)
        {
            _socketVoiceChannel = socketVoiceChannel ?? throw new ArgumentNullException(nameof(socketVoiceChannel));
        }

        public virtual ICategoryChannel Category => _socketVoiceChannel.Category;

        public virtual int Bitrate => _socketVoiceChannel.Bitrate;

        public virtual int? UserLimit => _socketVoiceChannel.UserLimit;

        public virtual ulong? CategoryId => _socketVoiceChannel.CategoryId;

        public string Mention => _socketVoiceChannel.Mention;

        public virtual Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false)
        {
            return _socketVoiceChannel.ConnectAsync(selfDeaf, selfMute, external);
        }

        public virtual Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null)
        {
            return _socketVoiceChannel.CreateInviteAsync(maxAge, maxUses, isTemporary, isUnique, options);
        }

        public Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null)
        {
            return _socketVoiceChannel.CreateInviteToApplicationAsync(applicationId, maxAge, maxUses, isTemporary, isUnique, options);
        }

        public Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null)
        {
            return _socketVoiceChannel.CreateInviteToApplicationAsync(application, maxAge, maxUses, isTemporary, isUnique, options);
        }

        public Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null)
        {
            return _socketVoiceChannel.CreateInviteToStreamAsync(user, maxAge, maxUses, isTemporary, isUnique, options);
        }

        public virtual Task DisconnectAsync()
        {
            return _socketVoiceChannel.DisconnectAsync();
        }

        public virtual Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((INestedChannel)_socketVoiceChannel).GetCategoryAsync(mode, options);
        }

        public virtual Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null)
        {
            return _socketVoiceChannel.GetInvitesAsync(options);
        }

        public virtual Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions? options = null)
        {
            return _socketVoiceChannel.ModifyAsync(func, options);
        }

        public Task ModifyAsync(Action<AudioChannelProperties> func, RequestOptions? options = null)
        {
            return _socketVoiceChannel.ModifyAsync(func, options);
        }

        public virtual Task SyncPermissionsAsync(RequestOptions? options = null)
        {
            return _socketVoiceChannel.SyncPermissionsAsync(options);
        }
    }
}
