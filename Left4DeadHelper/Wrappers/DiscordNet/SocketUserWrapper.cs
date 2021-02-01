using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public class SocketUserWrapper : SocketEntityWrapper<ulong>, ISocketUserWrapper
    {
        private readonly SocketUser _socketUser;

        public SocketUserWrapper(SocketUser socketUser)
            :base(socketUser)
        {
            _socketUser = socketUser ?? throw new ArgumentNullException(nameof(socketUser));
        }

        public virtual IReadOnlyCollection<ISocketGuildWrapper>? MutualGuilds
        {
            get
            {
                var rawMutualGuilds = _socketUser.MutualGuilds;
                if (rawMutualGuilds != null)
                {
                    return _socketUser.MutualGuilds.Select(g => new SocketGuildWrapper(g)).ToList().AsReadOnly();
                }
                return null;
            }
        }

        public virtual string AvatarId => _socketUser.AvatarId;

        public virtual string Discriminator => _socketUser.Discriminator;

        public virtual ushort DiscriminatorValue => _socketUser.DiscriminatorValue;

        public virtual bool IsBot => _socketUser.IsBot;

        public virtual bool IsWebhook => _socketUser.IsWebhook;

        public virtual string Username => _socketUser.Username;

        public virtual DateTimeOffset CreatedAt => _socketUser.CreatedAt;

        public virtual string Mention => _socketUser.Mention;

        public virtual IActivity Activity => _socketUser.Activity;

        public virtual UserStatus Status => _socketUser.Status;

        public virtual IImmutableSet<ClientType> ActiveClients => _socketUser.ActiveClients;

        public virtual UserProperties? PublicFlags => _socketUser.PublicFlags;

        public virtual IImmutableList<IActivity> Activities => _socketUser.Activities;

        public virtual string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return _socketUser.GetAvatarUrl(format, size);
        }

        public virtual string GetDefaultAvatarUrl()
        {
            return _socketUser.GetDefaultAvatarUrl();
        }

        public virtual Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions? options = null)
        {
            return _socketUser.GetOrCreateDMChannelAsync(options);
        }
    }
}
