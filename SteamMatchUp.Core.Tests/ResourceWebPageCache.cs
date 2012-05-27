using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Resources;
using System.Reflection;
using System.Diagnostics;

namespace SteamMatchUp.Core.Tests
{
    public class ResourceWebPageCache : IWebpageCache
    {
        static readonly Assembly assembly = typeof(ResourceWebPageCache).Assembly;

        public XmlDocument GetContent(Uri url)
        {
            Trace.WriteLine(url);
            var resourceKey = string.Format("SteamMatchUp.Core.Tests.{0}", url.Segments[2]).Replace("/", string.Empty);
            Trace.WriteLine(resourceKey);

            var stream = assembly.GetManifestResourceStream(resourceKey);
            if (stream == null)
                return null;

            var doc = new XmlDocument();
            doc.Load(stream);
            return doc;
        }
    }
}
