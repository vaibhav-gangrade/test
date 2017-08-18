using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Data.Entity;
using System.Web.Helpers;
using System.Security.Claims;
using Millionlights.Models;

namespace Millionlights
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //PrashantSir oAuth Commented
        //protected void Application_Start()
        //{
        //    Database.SetInitializer<Models.ApplicationDbContext>(null);
        //    //Database.SetInitializer<MillionLightContext>(null);
        //    //Database.SetInitializer<MillionLightContext>(new DropCreateDatabaseIfModelChanges<MillionLightContext>());
        //    AreaRegistration.RegisterAllAreas();
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);
        //}
        protected void Application_Start()
        {
            Database.SetInitializer<Models.ApplicationDbContext>(null);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
        protected void Application_BeginRequest()
        {
            //if (!Context.Request.IsSecureConnection)
            //    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));

            //if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
            //{
            //    UriBuilder builder = new UriBuilder(Request.Url);
            //    builder.Host = "www." + Request.Url.Host;
            //    Response.StatusCode = 301;
            //    Response.AddHeader("Location", builder.ToString());
            //    Response.End();
            //}
        }
    }
}
