using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Millionlights.Models;
using Millionlights.Services;
using Millionlights.ActionFilters;
using Millionlights.Filters;

namespace Millionlights.Controllers
{
    //PrashantSir oAuth Commented
    //[Route("api/[controller]")]
    //public class AuthenticateController : ApiController
    //{
    //    private MillionlightsContext db = new MillionlightsContext();
    //    private readonly IUserService _userServices;
    //    private int? userId = null;


    //    /// <summary>
    //    /// Public constructor to initialize product service instance
    //    /// </summary>
    //    public AuthenticateController(IUserService userService)
    //    {
    //        _userServices = userService;
    //    }

    //    /// <summary>
    //    /// Authenticates user and returns token with expiry.
    //    /// </summary>
    //    /// <returns></returns>
    //    [ApiAuthenticationFilter(true)]
    //    [HttpPost]
    //    public HttpResponseMessage Authenticate()
    //    {
    //        if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
    //        {
    //            var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
    //            if (basicAuthenticationIdentity != null)
    //            {
    //                userId = basicAuthenticationIdentity.UserId;
    //                return GetAuthToken(userId.Value);
    //            }
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// Returns auth token for the validated user.
    //    /// </summary>
    //    /// <param name="userId"></param>
    //    /// <returns></returns>
    //    private HttpResponseMessage GetAuthToken(int userId)
    //    {
    //        var token = _userServices.GenerateAuthToken(userId);
    //        var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
    //        response.Headers.Add("Token", token.Token);
    //        response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
    //        response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
    //        return response;
    //    }

    //    [AuthorizationRequired]
    //    // GET api/product
    //    [HttpGet]
    //    public HttpResponseMessage GetCourses()
    //    {
    //        var products = _userServices.GetUsersCourses(userId.Value);

    //        if (products.Any())
    //        {
    //            return Request.CreateResponse(HttpStatusCode.OK, products);
    //        }

    //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Products not found");
    //    }

    //}
}