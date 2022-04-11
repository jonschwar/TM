using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TM.TwitterTypes;

namespace TM.TwitterMonitoring
{
    public class HashtagDashboard : IHashtagDashboardWriter, IHashtagDashboardReader
    {
        private readonly ConcurrentDictionary<string, long> _tags = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, long> _totalTweetCount = new ConcurrentDictionary<string, long>();
        private const string KEY_TO_TOTAL_COUNT = "TotalTweetCount";
        
        public async void WriteHashtag(Tweet tweet)
        {
            await Task.Run(() =>
            {
                foreach (var hashTag in tweet.Hashtags)
                {
                    long hashTagOccurenceCount;

                    if (_tags.TryGetValue(hashTag, out hashTagOccurenceCount))
                    {
                        _ = _tags.TryUpdate(hashTag, hashTagOccurenceCount + 1, hashTagOccurenceCount);
                    }
                    else
                    {
                        _ = _tags.TryAdd(hashTag, 1);
                    }
                }
            });

            await Task.Run(() =>
            {
                long currentTweetCount;

                if(_totalTweetCount.TryGetValue(KEY_TO_TOTAL_COUNT, out currentTweetCount))
                {
                    _ = _totalTweetCount.TryUpdate(KEY_TO_TOTAL_COUNT, currentTweetCount + 1, currentTweetCount);
                }
                else
                {
                    _ = _totalTweetCount.TryAdd(KEY_TO_TOTAL_COUNT, 1);
                }
            });
        }

        public async Task<TweetStats> ReadCurrentStats()
        {
            var totalTagCount = await Task.Run(() => _tags.Count());
            var hashtagValuePairs = await Task.Run(() => _tags.ToArray().OrderByDescending(x => x.Value).Take(10).ToList());

            long currentTweetCount;

            _ = _totalTweetCount.TryGetValue(KEY_TO_TOTAL_COUNT, out currentTweetCount);

            return new TweetStats
            {
                TotalTweetCount = currentTweetCount,
                Hashtags = new List<HashStat>(from tag in hashtagValuePairs 
                                              select new HashStat(tag.Key, tag.Value))
            };
        }
    }

    public class TweetStats
    {
        public long TotalTweetCount { get; set; }
        public List<HashStat> Hashtags { get; set; } = new List<HashStat>();
    }

    public struct HashStat
    {
        public HashStat(string tag, long count)
        {
            Tag = tag;
            Count = count;
        }
        public string Tag { get; set; }
        public long Count { get; set; }
    }
}
