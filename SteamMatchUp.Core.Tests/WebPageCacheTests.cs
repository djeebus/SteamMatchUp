using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class WebPageCacheTests
    {
        class MockCleaner : IWebpageCleaner
        {
            public System.Xml.XmlDocument GetDocFromContent(string content)
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(content);
                return doc;
            }
        }

        class MockHttpClient : IHttpClient
        {
            string _content;

            public int TotalCalls { get; set; }

            public MockHttpClient(string content)
            {
                this._content = content;
            }

            public string GetContent(Uri uri, Dictionary<System.Net.HttpRequestHeader, string> headers)
            {
                this.TotalCalls++;

                return this._content;
            }
        }

        [TestMethod]
        public void TestRelativePath()
        {
            var staticContent = "<magical>hello</magical>";

            var cache = GetMockCache(staticContent, null);

            var content = cache.GetContent(new Uri("http://localhost./test/"));
            Assert.IsNotNull(content);
            Assert.IsTrue(content.ChildNodes.Count == 1);

            var xMagical = content.ChildNodes[0];
            Assert.IsNotNull(xMagical);
            Assert.IsTrue(xMagical.LocalName == "magical");
            Assert.IsTrue(xMagical.InnerText == "hello");
        }

        [TestMethod]
        public void TestAbsolutePath()
        {
            var staticContent = "<magical>hello</magical>";

            var cache = GetMockCache(staticContent, @"C:\temp\steamtests\");

            var content = cache.GetContent(new Uri("http://localhost./test/"));
            Assert.IsNotNull(content);
            Assert.IsTrue(content.ChildNodes.Count == 1);

            var xMagical = content.ChildNodes[0];
            Assert.IsNotNull(xMagical);
            Assert.IsTrue(xMagical.LocalName == "magical");
            Assert.IsTrue(xMagical.InnerText == "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullUri()
        {
            var cache = GetMockCache("<magical></magical>", null);

            cache.GetContent(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam1()
        {
            new WebpageCache(null, new MockCleaner(), new HttpClient());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam2()
        {
            new WebpageCache("abc", null, new HttpClient());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam3()
        {
            new WebpageCache("abc", new MockCleaner(), null);
        }

        [TestMethod]
        public void TestRepeatDownload()
        {
            MockHttpClient client;
            var cache = GetMockCache("<magical></magical>", null, out client);
            var uri = new Uri("http://temp2/temp2");

            cache.GetContent(uri);
            cache.GetContent(uri);

            Assert.AreEqual(1, client.TotalCalls);
        }

        [TestMethod]
        public void TestMultithreadDownload()
        {
            MockHttpClient client;
            var cache = GetMockCache("<magical></magical>", null, out client);
            var uri = new Uri("http://temp/temp");

            var total = 100;

            var ids = new List<int>();
            var threads = (from x in Enumerable.Range(1, total)
                           let s = new ThreadStart(() => { 
                               cache.GetContent(uri); 
                               ids.Add(Thread.CurrentThread.ManagedThreadId); 
                           })
                           let t = new Thread(s)
                           select t).AsParallel();

            foreach (var thread in threads)
            {
                thread.Start();
            }

            var items = threads.ToArray();
            Assert.AreEqual(total, items.Length);
            Assert.AreEqual(1, client.TotalCalls);
            Assert.AreNotEqual(1, ids.GroupBy(id => id).Count());
        }

        private static WebpageCache GetMockCache(string staticContent, string rootPath)
        {
            MockHttpClient client;

            return GetMockCache(staticContent, rootPath, out client);
        }

        private static WebpageCache GetMockCache(string staticContent, string rootPath, out MockHttpClient client)
        {
            client = new MockHttpClient(staticContent);

            return new SteamMatchUp.WebpageCache(rootPath ?? ".\\temp", new MockCleaner(), client);
        }
    }
}
