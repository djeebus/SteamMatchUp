using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.Messaging;
using System.Web.Security;
using System.Diagnostics;
using SteamMatchUp.SteamApi;
using System.Configuration;

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

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            return this.Redirect("~/");
        }

        [HttpPost]
        public ActionResult BeginAuth()
        {
            var provider = "http://steamcommunity.com/openid";
            var realm = new DotNetOpenAuth.OpenId.Realm(string.Format("{0}{1}{2}", this.Request.Url.Scheme, Uri.SchemeDelimiter, this.Request.Url.Authority));
            var returnTo = new Uri(this.Request.Url, this.Url.Action("EndAuth"));

            using (var rp = new OpenIdRelyingParty())
            {
                var request = rp.CreateRequest(provider, realm, returnTo);

                var claimsRequest = new ClaimsRequest
                {
                    Email = DemandLevel.Require,
                    BirthDate = DemandLevel.Request,
                    Country = DemandLevel.Request,
                    FullName = DemandLevel.Request,
                    Gender = DemandLevel.Request,
                    Language = DemandLevel.Request,
                    Nickname = DemandLevel.Request,
                    PostalCode = DemandLevel.Request,
                    TimeZone = DemandLevel.Request,
                };

                request.AddExtension(claimsRequest);

                return request.RedirectingResponse.AsActionResult();
            }
        }

        public ActionResult EndAuth()
        {
            using (var rp = new OpenIdRelyingParty())
            {
                var result = rp.GetResponse();
                if (result == null)
                    return this.RedirectToAction("Index");

                switch (result.Status)
                {
                    case AuthenticationStatus.Authenticated:

                        var uri = new Uri(result.ClaimedIdentifier.ToString());
                        var id = uri.Segments[3];

                        FormsAuthentication.RedirectFromLoginPage(id, false);
                        return this.RedirectToAction("Index");

                    default:
                        Trace.WriteLine("Result: " + result.Status);
                        return this.RedirectToAction("Index");
                }
            }
        }
    }
}
