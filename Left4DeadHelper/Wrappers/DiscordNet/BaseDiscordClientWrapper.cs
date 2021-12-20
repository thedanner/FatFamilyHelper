using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class BaseDiscordClientWrapper : IBaseDiscordClientWrapper
    {
        private readonly BaseDiscordClient _baseDiscordClient;

        public BaseDiscordClientWrapper(BaseDiscordClient baseDiscordClient)
        {
            _baseDiscordClient = baseDiscordClient ?? throw new ArgumentNullException(nameof(baseDiscordClient));
        }

        public virtual LoginState LoginState => _baseDiscordClient.LoginState;

        public virtual ConnectionState ConnectionState => ((IDiscordClient)_baseDiscordClient).ConnectionState;

        public virtual ISelfUser CurrentUser => _baseDiscordClient.CurrentUser;

        public virtual TokenType TokenType => _baseDiscordClient.TokenType;

        public virtual event Func<LogMessage, Task> Log
        {
            add { _baseDiscordClient.Log += value; }
            remove { _baseDiscordClient.Log += value; }
        }

        public virtual event Func<Task> LoggedIn
        {
            add { _baseDiscordClient.LoggedIn += value; }
            remove { _baseDiscordClient.LoggedIn += value; }
        }

        public virtual event Func<Task> LoggedOut
        {
            add { _baseDiscordClient.LoggedOut += value; }
            remove { _baseDiscordClient.LoggedOut += value; }
        }

        public Task<IReadOnlyCollection<IApplicationCommand>> BulkOverwriteGlobalApplicationCommand(ApplicationCommandProperties[] properties, RequestOptions? options = null)
        {
            return BulkOverwriteGlobalApplicationCommand(properties, options);
        }

        public Task<IApplicationCommand> CreateGlobalApplicationCommand(ApplicationCommandProperties properties, RequestOptions? options = null)
        {
            return CreateGlobalApplicationCommand(properties, options);
        }

        public virtual Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream? jpegIcon = null, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).CreateGuildAsync(name, region, jpegIcon, options);
        }

        public virtual void Dispose()
        {
            _baseDiscordClient.Dispose();
        }

        public virtual Task<IApplication> GetApplicationInfoAsync(RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetApplicationInfoAsync(options);
        }

        public virtual Task<BotGateway> GetBotGatewayAsync(RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetBotGatewayAsync(options);
        }

        public virtual Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetChannelAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync(RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetConnectionsAsync(options);
        }

        public virtual Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetDMChannelsAsync(mode, options);
        }

        public Task<IApplicationCommand> GetGlobalApplicationCommandAsync(ulong id, RequestOptions? options = null)
        {
            return GetGlobalApplicationCommandAsync(id, options);
        }

        public Task<IReadOnlyCollection<IApplicationCommand>> GetGlobalApplicationCommandsAsync(RequestOptions? options = null)
        {
            return GetGlobalApplicationCommandsAsync(options);
        }

        public virtual Task<IReadOnlyCollection<IGroupChannel>> GetGroupChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetGroupChannelsAsync(mode, options);
        }

        public virtual Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetGuildAsync(id, mode, options);
        }

        public virtual Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetGuildsAsync(mode, options);
        }

        public virtual Task<IInvite> GetInviteAsync(string inviteId, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetInviteAsync(inviteId, options);
        }

        public virtual Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetPrivateChannelsAsync(mode, options);
        }

        public virtual Task<int> GetRecommendedShardCountAsync(RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetRecommendedShardCountAsync(options);
        }

        public virtual Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetUserAsync(id, mode, options);
        }

        public virtual Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetUserAsync(username, discriminator, options);
        }

        public virtual Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetVoiceRegionAsync(id, options);
        }

        public virtual Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetVoiceRegionsAsync(options);
        }

        public virtual Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null)
        {
            return ((IDiscordClient)_baseDiscordClient).GetWebhookAsync(id, options);
        }

        public virtual Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
        {
            return _baseDiscordClient.LoginAsync(tokenType, token, validateToken);
        }

        public virtual Task LogoutAsync()
        {
            return _baseDiscordClient.LogoutAsync();
        }

        public virtual Task StartAsync()
        {
            return ((IDiscordClient)_baseDiscordClient).StartAsync();
        }

        public virtual Task StopAsync()
        {
            return ((IDiscordClient)_baseDiscordClient).StopAsync();
        }
    }
}
