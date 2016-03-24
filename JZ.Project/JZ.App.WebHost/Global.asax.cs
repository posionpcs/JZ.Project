using System;
using System.Web;
using System.Web.Http;

namespace JZ.App.WebHost
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                   name: "DefaultApi",
                   routeTemplate: "api/{controller}/{id}",
                   defaults: new { id = RouteParameter.Optional });
        }
    }
}