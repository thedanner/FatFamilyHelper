using Left4DeadHelper.Wrappers.DiscordNet;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Services;

public interface IDiscordConnectionBootstrapper
{
    Task StartAsync(IDiscordSocketClientWrapper client, CancellationToken cancellationToken);
}
