using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SteamMatchUp.Website.Controllers
{
	public class HomeController : Controller
	{
		public HomeController(ISteamProfileParser parser)
		{
		}

        public ActionResult Index()
        {
            return this.View();
        }
	}
}
