using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using System.IO;
using System.Xml;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class WebPageCacheTests
    {
        Moq.Mock<IWebpageCleaner> _webpageCleaner = new Moq.Mock<IWebpageCleaner>();
        Moq.Mock<IHttpClient> _httpClient = new Moq.Mock<IHttpClient>();

        private WebpageCache GetMockCache(string rootPath = @".\temp")
        {
            return new SteamMatchUp.WebpageCache(rootPath, _webpageCleaner.Object , _httpClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelativeRootInNonWebApp()
        {
            System.Web.HttpContext.Current = null;
            GetMockCache("~/temp");
        }

        [TestMethod]
        public void RelativeRootInWebApp()
        {
            var request = new System.Web.HttpRequest("~/abc.html", "http://www.google.com/", "?123=123");
            var response = new System.Web.HttpResponse(new StringWriter());
            var context = new System.Web.HttpContext(request, response);

            System.Web.HttpContext.Current = context;

            GetMockCache("~/temp");
        }

        [TestMethod]
        public void TestAbsolutePath()
        {
            var uri = new Uri("http://localhost./test/");

            this._httpClient
                .Setup(c => c.GetContent(uri, It.IsAny<Dictionary<System.Net.HttpRequestHeader, string>>()))
                .Returns("<magical>hello</magical>");

            var cache = GetMockCache(@"C:\temp\steamtests\");

            var content = cache.GetContent(uri, true);
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
            var cache = GetMockCache();

            cache.GetContent(null, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam1()
        {
            new WebpageCache(null, _webpageCleaner.Object, _httpClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam2()
        {
            new WebpageCache("abc", null, _httpClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullConstructorParam3()
        {
            new WebpageCache("abc", _webpageCleaner.Object, null);
        }

        [TestMethod]
        public void TestRepeatDownload()
        {
            var raw = "<h>1</h>";

            var doc = new XmlDocument();
            doc.LoadXml(raw);

            var uri = new Uri("http://temp2/temp2");

            _httpClient
                .Setup(s => s.GetContent(uri, null))
                .Returns(raw);

            _webpageCleaner
                .Setup(s => s.GetDocFromContent(It.IsAny<string>()))
                .Returns(doc);

            var cache = GetMockCache();

            var content1 = cache.GetContent(uri, true);
            var content2 = cache.GetContent(uri, true);

            Assert.IsNotNull(content1);
            Assert.IsNotNull(content2);
            Assert.AreEqual(content1.OuterXml, content2.OuterXml);
        }

        [TestMethod]
        public void TestMultithreadDownload()
        {
            var uri = new Uri("http://temp/temp");

            int x = 0;
            this._httpClient
                .Setup(s => s.GetContent(uri, It.IsAny<Dictionary<System.Net.HttpRequestHeader, string>>()))
                .Callback(() =>
                {
                    if (x != 0)
                        throw new ArgumentOutOfRangeException("x", x, "Called too often");

                    x++;
                })
                .Returns("<hello>world</hello>");

            this._webpageCleaner
                .Setup(s => s.GetDocFromContent(It.IsAny<string>()))
                .Returns<string>(s =>
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(s);
                    return doc;
                });
            
            var cache = GetMockCache();

            var total = 64;

            var tasks = (from i in Enumerable.Range(0, total)
                         select new Task(() => { cache.GetContent(uri, true); })).ToArray();

            foreach (var task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks);

            Assert.AreEqual(total, tasks.Length);
        }
    }
}
