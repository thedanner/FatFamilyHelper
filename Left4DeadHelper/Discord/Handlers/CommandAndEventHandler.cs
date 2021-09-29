using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Handlers
{
    public class CommandAndEventHandler : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Settings _settings;
        private readonly IServiceProvider _serviceProvider;

        private bool _disposedValue;

        // Retrieve client and CommandService instance via ctor
        public CommandAndEventHandler(DiscordSocketClient client, CommandService commandService, Settings settings,
             IServiceProvider serviceProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            _client.MessageReceived += HandleMessageReceivedAsync;
            _client.ReactionAdded += HandleReactionAddedAsync;

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
            if (message.Author.IsBot) return;

            var isPrivateChannel = message.Channel is IPrivateChannel;
            var hasAnyPrefix = _settings.DiscordSettings.Prefixes.Any(p => message.HasCharPrefix(p, ref argPos));
            if ((!isPrivateChannel
                && !(hasAnyPrefix || message.HasMentionPrefix(_client.CurrentUser, ref argPos))))
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

        private async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            var implementingServices = _serviceProvider.GetServices<IHandleMessageReceivedAsync>();

            await Task.WhenAll(implementingServices.Select(s => s.HandleMessageReceivedAsync(message)));
        }

        private async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> maybeCachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var implementingServices = _serviceProvider.GetServices<IHandleReactionAddedAsync>();

            await Task.WhenAll(implementingServices.Select(s => s.HandleReactionAddedAsync(maybeCachedMessage, channel, reaction)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client.MessageReceived -= HandleCommandAsync;
                    _client.MessageReceived -= HandleMessageReceivedAsync;
                    _client.ReactionAdded -= HandleReactionAddedAsync;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
