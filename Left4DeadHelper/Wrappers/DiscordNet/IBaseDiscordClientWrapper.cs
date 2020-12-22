using Discord;
using System;
using System.Threading.Tasks;

namespace Left4DeadHelper.Wrappers.DiscordNet
{
    public interface IBaseDiscordClientWrapper : IDiscordClient, IDisposable
    {
        LoginState LoginState { get; }
        
        event Func<LogMessage, Task> Log;
        event Func<Task> LoggedIn;
        event Func<Task> LoggedOut;

        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
        Task LogoutAsync();
    }
}
