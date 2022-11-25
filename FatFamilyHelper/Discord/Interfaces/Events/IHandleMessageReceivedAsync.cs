using Discord.WebSocket;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Interfaces.Events;

public interface IHandleMessageReceivedAsync : IHandleDiscordEvents
{
    Task HandleMessageReceivedAsync(SocketMessage message);
}
