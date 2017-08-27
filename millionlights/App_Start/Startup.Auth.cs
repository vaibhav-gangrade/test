using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Owin.Security.Providers.LinkedIn;
using System.Security.Claims;
using System;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Millionlights.Common;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Diagnostics;
using Millionlights.Models;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;

namespace Millionlights
{
    [RequireHttps]
    public partial class Startup
    {
        MillionlightsContext mldb = new MillionlightsContext();
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType("External");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "External",
                AuthenticationMode = AuthenticationMode.Active,
                LoginPath = new PathString(Paths.LoginPath),
                LogoutPath = new PathString(Paths.LogoutPath),
                CookieName = "MillionlightsSSO",
                //CookieName = "MillionlightstestSSO",
                CookieDomain = Paths.Multidomain,
                ExpireTimeSpan = TimeSpan.FromDays(15)
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            TweeterAuthOptions(app);
            FacebookAuthOptions(app);
            GoogleAuthOptions(app);
            MicrosoftAuthOptions(app);
            LinkedInAuthOptions(app);

            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString(Paths.AuthorizePath),
                TokenEndpointPath = new PathString(Paths.TokenPath),
                ApplicationCanDisplayErrors = true,
                //#if DEBUG
                AllowInsecureHttp = true,
                //#endif
                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    OnGrantClientCredentials = GrantClientCredetails
                },

                // Authorization code provider which creates and receives authorization code
                AuthorizationCodeProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },

                // Refresh token provider which creates and receives referesh token
                RefreshTokenProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                }
            });
        }

        #region Oauth Server
        private Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == Clients.Client1.Id)
            {
                context.Validated(Clients.Client1.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client2.Id)
            {
                context.Validated(Clients.Client2.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client3.Id)
            {
                context.Validated(Clients.Client3.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client4.Id)
            {
                context.Validated(Clients.Client4.RedirectUrl);
            }
            else
            {
                Trace.TraceInformation("ValidateClientRedirectUri Method - RedirectUrl not validated");
            }

            Trace.TraceInformation("Inside ValidateClientRedirectUri Method- context.ClientId" + context.ClientId + "context.Error" + context.Error + "context.ErrorDescription" +
                context.ErrorDescription + "context.ErrorUri" + context.ErrorUri + "context.RedirectUri" + context.RedirectUri + "context.Response" + context.Response + "context.Request" + context.Request);
            return Task.FromResult(0);
        }

        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                if (clientId == Clients.Client1.Id && clientSecret == Clients.Client1.Secret)
                {
                    context.Validated();
                }
                else if (clientId == Clients.Client2.Id && clientSecret == Clients.Client2.Secret)
                {
                    context.Validated();
                }
                else if (clientId == Clients.Client3.Id && clientSecret == Clients.Client3.Secret)
                {
                    context.Validated();
                }
                else if (clientId == Clients.Client4.Id && clientSecret == Clients.Client4.Secret)
                {
                    context.Validated();
                }
                else
                {
                    Trace.TraceInformation("ValidateClientAuthentication Method - Context Rejected");
                    context.Rejected();
                }
            }
            return Task.FromResult(0);
        }

        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                //Trace.TraceInformation("inside GrantResourceOwnerCredentials");
                //Trace.TraceInformation("context.UserName -" + context.UserName);
                //Trace.TraceInformation("context.Password -" + context.Password);
                //KeyValuePair<string, string> clientDetails = new KeyValuePair<string, string>(Clients.Client4.Id, Clients.Client3.Secret);

                //StreamReader sReader = new StreamReader(context.Request.Body);
                
                //Trace.TraceInformation("Request.Body -" + sReader.ReadToEnd());
                //Trace.TraceInformation("Request.QueryString -" + context.Request.QueryString.Value);
                Millionlights.Services.UserService userService = new Millionlights.Services.UserService();
                var user = mldb.Users.SingleOrDefault(p => p.EmailId == context.UserName && p.IsActive == true);

                //user = userService.Authenticate(context.UserName, temppass);

                if (user != null )
                {
                    var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                    Trace.TraceInformation("User Found.");
                    oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
                    bool resOwnerGranted = context.Validated(ticket);
                    Trace.TraceInformation("resOwnerGranted " + resOwnerGranted);
                }
                else
                {
                    context.SetError("Incorrect Credentials");
                    context.Rejected();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("inside GrantResourceOwnerCredentials method- Exception message" + ex.Message);
                Trace.TraceError("inside GrantResourceOwnerCredentials method- Exception StackTrace" + ex.StackTrace);
            }
            return Task.FromResult(0);
        }

        private Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        {
            try
            {
                var identity = new ClaimsIdentity(new GenericIdentity(
                                                context.ClientId,
                                                OAuthDefaults.AuthenticationType),
                                                context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

                bool clGranted = context.Validated(identity);
                Trace.TraceInformation("Inside GrantClientCredetails method- clGranted=" + clGranted);
            }
            catch (Exception ex)
            {
                Trace.TraceError("inside GrantClientCredetails method- Exception message" + ex.Message);
                Trace.TraceError("inside GrantClientCredetails method- Exception StackTrace" + ex.StackTrace);
            }
            return Task.FromResult(0);
        }


        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            //PD Changes
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(2);
            //
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            Trace.TraceInformation("Inside CreateAuthenticationCode method- _authenticationCodes.Count=" + _authenticationCodes.Count);
            _authenticationCodes[context.Token] = context.SerializeTicket();
            Trace.TraceInformation("Inside CreateAuthenticationCode Method- context.OwinContext" + context.OwinContext + "context.Ticket" + context.Ticket + "context.Token" +
                context.Token + "context.Response" + context.Response + "context.Request" + context.Request);
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
            Trace.TraceInformation("Inside ReceiveAuthenticationCode Method- context.OwinContext" + context.OwinContext + "context.Ticket" + context.Ticket + "context.Token" +
                context.Token + "context.Response" + context.Response + "context.Request" + context.Request);
        }

        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            //PD Changes
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(2);
            //
            context.SetToken(context.SerializeTicket());
            Trace.TraceInformation("Inside CreateRefreshToken Method- context.OwinContext" + context.OwinContext + "context.Ticket" + context.Ticket + "context.Token" +
                context.Token + "context.Response" + context.Response + "context.Request" + context.Request);
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
        #endregion

        #region External Authentications
        private static void FacebookAuthOptions(IAppBuilder app)
        {
            var x = new FacebookAuthenticationOptions();
            x.Scope.Add("email");
            x.AppId = "1704814389826228";
            x.AppSecret = "c74fc3bb490c5334bb9ee89fc73184fd";
            x.BackchannelHttpHandler = new HttpClientHandler();
            x.UserInformationEndpoint = "https://graph.facebook.com/v2.8/me?fields=id,name,email,first_name,last_name";
            // Facebook Deveoper account: Archana
            //x.AppId = "389441631447093";
            //x.AppSecret = "d176c5e04afebdf4e02886d9003bb839";

            x.Provider = new FacebookAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:facebook:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
                    }
                }
            };
            x.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(x);
        }

        private static void GoogleAuthOptions(IAppBuilder app)
        {
            var y = new GoogleOAuth2AuthenticationOptions();
            y.Scope.Add("email");
            y.ClientId = "220411177158-tnjrdamkaq64n44772pn53pfamf9anja.apps.googleusercontent.com";
            y.ClientSecret = "k0LHEZYuKhjpKafHHGqNIeFO";
            y.Provider = new GoogleOAuth2AuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new System.Security.Claims.Claim("urn:tokens:googleplus:accesstoken", context.AccessToken));
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:Google:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();

                        if (!context.Identity.HasClaim(claimType, claimValue))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Google"));
                        }
                    }
                }
            };
            y.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseGoogleAuthentication(y);
        }

        private static void MicrosoftAuthOptions(IAppBuilder app)
        {
            var m = new MicrosoftAccountAuthenticationOptions();
            m.Scope.Add("email");
            m.ClientId = "000000004C14E9A8";
            m.ClientSecret = "Mvhzv6fj0IxyC94FRvu14N8X199ZbZEe";
            m.Provider = new MicrosoftAccountAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new System.Security.Claims.Claim("urn:microsoftaccount:access_token", context.AccessToken));
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:microsoftaccount:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Microsoft"));

                    }

                }
            };
            m.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseMicrosoftAccountAuthentication(m);
        }

        private static void LinkedInAuthOptions(IAppBuilder app)
        {
            var l = new LinkedInAuthenticationOptions();
            l.ClientId = "785xb6c6vwm3ww";
            l.ClientSecret = "1Yn6sh46wxwtHYyp";
            l.Provider = new LinkedInAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new System.Security.Claims.Claim("urn:linkedin:access_token", context.AccessToken));
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:linkedin:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Linkedin"));
                    }
                }
            };
            l.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseLinkedInAuthentication(l);
        }

        private static void TweeterAuthOptions(IAppBuilder app)
        {
            var t = new TwitterAuthenticationOptions();
            t.ConsumerKey = "oodqrcpoRHLkQYCABKCqa8IIN";
            t.ConsumerSecret = "XBA1sKYm5dJYTjDGulLmqVqKIwe6lvINPsXhr7GlOgAXVLILPB";
            t.Provider = new TwitterAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new Claim("urn:tokens:twitter:accesstoken", context.AccessToken));
                    context.Identity.AddClaim(new Claim("urn:tokens:twitter:accesstokensecret", context.AccessTokenSecret));
                    foreach (var claim in context.UserId)
                    {
                        var claimType = string.Format("urn:twitter:{0}", claim);
                        string claimValue = claim.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Twitter"));
                    }
                }
            };
            t.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseTwitterAuthentication(t);
        }

        #endregion
    }
}