using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using TM.TwitterTypes;
using TM.TwitterClients.Parsing;

namespace TM.TwitterClients.Monitoring
{
    public class HashtagMonitor : IHashtagMonitor
    {
        private readonly TwitterMonitorSettings _settings;
        private readonly HttpClient _http;

        public HashtagMonitor(HttpClient httpClient, IOptions<TwitterMonitorSettings> settings)
        {
            _http = httpClient;
            _settings = settings.Value;
        }

        public async IAsyncEnumerable<Tweet> ReadStream()
        {
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.BearerToken}");

            using (var response = await _http.GetAsync($"{_settings.BaseAddress}/{_settings.Endpoint}", HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                await foreach(var tweet in response.Content.ReadTweetDataAsync())
                {
                    yield return tweet != null ? tweet: null;
                }
            }
        }
    }

}
