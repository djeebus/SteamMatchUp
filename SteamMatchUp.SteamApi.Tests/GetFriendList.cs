using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace SteamMatchUp.SteamApi.Tests
{
    [TestClass]
    public class GetFriendList
    {
        [TestMethod]
        public void HappyPath()
        {
            var api = new SteamApiClient();
            api.Key = Constants.ValidApiKey;

            var response = api.GetFriendList(Constants.ValidSteamId);

            Assert.IsNotNull(response);
            Assert.AreNotEqual(0, response.Length);

            foreach (var friend in response)
            {
                Assert.IsNotNull(friend);
                Assert.IsNotNull(friend.SteamId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(WebException))]
        public void InvalidUserId()
        {
            var api = new SteamApiClient();
            api.Key = Constants.ValidApiKey;

            var results = api.GetFriendList(Constants.InvalidSteamId);
        }
    }
}
