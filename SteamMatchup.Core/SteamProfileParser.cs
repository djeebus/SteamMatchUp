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

		public FriendCollection GetFriends(string steamCommunityId)
		{
			var url = GetProfileUrl(steamCommunityId, friendsPage);

			var content = DownloadContent(url);

			var doc = CleanseHtml(content);

			var friendsContainer = doc.SelectSingleNode("//div[@id='memberList']");

			var toreturn = new FriendCollection(GetFriendsFromPage(friendsContainer).ToArray())
			{
				Username = steamCommunityId,
				ProfileUrl = url,
			};

			return toreturn;
		}

		private static IEnumerable<Friend> GetFriendsFromPage(XmlNode parent)
		{
			foreach (XmlElement friend in parent.SelectNodes("div"))
			{
				var a = friend.SelectSingleNode("p/a");
				var name = a.InnerText;
				var link = new Uri(a.Attributes["href"].Value);
				var id = link.Segments[2];
				var img = friend.SelectSingleNode(".//img").Attributes["src"].Value;

				yield return new Friend
				{
					CommunityId = id,
					CommunityUrl = link.ToString(),
					IconUrl = img,
					Name = name,
				};
			}
		}

		public GameCollection GetGames(string steamCommunityId)
		{
			var url = GetProfileUrl(steamCommunityId, gamesPage);

			var content = DownloadContent(url);

			var doc = CleanseHtml(content);

			var gamesContainer = doc.SelectSingleNode("//div[@class='games_list_tab_content']");

			var toreturn = new GameCollection(GetGamesFromContainer(gamesContainer).ToList())
			{
				Username = steamCommunityId,
				ProfileUrl = url,
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
			string format = regex.IsMatch(steamCommunityId) ? urlNumberFormat : urlNameFormat;

			return string.Format(format, steamCommunityId, page);
		}

		private static string DownloadContent(string url)
		{
			WebClient wc = new WebClient();

			var content = wc.DownloadString(url);

			if (content.Contains("The specified profile could not be found."))
				throw new Exception("This person does not have a profile set up.");

			return content;
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
