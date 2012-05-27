using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SteamMatchUp
{
    public class HttpClient : IHttpClient
    {
        public string GetContent(Uri uri, Dictionary<System.Net.HttpRequestHeader, string> headers)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            var client = new WebClient();

            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    client.Headers.Add(key, headers[key]);
                }
            }

            return client.DownloadString(uri);
        }
    }
}
