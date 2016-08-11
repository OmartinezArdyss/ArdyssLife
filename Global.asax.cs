using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using ArdyssLife;
using System.Configuration;
using System.Web.UI;
using Data.Db.Shopify;
using Data.Clases;
using ArdyssLife.PaginaBase;
using System.Threading;

namespace ArdyssLife
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SESSION.TIMEOUT"]);
        }

        void Session_End(object sender, EventArgs e)
        {

        }
    }
}
