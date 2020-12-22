using Left4DeadHelper.Wrappers.DiscordNet;
using Left4DeadHelper.Wrappers.Rcon;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper
{
    public interface IDiscordChatMover
    {
        Task StartAsync(IDiscordSocketClientWrapper client, CancellationToken cancellationToken);
        Task<int> MovePlayersToCorrectChannelsAsync(IRCONWrapper rcon, CancellationToken cancellationToken);
    }
}
