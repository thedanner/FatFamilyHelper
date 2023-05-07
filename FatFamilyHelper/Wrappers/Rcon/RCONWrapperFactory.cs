using CoreRCON;
using FatFamilyHelper.Models.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net;

namespace FatFamilyHelper.Wrappers.Rcon;

public class RCONWrapperFactory : IRCONWrapperFactory
{
    private readonly Left4DeadSettings _left4DeadSettings;

    public RCONWrapperFactory(IOptions<Left4DeadSettings>? left4DeadSettings)
    {
        if (left4DeadSettings is null) throw new ArgumentNullException(nameof(left4DeadSettings));

        _left4DeadSettings = left4DeadSettings.Value;
    }

    public IRCONWrapper GetRcon()
    {
        var serverInfo = _left4DeadSettings.ServerInfo;

        var addresses = Dns.GetHostAddresses(_left4DeadSettings.ServerInfo.Ip);

        var endpoint = new IPEndPoint(addresses[0], _left4DeadSettings.ServerInfo.Port);

        var rcon = new RCON(endpoint, serverInfo.RconPassword);

        var wrapper = new RCONWrapper(rcon);

        return wrapper;
    }
}
