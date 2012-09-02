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
        const string ValidSteamId = "76561197961887342";
        static readonly Uri ValidSteamProfileUrl = new Uri("http://steamcommunity.com/id/djeebus/");

        [TestMethod]
        public void GetGames()
        {
            var parser = GetProfileParser();

            var games = parser.GetGames(ValidSteamProfileUrl);

            Assert.IsNotNull(games);
            Assert.IsNotNull(games.Username);

            Assert.AreNotEqual(0, games.Count);

            foreach (var game in games)
            {
                Assert.IsNotNull(game);
                Assert.IsNotNull(game.IconUrl);
                Assert.IsNotNull(game.Id);
                Assert.IsNotNull(game.Name);
                Assert.IsNotNull(game.SteamUrl);
            }
        }

        [TestMethod]
        public void GetFriends()
        {
            var parser = GetProfileParser();

            var friends = parser.GetFriends(ValidSteamProfileUrl);

            Assert.IsNotNull(friends);
            Assert.AreNotEqual(0, friends.Count);

            foreach (var friend in friends)
            {
                Assert.IsNotNull(friend);
                Assert.IsNotNull(friend.CommunityUrl);
                Assert.IsNotNull(friend.IconUrl);
                Assert.IsNotNull(friend.Id);
                Assert.IsNotNull(friend.Username);
            }
        }

        private static SteamProfileParser GetProfileParser()
        {
            string rootDir = AppDomain.CurrentDomain.DynamicDirectory ?? AppDomain.CurrentDomain.BaseDirectory;

            var cleaner = new WebpageCleaner();
            var client = new HttpClient();

            var cache = new WebpageCache(rootDir, cleaner, client);

            var parser = new SteamProfileParser(cache);
            return parser;
        }

        [TestMethod]
        public void GetUser()
        {
            string rootDir = AppDomain.CurrentDomain.DynamicDirectory ?? AppDomain.CurrentDomain.BaseDirectory;

            var cleaner = new WebpageCleaner();
            var client = new HttpClient();

            var cache = new WebpageCache(rootDir, cleaner, client);

            var parser = new SteamProfileParser(cache);

            var user = parser.GetUser(ValidSteamProfileUrl);

            Assert.IsNotNull(user);
            Assert.IsNotNull(user.CommunityUrl);
            Assert.IsNotNull(user.IconUrl);
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.Username);
        }
    }
}
