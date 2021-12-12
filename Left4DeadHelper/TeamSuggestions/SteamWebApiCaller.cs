using Left4DeadHelper.TeamSuggestions.RestModels.ISteamUserStats;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Left4DeadHelper.TeamSuggestions;

public class SteamWebApiCaller : ISteamWebApiCaller
{
    private const string BaseUrl = "http://api.steampowered.com";

    private readonly HttpClient _httpClient;
    private readonly ISteamWebApiAccessKeyProvider _accessKeyProvider;

    public SteamWebApiCaller(HttpClient httpClient, ISteamWebApiAccessKeyProvider accessKeyProvider)
    {
        _httpClient = httpClient;
        _accessKeyProvider = accessKeyProvider;
    }

    private Uri BuildUri(string @interface, string method, int version, bool appendAuthKey, Dictionary<string, string>? query = null)
    {
        var queryBuilder = new StringBuilder();

        if (appendAuthKey)
        {
            var accessKey = _accessKeyProvider.AccessKey;
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new Exception("An auth key is required for this call.");
            }

            queryBuilder.Append($"key={HttpUtility.UrlEncode(accessKey)}");
        }

        if (query != null)
        {
            foreach (var kvp in query)
            {
                if (queryBuilder.Length != 0)
                {
                    queryBuilder.Append('&');
                }

                queryBuilder
                    .Append(HttpUtility.UrlEncode(kvp.Key))
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(kvp.Value));
            }
        }

        var uriBuilder = new UriBuilder()
        {
            Scheme = "https",
            Host = "api.steampowered.com",
            Port = -1,
            Path = $"/{@interface}/{method}/v{version}",
            Query = queryBuilder.ToString()
        };

        return uriBuilder.Uri;
    }

    private async Task<TOut> GetAsync<TOut>(Uri uri)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, uri);

        var httpResponseMessage = await _httpClient.SendAsync(message).ConfigureAwait(false);

        httpResponseMessage.EnsureSuccessStatusCode();

        using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<TOut>(contentStream).ConfigureAwait(false);

        //var stringValue = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        //var result = JsonSerializer.Deserialize<TOut>(stringValue);

        return result!;
    }

    public async Task<GetUserStatsForGame> GetUserStatsForGameAsync(SteamId steamId, int appId)
    {
        const string Interface = "ISteamUserStats";
        const string Method = "GetUserStatsForGame";
        const int Version = 2;

        var query = new Dictionary<string, string>
            {
                { "steamid", steamId.Value.ToString() },
                { "appId", appId.ToString() }
            };

        var uri = BuildUri(Interface, Method, Version, true, query);

        var response = await GetAsync<GetUserStatsForGame>(uri);

        return response;
    }
}
