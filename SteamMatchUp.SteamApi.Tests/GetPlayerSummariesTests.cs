using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.SteamApi.Tests
{
    [TestClass]
    public class GetPlayerSummariesTests
    {
        [TestMethod]
        public void InvalidSteamId()
        {
            var api = new SteamApi.SteamApiClient();

            api.Key = Constants.ValidApiKey;

            string userId = Constants.ValidSteamId;

            var response = api.GetPlayerSummaries(new[] { userId });

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Length);

            var user = response[0];
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.SteamId);
            Assert.AreEqual(CommunityVisibilityState.FriendsOfFriends, user.CommunityVisibilityState);
            //Assert.AreEqual(PersonaState.Offline, user.PersonaState);
            Assert.AreEqual(new Uri("http://steamcommunity.com/id/djeebus/"), user.ProfileUrl);
            Assert.IsNotNull(user.Avatar);
            Assert.IsNotNull(user.AvatarFull);
            Assert.IsNotNull(user.AvatarMedium);
            //Assert.AreEqual(0, user.LastLogOffTime);
            Assert.AreEqual("Joseph", user.RealName);
        }
    }
}
