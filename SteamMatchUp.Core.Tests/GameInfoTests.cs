using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class GameInfoTests
    {
        [TestMethod]
        public void ToString()
        {
            var info = new GameInfo
            {
                IconUrl = new Uri("http://www.google.com/"),
                Name = "this is a name",
                Genres = new string[] { "world", "hello" },
                Features = new string[] { "hello", "world" },
            };

            info.ToString();
        }
    }
}
