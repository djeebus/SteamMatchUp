using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamMatchUp;

namespace SteamSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var cleaner = new WebpageCleaner();
            var client = new HttpClient();
            var cache = new WebpageCache(@"C:\temp\steam page cache", cleaner, client);

            var profileParser = new SteamProfileParser(cache);
            var gameParser = new SteamGameParser(cache);

            var games = profileParser.GetGames(new Uri("http://google.com/djeebus"));

            foreach (var g in games)
            {
                Console.WriteLine("Downloading {0} ... ", g.Name);

                gameParser.GetInfo(g.SteamUrl);
            }

            Console.WriteLine("Done!");

            Console.ReadLine();
        }
    }
}
