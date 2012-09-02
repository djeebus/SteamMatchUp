using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SteamMatchUp
{
	public class SteamProfileParser : ISteamProfileParser
	{
        const string userPage = "?xml=1";
		const string gamesPage = "games?tab=all&xml=1";
		const string friendsPage = "friends?xml=1";

		static readonly Regex regex = new Regex(@"^\d+%", RegexOptions.Compiled);

		private readonly IWebpageCache _cache;

		public SteamProfileParser(IWebpageCache cache)
		{
			this._cache = cache;
		}

		public FriendCollection GetFriends(Uri profileUrl)
		{
			var doc = DownloadContent(profileUrl, friendsPage);
            if (doc == null)
                return null;

			string username = GetUserNameFromDoc(doc);

			var friendsContainer = doc.SelectSingleNode("/friendsList/friends");

            var friendIds = GetFriendIdsFromPage(friendsContainer);

            var api = new SteamApi.SteamApiClient();
            api.Key = ConfigurationManager.AppSettings["Steam Api Key"];

            var friends = from friend in api.GetPlayerSummaries(friendIds.ToArray())
                          select new User
                          {
                              CommunityUrl = friend.ProfileUrl,
                              IconUrl = friend.AvatarMedium,
                              Id = friend.SteamId,
                              Username = friend.PersonaName,
                          };

			var toreturn = new FriendCollection(friends)
			{
				SteamId = username,
			};

			return toreturn;
		}

		private string GetUserNameFromDoc(XmlDocument doc)
		{
            if (doc == null)
                throw new ArgumentNullException("doc");

			var x = doc.SelectSingleNode("/friendsList/steamID64");
			if (x == null)
				return null;

			return x.InnerText.Trim();
		}

		private static IEnumerable<string> GetFriendIdsFromPage(XmlNode parent)
		{
			foreach (XmlElement friend in parent.SelectNodes("friend"))
			{
                yield return friend.InnerText;
			}
		}

        public User GetUser(Uri profileUrl)
		{
			var doc = DownloadContent(profileUrl, userPage);

			var username = doc.SelectSingleNode("/profile/steamID").InnerText.Trim();
			var imageUrl = doc.SelectSingleNode("/profile/avatarFull").InnerText.Trim();
            var id = doc.SelectSingleNode("/profile/steamID64").InnerText.Trim();

			return new User
			{
				Id = id,
				Username = username,
				IconUrl = new Uri(imageUrl),
                CommunityUrl = profileUrl,
			};
		}

		public GameCollection GetGames(Uri profileUrl)
		{
			var doc = DownloadContent(profileUrl, gamesPage);
            if (doc == null)
                return null;

            var gamesElements = doc.SelectNodes("/gamesList/games/game");

            var games = from g in gamesElements.Cast<XmlElement>()
                        let appId = g["appID"]
                        let logo = g["logo"]
                        let name = g["name"]
                        let link = g["storeLink"]
                        select new Game
                        {
                            Id = appId.InnerText,
                            IconUrl = logo.InnerText,
                            Name = name.InnerText,
                            SteamUrl = link.InnerText,
                        };

            return new GameCollection(games)
            {
                SteamId = doc.SelectSingleNode("/gamesList/steamID64").InnerText,
                Username = doc.SelectSingleNode("/gamesList/steamID").InnerText,
            };
		}

		private XmlDocument DownloadContent(Uri steamCommunityUrl, string page)
		{
			var url = new Uri(steamCommunityUrl, page);

			var doc = this._cache.GetContent(url, false);
            if (doc == null)
                return null;

            if (doc.OuterXml.Contains("The specified profile could not be found."))
                return null;

            return doc;
		}
	}
}
