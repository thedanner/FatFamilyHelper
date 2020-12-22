using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Handlers
{
    public class CommandHandler : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Settings _settings;
        private readonly IServiceProvider _serviceProvider;

        private bool disposedValue;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commandService, Settings settings, IServiceProvider serviceProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commandService.AddModulesAsync(typeof(MoveChannelsModule).Assembly, _serviceProvider);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;
            
            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(_settings.DiscordSettings.Prefixes.Any(p => message.HasCharPrefix(p, ref argPos)) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var result = await _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _serviceProvider);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _client.MessageReceived -= HandleCommandAsync;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CommandHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
