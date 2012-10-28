using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SteamMatchUp
{
    public interface IWebpageCleaner
    {
        XmlDocument GetDocFromContent(string content);
    }
}
