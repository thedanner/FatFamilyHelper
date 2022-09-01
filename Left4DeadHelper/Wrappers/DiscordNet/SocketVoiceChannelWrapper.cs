using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string RTCRegion => _socketVoiceChannel.RTCRegion;

        // TODO add async/await to all async methods to keep stack traces correct.

        public virtual Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false) =>
            _socketVoiceChannel.ConnectAsync(selfDeaf, selfMute, external);

        public virtual Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) =>
            _socketVoiceChannel.CreateInviteAsync(maxAge, maxUses, isTemporary, isUnique, options);

        public Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) =>
            _socketVoiceChannel.CreateInviteToApplicationAsync(applicationId, maxAge, maxUses, isTemporary, isUnique, options);

        public Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) =>
            _socketVoiceChannel.CreateInviteToApplicationAsync(application, maxAge, maxUses, isTemporary, isUnique, options);

        public Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) =>
            _socketVoiceChannel.CreateInviteToStreamAsync(user, maxAge, maxUses, isTemporary, isUnique, options);

        public Task DeleteMessageAsync(ulong messageId, RequestOptions? options = null) =>
            _socketVoiceChannel.DeleteMessageAsync(messageId, options);

        public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
            _socketVoiceChannel.DeleteMessageAsync(message, options);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions? options = null) =>
            _socketVoiceChannel.DeleteMessagesAsync(messages, options);

        public Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions? options = null) =>
            _socketVoiceChannel.DeleteMessagesAsync(messageIds, options);

        public virtual Task DisconnectAsync() =>
            _socketVoiceChannel.DisconnectAsync();

        public IDisposable EnterTypingState(RequestOptions? options = null) =>
            _socketVoiceChannel.EnterTypingState(options);

        public virtual Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
            ((INestedChannel)_socketVoiceChannel).GetCategoryAsync(mode, options);

        public virtual Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null) =>
            _socketVoiceChannel.GetInvitesAsync(options);

        public Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
            ((IMessageChannel)_socketVoiceChannel).GetMessageAsync(id, mode, options);

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
            ((IMessageChannel)_socketVoiceChannel).GetMessagesAsync(limit, mode, options);

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
            ((IMessageChannel)_socketVoiceChannel).GetMessagesAsync(fromMessageId, dir, limit, mode, options);

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
            ((IMessageChannel)_socketVoiceChannel).GetMessagesAsync(fromMessage, dir, limit, mode, options);

        public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) =>
            ((IMessageChannel)_socketVoiceChannel).GetPinnedMessagesAsync(options);

        public virtual Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions? options = null) =>
            _socketVoiceChannel.ModifyAsync(func, options);

        public Task ModifyAsync(Action<AudioChannelProperties> func, RequestOptions? options = null) =>
            _socketVoiceChannel.ModifyAsync(func, options);

        public Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions? options = null) =>
            _socketVoiceChannel.ModifyMessageAsync(messageId, func, options);

        public Task<IUserMessage> SendFileAsync(string filePath, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) =>
            ((IMessageChannel)_socketVoiceChannel).SendFileAsync(filePath, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, components, stickers, embeds, flags);

        public Task<IUserMessage> SendFileAsync(Stream stream, string filename, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) =>
            ((IMessageChannel)_socketVoiceChannel).SendFileAsync(stream, filename, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, components, stickers, embeds, flags);

        public Task<IUserMessage> SendFileAsync(FileAttachment attachment, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) =>
            ((IMessageChannel)_socketVoiceChannel).SendFileAsync(attachment, text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags);

        public Task<IUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) =>
            ((IMessageChannel)_socketVoiceChannel).SendFilesAsync(attachments, text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags);

        public Task<IUserMessage> SendMessageAsync(string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) =>
            ((IMessageChannel)_socketVoiceChannel).SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags);

        public virtual Task SyncPermissionsAsync(RequestOptions? options = null) =>
            _socketVoiceChannel.SyncPermissionsAsync(options);

        public Task TriggerTypingAsync(RequestOptions? options = null) =>
            _socketVoiceChannel.TriggerTypingAsync(options);
    }
}
