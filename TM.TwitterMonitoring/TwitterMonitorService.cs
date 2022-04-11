using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TM.TwitterClients.Monitoring;

namespace TM.TwitterMonitoring
{
    public class TwitterMonitorService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostAppLifeTime;
        private readonly IHashtagMonitor _tweetMonitor;
        private readonly IHashtagDashboardWriter _dashboardWriter;

        public TwitterMonitorService(IHashtagMonitor tweetMonitor, IHostApplicationLifetime hostAppLifeTime, IHashtagDashboardWriter dashboardWriter)
        {
            _tweetMonitor = tweetMonitor;
            _hostAppLifeTime = hostAppLifeTime;
            _dashboardWriter = dashboardWriter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostAppLifeTime.ApplicationStarted.Register(OnAppStarted);
            _hostAppLifeTime.ApplicationStopping.Register(OnStopping);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async void OnAppStarted()
        {
            await foreach (var tweet in _tweetMonitor.ReadStream())
            {
                if (tweet != null)
                {
                    if (tweet.Hashtags != null && tweet.Hashtags.Count > 0)
                    {
                        _dashboardWriter.WriteHashtag(tweet);
                    }
                }
            }
        }

        public void OnStopping()
        {

        }
    }
}
