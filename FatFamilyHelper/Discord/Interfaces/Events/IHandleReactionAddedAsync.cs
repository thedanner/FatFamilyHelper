using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Interfaces.Events;

public interface IHandleReactionAddedAsync : IHandleDiscordEvents
{
    Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> maybeCachedMessage,
        Cacheable<IMessageChannel, ulong> maybeCachedChannel, SocketReaction reaction);
}
