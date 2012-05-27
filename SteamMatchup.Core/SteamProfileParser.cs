using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace SteamMatchUp
{
	public class SteamProfileParser : ISteamProfileParser
	{
		const string urlNameFormat = "http://steamcommunity.com/id/{0}/{1}";
		const string urlNumberFormat = "http://steamcommunity.com/profiles/{0}/{1}";

		const string gamesPage = "games?tab=all";
		const string friendsPage = "friends";

		static readonly Regex regex = new Regex(@"^\d+%", RegexOptions.Compiled);

		private readonly IWebpageCache _cache;

		public SteamProfileParser(IWebpageCache cache)
		{
			this._cache = cache;
		}

		public FriendCollection GetFriends(string steamCommunityId)
		{
			var doc = DownloadContent(steamCommunityId, friendsPage);
            if (doc == null)
                return null;

			string username = GetUserNameFromDoc(doc);

			var friendsContainer = doc.SelectSingleNode("//div[@id='memberList']");

			var friends = (from f in GetFriendsFromPage(friendsContainer)
						   orderby f.Username
						   select f).ToArray();

			var toreturn = new FriendCollection(friends)
			{
				Username = username ?? steamCommunityId,
			};

			return toreturn;
		}

		private string GetUserNameFromDoc(XmlDocument doc)
		{
            if (doc == null)
                throw new ArgumentNullException("doc");

			var x = doc.SelectSingleNode("//h1");
			if (x == null)
				return null;

			return x.InnerText.Trim();
		}

		private static IEnumerable<User> GetFriendsFromPage(XmlNode parent)
		{
			foreach (XmlElement friend in parent.SelectNodes("div"))
			{
				var a = friend.SelectSingleNode("p/a");
				var name = a.InnerText;
				var link = new Uri(a.Attributes["href"].Value);
				var id = link.Segments[2];
				var img = friend.SelectSingleNode(".//img").Attributes["src"].Value;

				yield return new User
				{
					Id = id,
					CommunityUrl = link.ToString(),
					IconUrl = img,
					Username = name,
				};
			}
		}

		public User GetUser(string steamCommunityId)
		{
			string actualPage;
			var doc = DownloadContent(steamCommunityId, string.Empty, out actualPage);

			var username = doc.SelectSingleNode("//h1").FirstChild.InnerText.Trim();
			var imageUrl = doc.SelectSingleNode("//div[@class='avatarFull']/img/@src").Value.Replace("_full", string.Empty).Trim();

			return new User
			{
				Id = steamCommunityId,
				Username = username,
				CommunityUrl = actualPage,
				IconUrl = imageUrl,
				Stats = new List<Stat>(GetStats(doc)),
			};
		}

		private IEnumerable<Stat> GetStats(XmlDocument doc)
		{
			foreach (XmlElement stat in doc.SelectNodes("//div[@class='statsItem']"))
			{
				yield return new Stat
				{
					Name = stat.ChildNodes[0].InnerText.Trim(),
					Value = stat.ChildNodes[1].InnerText.Trim(),
				};
			}
		}

		public GameCollection GetGames(string steamCommunityId)
		{
			var doc = DownloadContent(steamCommunityId, gamesPage);
            if (doc == null)
                return null;

			var gamesContainer = doc.SelectSingleNode("//div[@class='games_list_tab_content']");

			var js = gamesContainer.SelectSingleNode("//script[not(@src)]").InnerText.Trim();

			var rgGames = js.Substring(0, js.IndexOf(';') - 1); // get all the text until the first semicolon

			rgGames = rgGames.Substring("var rgGames = ".Length); // skip the assignment part of the statement

			var code = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(rgGames);

			var games = from g in code
						let id = (int)g["appid"]
						select new Game
						{
							Id = id.ToString(),
							Name = (string)g["name"],
							IconUrl = (string)g["logo"],
							SteamUrl = string.Format("http://store.steampowered.com/app/{0}", id),
						};

			return new GameCollection(games)
			{
				Username = GetUserNameFromDoc(doc) ?? steamCommunityId,
			};
		}

		private XmlDocument DownloadContent(string steamCommunityId, string page)
		{
			string url;
			return DownloadContent(steamCommunityId, page, out url);
		}

		private XmlDocument DownloadContent(string steamCommunityId, string page, out string actualPage)
		{
            actualPage = null;

			foreach (var format in new string[] { urlNameFormat, urlNumberFormat })
			{
				var url = new Uri(string.Format(format, steamCommunityId, page));

				var doc = this._cache.GetContent(url);
                if (doc == null)
                    continue;

				if (doc.OuterXml.Contains("The specified profile could not be found."))
					continue;

				actualPage = url.ToString();
				return doc;
			}

            return null;
		}
	}
}
