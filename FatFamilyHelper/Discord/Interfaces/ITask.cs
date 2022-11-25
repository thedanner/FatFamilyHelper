using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Discord.Interfaces;

public interface ITask
{
    Task RunTaskAsync(
        DiscordSocketClient client, IReadOnlyDictionary<string, object> taskSettings,
        CancellationToken cancellationToken);
}
