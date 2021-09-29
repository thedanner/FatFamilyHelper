using Discord.WebSocket;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Interfaces.Events
{
    public interface IHandleMessageReceivedAsync : IHandleDiscordEvents
    {
        Task HandleMessageReceivedAsync(SocketMessage message);
    }
}
