using CoreRCON;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FatFamilyHelper.Wrappers.Rcon;

public class RCONWrapperFactory : IRCONWrapperFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RCONWrapperFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRCONWrapper GetRcon() => new RCONWrapper(_serviceProvider.GetRequiredService<RCON>());
}
