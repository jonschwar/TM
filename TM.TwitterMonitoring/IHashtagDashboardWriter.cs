
using TM.TwitterTypes;

namespace TM.TwitterMonitoring
{
    public interface IHashtagDashboardWriter
    {
        public void WriteHashtag(Tweet tweet);
    }
}
