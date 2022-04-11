using System.Threading.Tasks;

namespace TM.TwitterMonitoring
{
    public interface IHashtagDashboardReader
    {
        public Task<TweetStats> ReadCurrentStats();
    }
}
