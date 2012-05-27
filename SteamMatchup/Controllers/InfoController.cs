using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SteamMatchUp.Website.Models;

namespace SteamMatchUp.Website.Controllers
{
    public class InfoController : ApiController, IInfoController
    {
        private readonly ISteamGameParser _gameParser;
        private readonly ISteamProfileParser _profileParser;
        private readonly ISteamProfileSearcher _profileSearcher;

        public InfoController(
            ISteamGameParser gameParser,
            ISteamProfileParser profileParser,
            ISteamProfileSearcher profileSearcher)
        {
            this._gameParser = gameParser;
            this._profileParser = profileParser;
            this._profileSearcher = profileSearcher;
        }

        [ActionName("players"), HttpGet]
        public searchPlayersResponse Search(string term)
        {
            var result = this._profileSearcher.Search(term);

            return new searchPlayersResponse
            {
                success = true,
                errorMessage = null,
                results = (from r in result
                           select new result
                           {
                               id = r.Id,
                               iconUrl = r.IconUrl.ToString(),
                               name = r.Username,
                           }).ToArray(),
            };
        }

        [ActionName("gamers"), HttpGet]
        public getGamersResponse GetGamers(string[] gamerIds)
        {
            return new getGamersResponse
            {
                success = true,
                results = (from id in gamerIds
                          let profile = _profileParser.GetGames(id)
                          let friends = _profileParser.GetFriends(id)
                          select new gamer
                          {
                              id = id,
                              name = profile.Username,
                              gameIds = (from g in profile
                                         select g.Id).ToArray(),
                              friends = (from f in friends
                                         select new friend
                                         {
                                             id = f.Id,
                                             name = f.Username,
                                         }).ToArray(),
                          }).ToArray(),
            };
        }

        [ActionName("games"), HttpGet]
        public getGamesResponse GetGames(string[] gameIds)
        {
            return new getGamesResponse
            {
                success = true,
                results = (from id in gameIds.AsParallel()
                         let g = this._gameParser.GetInfo(id)
                         where g != null
                         select new game
                         {
                             id = id,
                             iconUrl = g.IconUrl.ToString(),
                             name = g.Name,
                             features = g.Features,
                             genres = g.Genres,
                         }).ToArray(),
            };
        }
    }
}
