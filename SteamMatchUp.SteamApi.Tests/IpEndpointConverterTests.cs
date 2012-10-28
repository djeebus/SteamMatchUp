using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace SteamMatchUp.SteamApi.Tests
{
    [TestClass]
    public class IpEndpointConverterTests
    {
        class TestResult
        {
            public IPEndPoint Endpoint { get; set; }
        }

        [TestMethod]
        public void ConvertIpEndpoint()
        {
            var expected = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

            var json = "{ \"Endpoint\": \"127.0.0.1:8080\" }";

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestResult>(json, new IpEndpointConverter());

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Endpoint);
        }
    }
}
