using DatabaseWebService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DatabaseWebService
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        /*private IEnumerable<IDisposable> GetHangfireServers()
        {
            string connString = ConfigurationManager.ConnectionStrings["GrafolitOTPEntities"].ConnectionString;

            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("data source=10.10.10.10;user id=martinp;password=m123.;integrated security=False;initial catalog=GrafolitOTP_Prod;", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                });

            yield return new BackgroundJobServer();
        }
        */
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //HangfireAspNet.Use(GetHangfireServers);
        }

        void Application_Error(object sender, EventArgs e)
        {

            string error = "";
            if (Context != null && Server.GetLastError() != null)
                DataTypesHelper.getError(Context.Error, ref error);

            if (HttpContext.Current.Error != null)
                DataTypesHelper.getError(HttpContext.Current.Error, ref error);

            //if is there error on client side we need aditional information about error

            error += "\r\n \r\n" + sender.GetType().FullName + "\r\n" + HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "\r\n";

            DataTypesHelper.LogThis(error);

            if (Context != null)
                Context.ClearError();


            Server.ClearError();
        }
    }
}
