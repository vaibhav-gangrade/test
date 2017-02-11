using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Millionlights
{
    public class RouteConfig 
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "ExternalLoginConfirmation",
               url: "{Account}/{ExternalLoginConfirmation}",
               defaults: new { controller = "Account", action = "ExternalLoginConfirmation", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "logOff",
              url: "{UserRegister}/{LoginOut}",
              defaults: new { controller = "UserRegister", action = "LoginOut" }
          );

            //AboutCourse
            routes.MapRoute(
                name: "AboutCourse",
                url: "{Course}/{AboutCourse}/{id}",
                defaults: new { controller = "Course", action = "AboutCourse", id = UrlParameter.Optional}
            );
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
