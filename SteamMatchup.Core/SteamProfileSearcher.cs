using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SteamMatchUp
{
    public class SteamProfileSearcher : ISteamProfileSearcher
    {
        private readonly IWebpageCache _cache;
        private readonly IWebpageCleaner _cleaner;

        const string SearchUrlFormat = @"http://steamcommunity.com/actions/Search?K={0}";

        public SteamProfileSearcher(IWebpageCache cache, IWebpageCleaner cleaner)
        {
            this._cache = cache;
            this._cleaner = cleaner;
        }

        public ProfileSearchResult[] Search(string text)
        {
            var uri = string.Format(SearchUrlFormat, System.Web.HttpUtility.UrlEncode(text));

            var doc = this._cache.GetContent(new Uri(uri));

            var results = doc.SelectNodes("//div[contains(@class, 'resultItem')]");

            if (results.Count == 0)
                return new ProfileSearchResult[0];

            var toreturn = new List<ProfileSearchResult>();

            foreach (XmlElement result in results)
            {
                var xPgTag = result.SelectSingleNode(".//*[@class='pgtag']");

                if (xPgTag == null || xPgTag.InnerText != "Player")
                    continue;

                var xName = result.SelectSingleNode(".//*[@class='linkTitle']");
                if (xName == null)
                    continue;

                var profileUrlString = xName.Attributes["href"].Value;
                if (string.IsNullOrWhiteSpace(profileUrlString))
                    continue;

                var profileUrl = new Uri(profileUrlString);

                var xUrl = result.SelectSingleNode(".//*[@class='avatarMedium']//img");
                if (xUrl == null)
                    continue;

                var aImgSrc = xUrl.Attributes["src"];
                if (aImgSrc == null)
                    continue;

                var model = new ProfileSearchResult
                {
                    IconUrl = new Uri(aImgSrc.Value),
                    Id = profileUrl.Segments[profileUrl.Segments.Length - 1],
                    Username = xName.InnerText,
                };

                toreturn.Add(model);
            }

            return toreturn.ToArray();
        }
    }
}
