using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Millionlights.Models;
using Millionlights.Common;
using System.Diagnostics;
using System;

namespace Millionlights.Controllers
{
    public class OAuth2Controller : Controller
    {
        MillionlightsContext db = new MillionlightsContext();

        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                Trace.TraceInformation("Inside Authorize Method");
                return RedirectToAction("AuthorizeError", "OAuth2");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            if (authentication.AuthenticationResponseChallenge==null) {
                Trace.TraceInformation("authentication.AuthenticationResponseChallenge is null");
            }
            else if (authentication.AuthenticationResponseGrant == null)
            {
                Trace.TraceInformation("authentication.AuthenticationResponseGrant is null");
            }
            else if (authentication.AuthenticationResponseRevoke == null)
            {
                Trace.TraceInformation("authentication.AuthenticationResponseRevoke is null");
            }
            var ticket = authentication.AuthenticateAsync("External").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                Trace.TraceInformation("identity is null");
                authentication.Challenge("External");
                Trace.TraceInformation("Request.Headers=" + Request.Headers + "Request.QueryString" + Request.QueryString);
                return new HttpUnauthorizedResult();
            }
            Trace.TraceInformation("Request.Headers=" + Request.Headers + "Request.QueryString" + Request.QueryString);
            var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');

            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
                {
                    try
                    {
                        identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                        foreach (var scope in scopes)
                        {
                            if (!string.IsNullOrEmpty(scope.Trim()))
                            {
                                identity.AddClaim(new Claim("urn:oauth:scope", scope));
                            }
                        }
                        authentication.SignIn(identity);
                        if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
                        {
                            authentication.SignOut("External");
                            authentication.Challenge("External");
                            return new HttpUnauthorizedResult();
                        }
                        Trace.TraceInformation("Authorize Post - Signed In");
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Authorize Post - Exception message" + ex.Message);
                        Trace.TraceError("Authorize Post - Exception StackTrace" + ex.StackTrace);
                        throw ex;
                    }
                }
                else
                {
                    Trace.TraceInformation("Authorize Post - submit.Grant is null");
                }
            }
            else if (Request.HttpMethod == "GET")
            {
                try
                {
                    Trace.TraceInformation("Authorize Get - Query " + Request.Url.Query);
                    if (Request.Url.Query.Contains(Clients.Client1.Id) || Request.Url.Query.Contains(Clients.Client2.Id) || Request.Url.Query.Contains(Clients.Client3.Id) || Request.Url.Query.Contains(Clients.Client4.Id))
                    {
                        identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                        Trace.TraceInformation("Authorize Post - identity.IsAuthenticated= " + identity.IsAuthenticated);
                        foreach (var scope in scopes)
                        {
                            if (!string.IsNullOrEmpty(scope.Trim()))
                            {
                                identity.AddClaim(new Claim("urn:oauth:scope", scope));
                            }
                        }
                        authentication.SignIn(identity);
                        Trace.TraceInformation("Authorize Get - Signed In");
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Authorize Get -Exception message" + ex.Message);
                    Trace.TraceError("Authorize Get -Exception StackTrace" + ex.StackTrace);
                    throw ex;
                }
            }

            return View();
        }
        public ActionResult AuthorizeError()
        {
            return View();
        }
    }
}