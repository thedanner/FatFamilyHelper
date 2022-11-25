using System;

namespace FatFamilyHelper.Minecraft;

public class CanPingProvider : ICanPingProvider
{
    private readonly object _lock = new();

    private readonly TimeSpan _minimumWait;

    private DateTimeOffset _lastPing;

    public CanPingProvider() : this(TimeSpan.FromSeconds(10))
    {

    }

    public CanPingProvider(TimeSpan minimumWait)
    {
        if (minimumWait < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(minimumWait), "Value must be positive.");

        _minimumWait = minimumWait;
    }

    public bool TryCanPing()
    {
        lock (_lock)
        {
            var now = DateTimeOffset.Now;
            if (_lastPing + _minimumWait < now)
            {
                _lastPing = now;
                return true;
            }
            return false;
        }
    }
}
