using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TM.TwitterTypes;

namespace TM.TwitterMonitoring.Tests
{
    [TestClass]
    public class TwitterMonitoringTests
    {
        private readonly Tweet _tweet1 = new Tweet
        {
            Id = "12312321",
            Hashtags = new List<string>(new string[] { "Tag1", "Tag2", "Tag3" })
        };

        //TODO: More test coverage needed!

        [TestMethod]
        public async Task ReadWrite_To_Dashboard_Success()
        {
            var dashboard = new HashtagDashboard();

            IHashtagDashboardReader reader = dashboard;

            var stats = await reader.ReadCurrentStats();

            Assert.IsNotNull(stats, "Dashboard should report no stats");
            Assert.IsTrue(stats.TotalTweetCount == 0, "Total tweet count should be zero");
            Assert.IsTrue(stats.Hashtags.Count == 0, "No hashtags should be reported");

            IHashtagDashboardWriter writer = dashboard;

            writer.WriteHashtag(_tweet1);

            stats = await reader.ReadCurrentStats();

            Assert.IsNotNull(stats, "Dashboard should have data to report");
            Assert.AreEqual(1, stats.TotalTweetCount, "Invalid tweet count read from dashboard");
            Assert.AreEqual(3, stats.Hashtags.Count, "Invalid hashtag count read from dashboard");

            var tag1 = (from tag in stats.Hashtags where tag.Tag == "Tag1" select tag).FirstOrDefault();
            Assert.IsNotNull(tag1, "Tag1 missing from reported tags");
            Assert.IsTrue(tag1.Count == 1, "Tag1 incorrectly ranked");
            var tag2 = (from tag in stats.Hashtags where tag.Tag == "Tag2" select tag).FirstOrDefault();
            Assert.IsNotNull(tag1, "Tag2 missing from reported tags");
            Assert.IsTrue(tag2.Count == 1, "Tag2 incorrectly ranked");
            var tag3 = (from tag in stats.Hashtags where tag.Tag == "Tag3" select tag).FirstOrDefault();
            Assert.IsTrue(tag3.Count == 1, "Tag3 incorrectly ranked");

        }
    }
}
