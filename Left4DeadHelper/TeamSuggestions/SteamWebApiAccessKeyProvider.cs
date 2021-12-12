using Left4DeadHelper.Models.Configuration;

namespace Left4DeadHelper.TeamSuggestions;

public class SteamWebApiAccessKeyProvider : ISteamWebApiAccessKeyProvider
{
    public SteamWebApiAccessKeyProvider(Settings settings)
    {
        AccessKey = settings.SteamWebApi?.AccessKey;
    }

    public string? AccessKey { get; private set; }
}
