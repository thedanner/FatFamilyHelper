using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.EventInterfaces
{
    public interface IHandleReactionAddedAsync : IHandleDiscordEvents
    {
        Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> maybeCachedMessage, ISocketMessageChannel channel,
            SocketReaction reaction);
    }
}
