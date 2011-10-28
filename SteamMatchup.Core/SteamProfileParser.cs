﻿using System;
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

		public FriendCollection GetFriends(string steamCommunityId)
		{
			var content = DownloadContent(steamCommunityId, friendsPage);

			var doc = CleanseHtml(content);

			var friendsContainer = doc.SelectSingleNode("//div[@id='memberList']");

			var toreturn = new FriendCollection(GetFriendsFromPage(friendsContainer).ToArray())
			{
				Username = steamCommunityId,
			};

			return toreturn;
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
            var content = DownloadContent(steamCommunityId, string.Empty, out actualPage);

            var doc = CleanseHtml(content);

            var username = doc.SelectSingleNode("//h1").InnerText.Trim();
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
			var content = DownloadContent(steamCommunityId, gamesPage);

			var doc = CleanseHtml(content);

			var gamesContainer = doc.SelectSingleNode("//div[@class='games_list_tab_content']");

			var toreturn = new GameCollection(GetGamesFromContainer(gamesContainer).ToList())
			{
				Username = steamCommunityId,
			};

			return toreturn;
		}

		private static IEnumerable<Game> GetGamesFromContainer(XmlNode parent)
		{
			foreach (XmlElement game in parent.SelectNodes("div[@class='gameListRow']"))
			{
				var logo = game.SelectSingleNode(".//div[@class='gameLogo']/a");
				var url = logo.Attributes["href"].Value;
				var name = game.SelectSingleNode(".//h4").InnerText;
				var img = logo.SelectSingleNode("img").Attributes["src"].Value;

				yield return new Game
				{
					IconUrl = img,
					Name = name,
					SteamUrl = url
				};
			}
		}

		private static string GetProfileUrl(string steamCommunityId, string page)
		{
			string format = urlNumberFormat; // regex.IsMatch(steamCommunityId) ? urlNumberFormat : urlNameFormat;

			return string.Format(format, steamCommunityId, page);
		}

        private static string DownloadContent(string steamCommunityId, string page)
        {
            string url;
            return DownloadContent(steamCommunityId, page, out url);
        }
		private static string DownloadContent(string steamCommunityId, string page, out string actualPage)
		{
			WebClient wc = new WebClient();

			foreach (var format in new string[] { urlNameFormat, urlNumberFormat })
			{
				var url = string.Format(format, steamCommunityId, page);

				var content = wc.DownloadString(url);

                if (!content.Contains("The specified profile could not be found."))
                {
                    actualPage = url;
                    return content;
                }
			}

			throw new Exception("This person does not have a profile set up.");
		}

		private static XmlDocument CleanseHtml(string text)
		{
			var streamReader = new StringReader(text);
			
			var doc = new XmlDocument();

			Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
			sgmlReader.DocType = "HTML";
			sgmlReader.InputStream = streamReader;

			doc.XmlResolver = null;
			doc.Load(sgmlReader);

			return doc;
		}
    }
}
