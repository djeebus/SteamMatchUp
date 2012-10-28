using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

using Sgml;

namespace SteamMatchUp
{
	public class WebpageCleaner : IWebpageCleaner
	{
		public XmlDocument GetDocFromContent(string content)
		{
			var start = DateTime.Now;
			XmlDocument doc = new XmlDocument();

			using (var streamReader = new StringReader(content))
			{
                SgmlReader sgmlReader = new SgmlReader
                {
                    DocType = "HTML",
                    InputStream = streamReader,
                };

				doc.XmlResolver = null;
				doc.Load(sgmlReader);
			}

			var duration = DateTime.Now - start;
			Trace.WriteLine(string.Format("Cleansed html in {0} milliseconds", duration.TotalMilliseconds), "WebpageCleaner");

			return doc;
		}
	}
}
