[assembly: WebActivator.PreApplicationStartMethod(typeof(SteamMatchUp.Website.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(SteamMatchUp.Website.App_Start.NinjectWebCommon), "Stop")]

namespace SteamMatchUp.Website.App_Start
{
	using System;
	using System.Web;

	using Microsoft.Web.Infrastructure.DynamicModuleHelper;

	using Ninject;
	using Ninject.Web.Common;
	using Ninject.Syntax;
	using System.Configuration;

	static class NinjectExtensions
	{
		public static IBindingWithOrOnSyntax<T> WithContructorArgumentFromAppSetting<T>(this IBindingWithSyntax<T> syntax, string name, string appSettingKey)
		{
			var value = ConfigurationManager.AppSettings[appSettingKey];

			return syntax.WithConstructorArgument(name, value);
		}
	}

	public static class NinjectWebCommon
	{
		private static void RegisterServices(IKernel kernel)
		{
            kernel.Bind<IHttpClient>().To<HttpClient>().InRequestScope();
			kernel.Bind<ISteamProfileParser>().To<SteamProfileParser>().InRequestScope();
			kernel.Bind<IWebpageCleaner>().To<WebpageCleaner>().InRequestScope();
			kernel.Bind<IWebpageCache>().To<WebpageCache>().InRequestScope().WithContructorArgumentFromAppSetting("rootDir", "Root Cache Dir");
			kernel.Bind<ISteamGameParser>().To<SteamGameParser>().InRequestScope();
            kernel.Bind<SteamApi.ISteamApi>().To<SteamApi.SteamApiClient>().InRequestScope().WithPropertyValue("Key", ConfigurationManager.AppSettings["Steam Api Key"]);
		}

		private static readonly Bootstrapper bootstrapper = new Bootstrapper();

		/// <summary>
		/// Starts the application
		/// </summary>
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
			DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
			bootstrapper.Initialize(CreateKernel);
		}

		/// <summary>
		/// Stops the application.
		/// </summary>
		public static void Stop()
		{
			bootstrapper.ShutDown();
		}

		/// <summary>
		/// Creates the kernel that will manage your application.
		/// </summary>
		/// <returns>The created kernel.</returns>
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();

			// set variables
			kernel.Settings.AllowNullInjection = true;

			kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
			kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

			RegisterServices(kernel);

            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

			return kernel;
		}
	}
}
