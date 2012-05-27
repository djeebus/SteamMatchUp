using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SteamMatchUp
{
    public interface IHttpClient
    {
        string GetContent(Uri uri, Dictionary<System.Net.HttpRequestHeader, string> headers);
    }
}
