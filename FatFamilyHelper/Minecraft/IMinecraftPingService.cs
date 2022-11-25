using FatFamilyHelper.Minecraft.Models;
using System.Threading.Tasks;

namespace FatFamilyHelper.Minecraft;

public interface IMinecraftPingService
{
    Task<PingPayload?> PingAsync(string hostname, ushort port);
}
