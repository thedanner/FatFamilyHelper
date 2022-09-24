using Discord.WebSocket;
using Left4DeadHelper.Wrappers.Rcon;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Services;

public interface IDiscordChatMover
{
    Task<MoveResult> MovePlayersToCorrectChannelsAsync(
        IRCONWrapper rcon,
        DiscordSocketClient client, SocketGuild guild,
        CancellationToken cancellationToken);

    Task<ReuniteResult> RenuitePlayersAsync(
        DiscordSocketClient client, SocketGuild guild,
        CancellationToken cancellationToken);
}
