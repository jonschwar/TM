using System;
using System.Collections.Generic;
using TM.TwitterTypes;

namespace TM.TwitterClients.Monitoring
{
    public interface IHashtagMonitor
    {
        IAsyncEnumerable<Tweet> ReadStream();
    }
}
