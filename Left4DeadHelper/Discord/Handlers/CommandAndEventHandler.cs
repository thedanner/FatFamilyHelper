using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Left4DeadHelper.Discord.Interfaces.Events;
using Left4DeadHelper.Discord.Modules;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Handlers
{
    public class CommandAndEventHandler : IDisposable
    {
        private readonly ILogger<CommandAndEventHandler> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly InteractionService _interactionService;
        private readonly Settings _settings;
        private readonly IServiceProvider _serviceProvider;

        private bool _disposedValue;

        // Retrieve client and CommandService instance via ctor
        public CommandAndEventHandler(
            ILogger<CommandAndEventHandler> logger, DiscordSocketClient client,
            CommandService commandService, InteractionService interactionService,
            Settings settings, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            _client.MessageReceived += HandleMessageReceivedAsync;
            _client.ReactionAdded += HandleReactionAddedAsync;

            await _commandService.AddModulesAsync(typeof(MinecraftPlayersModule).Assembly, _serviceProvider);

            await _interactionService.AddModulesAsync(typeof(MinecraftPlayersInteractiveModule).Assembly, _serviceProvider);

            _client.InteractionCreated += HandleInteraction;
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

        private async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> maybeCachedMessage,
            Cacheable<IMessageChannel, ulong> maybeCachedChannel, SocketReaction reaction)
        {
               var implementingServices = _serviceProvider.GetServices<IHandleReactionAddedAsync>();
        
            await Task.WhenAll(implementingServices.Select(s => s.HandleReactionAddedAsync(maybeCachedMessage, maybeCachedChannel, reaction)));
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to handle interaction (e.g., from a slash command).");

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
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
                    _client.InteractionCreated -= HandleInteraction;

                    (_commandService as IDisposable)?.Dispose();
                    _interactionService?.Dispose();
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
