using INSS.ODS.Common.Utilities.Logging;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Insolvency.CommonServices.WorldpayProxy
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //.NET Framework 4.7 onwards allows to set system default TLS version
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Logger.LogInfo("Service started");
        }
    }
}
