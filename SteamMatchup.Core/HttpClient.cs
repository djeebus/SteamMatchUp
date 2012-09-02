using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace SteamMatchUp
{
    public class HttpClient : IHttpClient
    {
        public string GetContent(Uri uri, Dictionary<System.Net.HttpRequestHeader, string> headers)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            using (var client = new WebClient())
            {
                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        client.Headers.Add(key, headers[key]);
                    }
                }

                DateTime start = DateTime.Now;
                var content = client.DownloadString(uri);
                var elapsed = DateTime.Now - start;

                Trace.WriteLine(string.Format("{0}: \n{1}", uri, content.Length));

                Trace.WriteLine(string.Format("Downloading '{0}' took {1} milliseconds", uri, elapsed.TotalMilliseconds));

                return content;
            }
        }
    }
}
