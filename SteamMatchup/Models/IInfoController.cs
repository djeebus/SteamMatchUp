using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SteamMatchUp.Website.Models
{
    public interface IInfoController
    {
        searchPlayersResponse Search(string term);
        getGamersResponse GetGamers(string[] gamerIds);
        getGamesResponse GetGames(string[] gameIds);
    }

    public class response
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
    }

    public class searchPlayersResponse : response
    {
        public result[] results { get; set; }
    }

    public class getGamersResponse : response
    {
        public gamer[] results { get; set; }
    }

    public class getGamesResponse : response
    {
        public game[] results { get; set; }
    }

    public class gamer
    {
        public string id { get; set; }
        public string name { get; set; }

        public string[] gameIds { get; set; }
        public friend[] friends { get; set; }
    }

    public class friend
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class game
    {
        public string id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
        public string[] features { get; set; }
        public string[] genres { get; set; }
    }

    public class result
    {
        public string id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
    }
}