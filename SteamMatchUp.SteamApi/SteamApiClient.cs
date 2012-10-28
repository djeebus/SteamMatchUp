using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Diagnostics;

namespace SteamMatchUp.SteamApi
{
    public class ApiMethod
    {
        private readonly string _url;

        public string InterfaceName { get; private set; }
        public string MethodName { get; private set; }
        public string Version { get; private set; }

        const string UrlFormat = "http://api.steampowered.com/{interface}/{method}/v{version}/";

        public string Url
        {
            get { return this._url; }
        }

        public ApiMethod(string interfaceMethod, string methodName, string version)
        {
            this.InterfaceName = interfaceMethod;
            this.MethodName= methodName;
            this.Version = version;

            this._url = CreateSteamUri();
        }

        private string CreateSteamUri()
        {
            var sb = new StringBuilder(UrlFormat);
            sb.Replace("{interface}", this.InterfaceName);
            sb.Replace("{method}", this.MethodName);
            sb.Replace("{version}", this.Version);

            return sb.ToString();
        }
    }

    public class SteamApiClient : ISteamApi
    {
        public string Key { get; set; }

        static readonly ApiMethod _playerSummaries = new ApiMethod("ISteamUser", "GetPlayerSummaries", "0002");
        public PlayerSummary[] GetPlayerSummaries(string[] steamIds)
        {
            var qs = new NameValueCollection();
            foreach (var id in steamIds)
            {
                qs.Add("steamids", id);
            }

            var result = MakeApiCall<GetPlayerSummariesResponseWrapper>(_playerSummaries, qs);

            return result.Response.Players;
        }

        static readonly ApiMethod _getFriendList = new ApiMethod("ISteamUser", "GetFriendList", "0001");
        public FriendSummary[] GetFriendList(string steamId)
        {
            var qs = new NameValueCollection();
            qs.Add("steamid", steamId);
            qs.Add("relationship", "all");

            var result = MakeApiCall<GetFriendSummaryResponseWrapper>(_getFriendList, qs);

            return result.FriendsList.Friends;
        }

        private T MakeApiCall<T>(ApiMethod apiMethod, NameValueCollection qs)
        {
            qs = qs ?? new NameValueCollection();
            qs.Add("key", this.Key);
            qs.Add("format", "json"); // faster than xml

            var url = CreateUrl(apiMethod.Url, qs);

            string response;

            using (var client = new WebClient())
            {
                try
                {
                    response = client.DownloadString(url);

                    Trace.WriteLine(string.Format("{0}: {1}", url, response));
                }
                catch (WebException wex)
                {
                    if (wex.Response == null)
                        throw;

                    if (!(wex.Response is HttpWebResponse))
                        throw;

                    var httpWebResponse = (HttpWebResponse)wex.Response;
                    if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
                        throw new InvalidKeyException(wex);

                    throw;
                }
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response, converters);
        }

        static readonly Newtonsoft.Json.JsonConverter[] converters = new[]
        {
            new IpEndpointConverter(),
        };

        private Uri CreateUrl(string url, NameValueCollection qs)
        {
            return new Uri(string.Format("{0}?{1}",
                url,
                string.Join("&", from k in qs.AllKeys
                                 select string.Format("{0}={1}", k, qs[k]))));
        }
    }
}
