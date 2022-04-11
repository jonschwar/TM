using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TM.TwitterMonitoring;

namespace TM.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashtagStatsController : ControllerBase
    {
        private readonly IHashtagDashboardReader _dashboardReader;

        public HashtagStatsController(IHashtagDashboardReader dashboardReader)
        {
            _dashboardReader = dashboardReader;
        }

        [HttpGet]
        public async Task<TweetStats> GetAsync()
        {
            return await _dashboardReader.ReadCurrentStats();
        }
    }
}
