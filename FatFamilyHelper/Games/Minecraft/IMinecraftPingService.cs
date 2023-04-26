﻿using FatFamilyHelper.Games.Minecraft.Models;
using System.Threading.Tasks;

namespace FatFamilyHelper.Games.Minecraft;

public interface IMinecraftPingService
{
    Task<MinecraftPingPayload?> PingAsync(string hostname, ushort port);
}
