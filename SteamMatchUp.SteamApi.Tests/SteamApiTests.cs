using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.SteamApi.Tests
{
    [TestClass]
    public class SteamApiTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidKeyException))]
        public void InvalidApiKeyShouldThrowException()
        {
            var api = new SteamApi.SteamApiClient();

            api.Key = Constants.InvalidApiKey;

            var response = api.GetPlayerSummaries(new[] { Constants.InvalidSteamId });
        }
    }
}
