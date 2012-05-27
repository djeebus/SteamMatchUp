using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamMatchUp.Core.Tests
{
    [TestClass]
    public class WebPageCleanerTests
    {
        [TestMethod]
        public void CleanUnclosedImgTag()
        {
            var cleaner = new WebpageCleaner();

            var doc = cleaner.GetDocFromContent("<html><img src=hello></html>");

            Assert.IsNotNull(doc);
            Assert.IsTrue(doc.ChildNodes.Count == 1);

            var xHtml = doc.ChildNodes[0];
            Assert.IsNotNull(xHtml);
            Assert.IsTrue(xHtml.LocalName == "html");
            Assert.IsTrue(xHtml.ChildNodes.Count == 1);

            var xImg = xHtml.ChildNodes[0];
            Assert.IsNotNull(xImg);
            Assert.IsTrue(xImg.LocalName == "img");
            Assert.IsTrue(xImg.Attributes.Count == 1);

            var aSrc = xImg.Attributes[0];
            Assert.IsNotNull(aSrc);
            Assert.IsTrue(aSrc.Name == "src");
            Assert.IsTrue(aSrc.Value == "hello");
        }
    }
}
