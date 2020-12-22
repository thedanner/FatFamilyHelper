using Discord;
using Microsoft.Extensions.Logging;

namespace Left4DeadHelper.Helpers.Extensions
{
    public static class LoggingExtensions
    {
        public static LogLevel ToLogLevel(this LogSeverity severity)
        {
            LogLevel level;
            switch (severity)
            {
                case LogSeverity.Critical: level = LogLevel.Critical; break;
                case LogSeverity.Error: level = LogLevel.Error; break;
                case LogSeverity.Warning: level = LogLevel.Warning; break;
                case LogSeverity.Info: level = LogLevel.Information; break;
                case LogSeverity.Verbose: level = LogLevel.Debug; break;
                case LogSeverity.Debug: level = LogLevel.Trace; break;
                default: level = LogLevel.Information; break;
            }
            return level;
        }
    }
}
