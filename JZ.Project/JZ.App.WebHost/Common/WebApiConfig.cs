using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace JZ.App.WebHost.Common
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // JSON 序列化时间
            var jsonFormatter = new JsonMediaTypeFormatter();
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
            jsonFormatter.SerializerSettings.Converters.Add(
                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }
                );

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Process", action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}