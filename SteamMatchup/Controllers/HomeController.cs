using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SteamMatchUp.Website.Controllers
{
	public class HomeController : Controller
	{
		ISteamProfileParser parser;

		public HomeController(ISteamProfileParser parser)
		{
			this.parser = parser;
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[OutputCache(VaryByParam="username", Duration=60*60)]
		public ActionResult GetGames(string username)
		{
			try
			{
				var games = parser.GetGames(username);

				return this.Json(games);
			}
			catch (Exception ex)
			{
				this.Response.StatusCode = 500;
				return this.Content(ex.Message);
			}
		}

		[HttpPost]
		[OutputCache(VaryByParam = "username", Duration = 60 * 60)]
		public ActionResult GetFriends(string username)
		{
			try
			{
				var friends = parser.GetFriends(username);

				return this.Json(friends);
			}
			catch (Exception ex)
			{
				this.Response.StatusCode = 500;
				return this.Content(ex.Message);
			}
		}
	}
}
