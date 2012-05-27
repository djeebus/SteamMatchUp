using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace SteamMatchUp
{
	public class WebpageCache : IWebpageCache
	{
		static readonly Dictionary<string, object> lockerObjects = new Dictionary<string, object>();

		private readonly string _rootDir;
		private readonly IWebpageCleaner _cleaner;
        private readonly IHttpClient _httpClient;

		public WebpageCache(string rootDir, IWebpageCleaner cleaner, IHttpClient httpClient)
		{
            if (cleaner == null)
                throw new ArgumentNullException("cleaner");

			if (string.IsNullOrWhiteSpace(rootDir))
				throw new ArgumentNullException("rootDir");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

			this._cleaner = cleaner;
            this._httpClient = httpClient;

			this._rootDir = EnsureRootPath(rootDir);
		}

		private string EnsureRootPath(string rootDir)
		{
			if (Path.IsPathRooted(rootDir))
				return rootDir;

			if (rootDir[0] == '~')
			{
				var context = HttpContext.Current;
				if (context == null)
					throw new ArgumentOutOfRangeException("rootDir", rootDir, "Cannot use application-relative path in a non web application");

				return context.Server.MapPath(rootDir);
			}

			return Path.GetFullPath(rootDir);
		}

		public XmlDocument GetContent(Uri url)
		{
            if (url == null)
                throw new ArgumentNullException("url");

			var doc = new XmlDocument();

			var filename = GetFolder(url);

			// cheap search
			if (File.Exists(filename))
			{
				doc.Load(filename);
				return doc;
			}

			object fileLocker = GetFileLocker(filename);

			lock (fileLocker)
			{
				// locked search
				if (File.Exists(filename))
				{
					doc.Load(filename);
					return doc;
				}

                var dt = DateTime.Now;

                var content = this._httpClient.GetContent(url, new Dictionary<HttpRequestHeader, string> {
                    { HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8" },
                    { HttpRequestHeader.Cookie, "Steam_Language=english; community_game_list_scroll_size=10; timezoneOffset=-14400,0; birthtime=157795201; lastagecheckage=1-January-1975;" },
                    { HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.162 Safari/535.19" },
                });

				var duration = DateTime.Now - dt;

				Trace.WriteLine(string.Format("Downloaded {0} in {1} ms", url, duration.TotalMilliseconds), "WebpageCache");

				doc = this._cleaner.GetDocFromContent(content);

				this.Save(url, doc);

				return doc;
			}
		}

		private object GetFileLocker(string filename)
		{
			lock (lockerObjects)
			{
				if (lockerObjects.ContainsKey(filename))
					return lockerObjects[filename];

				var locker = new object();
				lockerObjects.Add(filename, locker);

				return locker;
			}
		}

		void Save(Uri url, XmlDocument doc)
		{
			var filename = GetFolder(url);

			EnsureFolder(Path.GetDirectoryName(filename));

			if (File.Exists(filename))
				File.Delete(filename);

			File.WriteAllText(filename, doc.OuterXml);
		}

		void EnsureFolder(string folder)
		{
			if (Directory.Exists(folder))
				return;

			string parent = Path.GetDirectoryName(folder);
			EnsureFolder(parent);

			Directory.CreateDirectory(folder);
		}

		string GetFolder(Uri url)
		{
			string filename = Path.Combine(
				this._rootDir,
				string.Format(@"{0}\{1}",
					url.Host,
					url.PathAndQuery
						.Replace('/', '\\')
						.Replace('?', '_')
						.Replace('&', '-'))).ToLower();

			if (filename.EndsWith(@"\"))
				filename += "index";

			filename += ".xhtml";

			return filename;
		}
	}
}
