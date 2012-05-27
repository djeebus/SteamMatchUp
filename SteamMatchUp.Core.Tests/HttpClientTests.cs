using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class HttpClientTests
    {
        Uri goodUri = new Uri("http://www.google.com");

        Dictionary<System.Net.HttpRequestHeader, string> goodHeaders =
            new Dictionary<System.Net.HttpRequestHeader, string> { { System.Net.HttpRequestHeader.Accept, "hello" } };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUrlAndHeaders()
        {
            var client = new HttpClient();

            client.GetContent(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUrl()
        {
            var client = new HttpClient();

            client.GetContent(null, goodHeaders);
        }

        [TestMethod]
        public void NullHeaders()
        {
            var client = new HttpClient();

            var text = client.GetContent(goodUri, null);
        }

        [TestMethod]
        public void HappyPath()
        {
            var client = new HttpClient();

            var text = client.GetContent(goodUri, goodHeaders);
        }
    }
}
