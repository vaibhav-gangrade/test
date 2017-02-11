//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;

//namespace Millionlights.App_Start
//{
//    public class WebApiConfig
//    {
//        public static void Register(HttpConfiguration config)
//        {
//            //config.MapHttpAttributeRoutes();

//            //config.Routes.MapHttpRoute(
//            //    name: "DefaultApi",
//            //    routeTemplate: "api/{controller}/{id}",
//            //    defaults: new { id = RouteParameter.Optional }
//            //);
//        }
//    }
//}
//PrashantSir oAuth
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Millionlights
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
             );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
        }
    }
}
