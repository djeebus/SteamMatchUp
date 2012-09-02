using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SteamMatchUp
{
	public interface IWebpageCache
	{
		XmlDocument GetContent(Uri url, bool scrubHtml);
	}
}
