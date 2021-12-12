using Left4DeadHelper.Minecraft.Models;
using System.Threading.Tasks;

namespace Left4DeadHelper.Minecraft;

public interface IMinecraftPingService
{
    Task<PingPayload?> PingAsync(string hostname, ushort port);
}
