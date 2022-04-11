using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using TM.TwitterTypes;

namespace TM.TwitterClients.Parsing
{
    internal static class ParsingExtensions
    {
        public static async IAsyncEnumerable<Tweet> ReadTweetDataAsync(this HttpContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Stream contentStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

            using (contentStream)
            {
                using (StreamReader contentStreamReader = new StreamReader(contentStream))
                {

                    while (!contentStreamReader.EndOfStream)
                    {
                        string line = await contentStreamReader.ReadLineAsync().ConfigureAwait(false);
                        if (line != null)
                        {
                            JToken id = null;
                            List<string> hashtags = null;

                            try
                            {
                                id = JObject.Parse(line)["data"]["id"].ToString();
                                hashtags = JObject.Parse(line)["data"]?["entities"]?["hashtags"]?.Children()["tag"].Values<string>().ToList();
                            }
                            catch { }

                            yield return new Tweet
                            {
                                Id = ( id != null) ? id.Value<string>() : String.Empty,
                                Hashtags = hashtags != null ? hashtags : new List<string>()
                            };
                        }
                    }
                }
            }
        }
    }
}
