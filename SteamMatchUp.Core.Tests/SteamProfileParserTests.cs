using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class SteamProfileParserTests
    {
        static readonly IWebpageCache _cache = new ResourceWebPageCache();

        [TestMethod]
        public void GetFriendsWithInvalidUserId()
        {
            var parser = new SteamProfileParser(_cache);

            var friends = parser.GetFriends("invalid");
            Assert.IsNull(friends);
        }

        [TestMethod]
        public void TestFriends()
        {
            var parser = new SteamProfileParser(_cache);

            var friends = parser.GetFriends("djeebus-friends.xhtml");
            Assert.IsNotNull(friends);
            Assert.AreEqual("djeebus", friends.Username);
            Assert.AreEqual(25, friends.Count);

            foreach (var friend in friends)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(friend.CommunityUrl));
                Assert.IsFalse(string.IsNullOrWhiteSpace(friend.IconUrl));
                Assert.IsFalse(string.IsNullOrWhiteSpace(friend.Id));
                Assert.IsFalse(string.IsNullOrWhiteSpace(friend.Username));
            }
        }

        [TestMethod]
        public void TestGames()
        {
            var parser = new SteamProfileParser(_cache);

            var result = parser.GetGames("djeebus-games.xhtml");

            Assert.AreEqual("djeebus", result.Username);
            Assert.AreEqual(231, result.Count);
        }

        [TestMethod]
        public void TestUnknownUserGame()
        {
            var parser = new SteamProfileParser(_cache);

            var result = parser.GetGames("invalid");

            Assert.IsNull(result);
        }
    }
}
