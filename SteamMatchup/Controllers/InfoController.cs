using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SteamMatchUp.Website.Models;
using System.Web.Caching;
using System.Diagnostics;

namespace SteamMatchUp.Website.Controllers
{
    public class InfoController : ApiController, IInfoController
    {
        private readonly ISteamGameParser _gameParser;
        private readonly ISteamProfileParser _profileParser;
        private readonly SteamApi.ISteamApi _steamApi;

        private readonly Cache _cache;

        public InfoController(
            ISteamGameParser gameParser,
            ISteamProfileParser profileParser,
            SteamApi.ISteamApi steamApi)
        {
            this._gameParser = gameParser;
            this._profileParser = profileParser;
            this._steamApi = steamApi;

            this._cache = System.Web.HttpContext.Current.Cache;
        }

        [ActionName("gamers"), HttpGet]
        public getGamersResponse GetGamers([FromUri] string[] gamerIds)
        {
            if (gamerIds == null || gamerIds.Length == 0)
                throw new ArgumentNullException("gamerIds");

            var users = _steamApi.GetPlayerSummaries(gamerIds);

            return new getGamersResponse
            {
                success = true,
                results = (from user in users                           
                           let profile = _profileParser.GetGames(user.ProfileUrl)
                           let friends = _profileParser.GetFriends(user.ProfileUrl)
                           select new gamer
                           {
                               id = user.SteamId,
                               name = profile.Username,
                               gameIds = (from g in profile
                                          select g.Id).ToArray(),
                               friends = (from f in friends
                                          orderby f.Username
                                          select new friend
                                          {
                                              id = f.Id,
                                              name = f.Username,
                                          }).ToArray(),
                           }).ToArray(),
            };
        }

        [ActionName("games"), HttpGet]
        public getGamesResponse GetGames([FromUri] string[] gameIds)
        {
            DateTime start = DateTime.Now;

            var model = new getGamesResponse
            {
                success = true,
                results = (from id in gameIds.AsParallel()
                           let g = retrieveGame(id) ?? downloadGame(id)
                           select g).ToArray(),
            };

            DateTime finishe = DateTime.Now;

            Trace.WriteLine(string.Format("Retrieving {0} games took {1} milliseconds", gameIds.Length, (finishe - start).TotalMilliseconds));

            return model;
        }

        private game downloadGame(string id)
        {
            var g = this._gameParser.GetInfo(id);

            var model = ConvertGame(id, g);

            storeGame(model);

            return model;
        }

        private void storeGame(game game)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            _cache.Add(game.id, game, null, Cache.NoAbsoluteExpiration, new TimeSpan(24, 0, 0), CacheItemPriority.Normal, null);
        }

        private game retrieveGame(string id)
        {
            return _cache[id] as game;
        }

        private game ConvertGame(string id, GameInfo g)
        {
            if (g == null)
                return new game { id = id, isValid = false };

            return new game
            {
                id = id,
                isValid = true,
                iconUrl = g.IconUrl.ToString(),
                name = g.Name,
                features = g.Features,
                genres = g.Genres,
            };
        }
    }
}
