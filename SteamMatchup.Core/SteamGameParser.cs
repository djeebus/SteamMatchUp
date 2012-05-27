using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Diagnostics;

namespace SteamMatchUp
{
	public class SteamGameParser : ISteamGameParser
	{
		static readonly DateTime birthdate = new DateTime(1975, 1, 1);

		static readonly string[] FakeFeatures = new[] {
			"Captions available",
			"Commentary available",
			"HDR available",
			"Includes Source SDK",
			"Includes level editor",
			"Valve Anti-Cheat enabled",
			"Mods",
			"Mods (require HL2)",
		};

		const string AppUrlFormat = "http://store.steampowered.com/app/{0}";

		private readonly IWebpageCache _cache;

		public SteamGameParser(IWebpageCache cache)
		{
			this._cache = cache;
		}

		public GameInfo GetInfo(string appId)
		{
			DateTime dt = DateTime.Now;
			var url = string.Format(AppUrlFormat, appId);

			var doc = _cache.GetContent(new Uri(url));
            if (doc == null)
                return null;

			var gameInfo = GetGameInfoFromContent(doc);

			var duration = DateTime.Now - dt;
			Trace.WriteLine(string.Format("Info retrieved and parsed in {0} ms", duration.TotalMilliseconds), "SteamGameParser");

			return gameInfo;
		}

		private GameInfo GetGameInfoFromContent(XmlDocument doc)
		{
			string content = doc.OuterXml;

			if (content.Contains("agecheck"))
				return null; // age verification page, need to clean this one up later

			if (content.Contains("cluster_scroll_area"))
				return null; // home page, need to clean this one up later

			var mgr = new XmlNamespaceManager(doc.NameTable);

			var info = new GameInfo();

			info.Name = doc.SelectSingleNode("//div[@class='game_name']").InnerText.Trim();

			var specs = doc.SelectNodes("//div[@class='details_block']//div[@class='game_area_details_specs']");
			info.Features = (from s in specs.Cast<XmlElement>()
							 let feature = (s.InnerText ?? string.Empty).Trim()
							 where !FakeFeatures.Any(ff => ff == s.InnerText)
							 orderby feature
							 select s.InnerText).ToArray();

			var blockTitles = doc.SelectNodes("//div[@class='details_block']//b");
			var genreTitle = (from b in blockTitles.Cast<XmlElement>()
							  let genre = b.InnerText.Trim()
							  where genre.StartsWith("Genre")
							  select b).FirstOrDefault();

			var image = doc.SelectSingleNode("//div[@class='game_header_image_ctn']/img");
			if (image == null)
				return null;

			info.IconUrl = new Uri(image.Attributes["src"].Value);

			if (genreTitle != null)
			{
				var genres = new List<string>();

				var temp = genreTitle.NextSibling;
				while (true)
				{
					var t = temp;
					temp = temp.NextSibling;

					if (t is XmlText)
						continue;

					if (t.LocalName != "a")
						break;

					genres.Add(t.InnerText);
				}

				info.Genres = genres.OrderBy(g => g).ToArray();
			}

			return info;
		}
	}
}
