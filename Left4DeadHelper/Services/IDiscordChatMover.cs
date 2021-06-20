using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Services
{
    public interface IDiscordChatMover
    {
        Task<MoveResult> MovePlayersToCorrectChannelsAsync(
            IRCONWrapper rcon,
            IDiscordSocketClientWrapper client, ISocketGuildWrapper guild,
            CancellationToken cancellationToken);

        Task<ReuniteResult> RenuitePlayersAsync(
            IDiscordSocketClientWrapper client, ISocketGuildWrapper guild,
            CancellationToken cancellationToken);
    }
}
