using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Optimization;

namespace SteamMatchUp.Website
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                "Default API", 
                "api/{controller}/{action}");

			routes.MapRoute(
				"Default", // Route name
				"{action}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
            RegisterBinders(ModelBinders.Binders);
            RegisterFactories(ValueProviderFactories.Factories);
            RegisterBundles(BundleTable.Bundles);
		}

        class NoTransform : IBundleTransform
        {
            public void Process(BundleResponse bundle)
            {
                bundle.ContentType = "text/javascript";
            }
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            var libBundle = new Bundle("~/scripts/lib", typeof(NoTransform)); 
            libBundle.AddFile("~/scripts/jquery-1.7.2.js");
			libBundle.AddFile("~/scripts/jquery-ui-1.8.20.js");
            libBundle.AddFile("~/scripts/jquery.tmpl.js");
            libBundle.AddFile("~/scripts/knockout-2.1.0.debug.js");
            libBundle.AddFile("~/scripts/knockout.simpleGrid.js");
            libBundle.AddFile("~/scripts/underscore.js");
            libBundle.AddFile("~/scripts/json2.js");
            libBundle.AddFile("~/scripts/storage.js");
            bundles.Add(libBundle);

            var localBundle = new Bundle("~/scripts/local", typeof(NoTransform));
            localBundle.AddDirectory("~/scripts/models/", "*.js", false);
            bundles.Add(localBundle);
        }

        private void RegisterFactories(ValueProviderFactoryCollection valueProviderFactories)
        {
            valueProviderFactories.Add(new JsonValueProviderFactory());
        }

        private void RegisterBinders(ModelBinderDictionary modelBinders)
        {
        }
	}
}