using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class SteamGameParserTests
    {
        readonly IWebpageCache _cache = new ResourceWebPageCache();

        [TestMethod]
        public void TestCounterstrike()
        {
            var parser = new SteamGameParser(_cache);

            var info = parser.GetInfo("counterstrike.xhtml");
            Assert.AreEqual(info.Name, "Counter-Strike");

            Assert.AreEqual(1, info.Features.Length);
            Assert.AreEqual("Multi-player", info.Features[0]);

            Assert.AreEqual(1, info.Genres.Length);
            Assert.AreEqual("Action", info.Genres[0]);

            Assert.AreEqual(new Uri("http://cdn.steampowered.com/v/gfx/apps/10/header_292x136.jpg?t=1287184193"), info.IconUrl);
        }

        [TestMethod]
        public void TestDroplitz()
        {
            var parser = new SteamGameParser(this._cache);

            var info = parser.GetInfo("droplitz.xhtml");
            Assert.AreEqual(info.Name, "Droplitz");

            Assert.AreEqual(3, info.Features.Length);
            Assert.AreEqual("Single-player", info.Features[0]);
            Assert.AreEqual("Steam Achievements", info.Features[1]);
            Assert.AreEqual("Steam Leaderboards", info.Features[2]);

            Assert.AreEqual(2, info.Genres.Length);
            Assert.AreEqual("Casual", info.Genres[0]);
            Assert.AreEqual("Strategy", info.Genres[1]);
        }

        [TestMethod]
        public void TestMissingGame()
        {
            var parser = new SteamGameParser(this._cache);

            var info = parser.GetInfo("test.xhtml");
            Assert.IsNull(info);
        }
    }
}
