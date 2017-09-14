using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Millionlights.Models;
using System.Net;
using System.Configuration;
using Millionlights.EmailService;
using System.IO;
using Millionlights.Common;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace Millionlights.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private MillionlightsContext mldb = new MillionlightsContext();
        UserRegisterController userRegisterController = new UserRegisterController();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }


        public ActionResult ForgotPassword()
        {

            //  return View();

            return RedirectToAction("~/Views/Account/ForgotPassword");
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (TempData["AccountDeactivateFlag"] != null)
            {
                ViewBag.AccountDeactivateFlag = "true";
            }

            if (TempData["ExtAlreadyExist"] != null)
            {
                ViewBag.ExtProviderName = TempData["ExtProviderName"];
                ViewBag.ExtAlreadyExist = "true";
            }
            else
            {
                TempData["ExtAlreadyExist"] = null;
                ViewBag.ExtAlreadyExist = "false";
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            //PrashantSir oAuth
            OAuthLogin();
            //
            return View();
        }
        //PrashantSir oAuth
        private void OAuthLogin()
        {
            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Signin")))
                {
                    var email = Request.Form["username"];
                    var pass = Request.Form["password"];
                    //var userExist = mldb.Users.Where(x => x.EmailId == email && x.Password == pass && x.IsActive).FirstOrDefault();
                    var userExist = mldb.Users.Where(x => x.EmailId == email && x.IsActive).FirstOrDefault();
                    if (userExist != null)
                    {
                        if (userRegisterController.passwordDecrypt(userExist.Password) == pass)
                        {
                            var authentication = HttpContext.GetOwinContext().Authentication;
                            authentication.SignOut();
                            authentication.SignIn(
                                                     new AuthenticationProperties
                                                     {
                                                         AllowRefresh = true,
                                                         IsPersistent = true,
                                                         ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                                     },
                                                    new ClaimsIdentity(new[] { new Claim(
                                                        ClaimsIdentity.DefaultNameClaimType, email) },
                                                        "External"));
                        }
                    }
                }
            }
        }

        [AllowAnonymous]
        public ActionResult OLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            OAuthLogin();

            return View();
        }

        public ActionResult External()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (Request.HttpMethod == "POST")
            {
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.Contains("External") && !string.IsNullOrEmpty(Request.Form.Get(key)))
                    {
                        var authType = key.Substring("External".Length);
                        authentication.Challenge(authType);
                        return new HttpUnauthorizedResult();
                    }
                }
            }
            var identity = authentication.AuthenticateAsync("External").Result.Identity;
            if (identity != null)
            {
                authentication.SignOut();
                authentication.SignIn(
                                        new AuthenticationProperties
                                        {
                                            AllowRefresh = true,
                                            IsPersistent = true,
                                            ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                        },
                                        new ClaimsIdentity(identity.Claims, "External", identity.NameClaimType, identity.RoleClaimType));
                return Redirect(Request.QueryString["ReturnUrl"]);
            }

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    SignIn(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);

                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                User userEntity = new User();
                userEntity.UserName = Request["UserName"];
                userEntity.Password = Request["Password"];
                mldb.Users.Add(userEntity);

                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    SignIn(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);

                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();

        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {

            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            Session["CouponCodeExternal"] = Request.Form["couponCode"];
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            try
            {

                var coupCode = Session["CouponCodeExternal"];
                string email = null;
                string firstName = null;
                string lastName = null;
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                String[] verifier = (Request.Url.AbsoluteUri).Split('?');
                if (verifier.Length > 1)
                {
                    string[] errors = verifier[1].Split('=');
                    if (verifier[0] != null)
                    {
                        if (errors[0] == "error")
                        {
                            loginInfo = null;
                            Session.Abandon();
                            //WebSecurity.Logout();   
                            AuthenticationManager.SignOut();
                        }
                    }
                }
                if (loginInfo == null)
                {
                    return RedirectToAction("Login");
                }
                if (loginInfo.Login.LoginProvider == "Facebook")
                {
                    firstName = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:facebook:first_name").Value;
                    lastName = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:facebook:last_name").Value;
                    if (loginInfo.Email != null)
                    {
                        email = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:facebook:email").Value;
                    }

                    if (email != null)
                    {
                        email = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:facebook:email").Value;

                        //Login
                        AuthenticationManager.SignIn(
                            new AuthenticationProperties
                            {
                                AllowRefresh = true,
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddDays(15)
                            },
                            new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, email) }, "External"));
                    }


                }
                if (loginInfo.Login.LoginProvider == "Google")
                {
                    var externalIdentity = AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                    var emailClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                    var lastNameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                    var givenNameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
                    if (emailClaim != null && emailClaim.Value != null)
                    {
                        email = emailClaim.Value;
                        //Login
                        AuthenticationManager.SignIn(
                                                         new AuthenticationProperties
                                                         {
                                                             AllowRefresh = true,
                                                             IsPersistent = true,
                                                             ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                                         }, new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, email) }, "External"));
                    }
                    if (givenNameClaim != null && givenNameClaim.Value != null)
                    {
                        firstName = givenNameClaim.Value;
                    }
                    else{
                        firstName = emailClaim.Value;
                    }
                    if (lastNameClaim != null && lastNameClaim.Value != null)
                    {
                        lastName = lastNameClaim.Value;
                    }
                }
                if (loginInfo.Login.LoginProvider == "LinkedIn")
                {
                    // EventLogUtility.WriteEntry("Inside LinkedIn");
                    var externalIdentity = AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                    var fName = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:linkedin:firstName").Value;
                    var lName = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:linkedin:lastName").Value;
                    var lEmail = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:linkedin:emailAddress").Value;
                    if (fName != null && fName != "null" && fName != "")
                    {
                        firstName = fName;

                    }
                    if (lName != null && lName != "null" && lName != "")
                    {
                        lastName = lName;
                    }
                    if (lEmail != null && lEmail != "null" && lEmail != "")
                    {
                        email = lEmail;
                        //Login
                        AuthenticationManager.SignIn(
                                                         new AuthenticationProperties
                                                         {
                                                             AllowRefresh = true,
                                                             IsPersistent = true,
                                                             ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                                         },
                                                         new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, email) }, "External"));
                    }
                    // EventLogUtility.WriteEntry("Inside LinkedIn" + firstName+lastName+email);
                }
                if (loginInfo.Login.LoginProvider == "Twitter")
                {
                    if (loginInfo.DefaultUserName != null)
                    {
                        firstName = loginInfo.DefaultUserName;
                    }
                    if (loginInfo.Email != null && loginInfo.Email != "" && loginInfo.Email != "undefined")
                    {
                        email = loginInfo.Email;
                        //Login
                        AuthenticationManager.SignIn(
                                                         new AuthenticationProperties
                                                         {
                                                             AllowRefresh = true,
                                                             IsPersistent = true,
                                                             ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                                         },
                            new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, email) }, "External"));
                    }
                    //var externalIdentity = AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                    //var emailClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                    //var lastNameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                    //var givenNameClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
                }

               

                // Sign in the user with this external login provider if the user already has a login
                User user;
                if (email != null)
                {
                    Session["UserEmailId"] = email;
                    user = mldb.Users.FirstOrDefault(p => p.EmailId == email);
                    if (user != null && user.IsActive == false)
                    {
                        TempData["AccountDeactivateFlag"] = "true";
                        return RedirectToAction("Login");
                    }

                    if (user != null && user.UserType != loginInfo.Login.LoginProvider)
                    {

                        TempData["ExtAlreadyExist"] = "true";
                        TempData["ExtProviderName"] = user.UserType;
                        loginInfo = null;
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ExtAlreadyExist"] = null;
                        TempData["ExtProviderName"] = null;
                    }
                }
                else
                {
                    user = mldb.Users.FirstOrDefault(p => p.UserName == loginInfo.DefaultUserName);
                    if (user != null && user.IsActive == false)
                    {
                        TempData["AccountDeactivateFlag"] = "true";
                        return RedirectToAction("Login");
                    }

                    if (user != null && user.UserType != loginInfo.Login.LoginProvider)
                    {
                        Session["UserEmailId"] = user.EmailId;
                        TempData["ExtAlreadyExist"] = "true";
                        TempData["ExtProviderName"] = user.UserType;
                        loginInfo = null;
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ExtAlreadyExist"] = null;
                        TempData["ExtProviderName"] = null;
                    }
                }
                //Session["UserName"] = loginInfo.DefaultUserName;
                Session["UserName"] = firstName;
                //Session["FirstName"] = firstName;
                if (user != null)
                {
                    //Added by Suraj
                    Session["UserEmailId"] = email;
                    Session["UserMobile"] = null;
                    Session["CouponCodeExternal"] = null;
                    //

                    //Session["UserName"] = loginInfo.DefaultUserName;
                    
                    if(firstName == null)
                    {
                        Session["UserName"] = email;
                    }
                    else
                    {
                        Session["UserName"] = firstName;
                    }
                    //Session["FirstName"] = firstName;
                    Session["UserID"] = user.UserId;
                    Session["RoleID"] = 2;
                    Session["UserType"] = user.UserType;
                    //return RedirectToAction("UserDashboard", "Home");

                    if (coupCode.Equals("null") || coupCode.Equals("") || coupCode.Equals("undefined"))
                    {
                        //return RedirectToAction("UserDashboard", "Home");
                        return RedirectToAction("Profile", "UserRegister", new { id = user.UserId });
                    }
                    else
                    {
                        if (coupCode.ToString().Length == 8)
                        {
                            return RedirectToAction("ActivateCouponCode", "CouponCode", new { vcc = coupCode.ToString(), userId = user.UserId, isExt = "true" });
                        }
                        else
                        {
                            return RedirectToAction("ActivateCouponCode", "CouponCode", new { vcc = coupCode, userId = user.UserId, isExt = "true" });
                        }
                    }
                }
                else
                {
                    Session["CouponCodeExternal"] = null;
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    User users = new User();
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    users.UserType = loginInfo.Login.LoginProvider;
                    users.ProviderKey = loginInfo.Login.ProviderKey;
                    users.UserName = loginInfo.DefaultUserName;
                    users.EmailId = email;
                    Session["UserEmailId"] = email;
                    users.IsActive = true;
                    mldb.Users.Add(users);
                    mldb.SaveChanges();
                    int userId = users.UserId;
                    Session["UserID"] = userId;
                    Session["RoleID"] = 2;
                    Session["UserType"] = loginInfo.Login.LoginProvider;
                    UserInRole usrInRole = new UserInRole();
                    usrInRole.UserId = userId;
                    usrInRole.RoleId = 2;
                    mldb.UserInRoles.Add(usrInRole);
                    mldb.SaveChanges();

                    UserDetails userDetails = new UserDetails();
                    userDetails.UserId = userId;
                    //userDetails.State = "Temp";
                    //userDetails.Country = "Temp";
                    userDetails.FirstName = firstName;
                    userDetails.LastName = lastName;
                    userDetails.IsActive = true;
                    userDetails.RegisteredDatetime = DateTime.Now;
                    string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                    userDetails.PartnerId = int.Parse(partId);
                    mldb.UsersDetails.Add(userDetails);
                    mldb.SaveChanges();

                    //New Referral Code for User Registrations
                    CouponGen cc = new CouponGen();
                    ReferralCodes refC = new ReferralCodes();
                    refC.ReferralCode = cc.GenerateReferralCodes();
                    refC.Referrer = users.UserId;
                    refC.ReceiverEmail = null;
                    refC.ReceiverPhoneNumber = null;
                    refC.SharedOn = null;
                    refC.IsCodeUsed = false;
                    refC.CodeUsedOn = null;
                    refC.Receiver = null;
                    refC.ReferrerRewardAmount = null;
                    refC.ReceiverRewardAmount = null;
                    refC.CodeGeneratedOn = DateTime.Now;
                    refC.CodeValidity = null;
                    refC.IsActive = true;
                    mldb.ReferralCodes.Add(refC);
                    mldb.SaveChanges();
                    //
                    //

                    //Send Email
                    if (email != null && email != "" && email != "null" && email != "undefined")
                    {

                        string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ExternalUserRegistration.html");

                        MillionLightsEmails mEmail = new MillionLightsEmails();
                        mEmail.SendExternalRegistrationCompleteEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                             ConfigurationManager.AppSettings["Telephone"],
                              ConfigurationManager.AppSettings["EmailId"],
                            "Your Edunetworks Registration Is Successful",
                            regTemplate,
                            firstName,
                            email
                            );

                        //email notification
                        UserNotitification userNotification = new UserNotitification();
                        userNotification.Receiver = userId;
                        userNotification.Sender = "System";
                        userNotification.Subject = Millionlights.Models.Constants.ExtUserRegSubNotification;
                        userNotification.Message = String.Format(Millionlights.Models.Constants.ExtUserRegMsgNotification, email);
                        userNotification.IsAlert = false;
                        userNotification.DateSent = DateTime.Now;
                        userNotification.ReadDate = null;
                        userNotification.SMSDate = null;
                        userNotification.MailDate = DateTime.Now;
                        userNotification.NotificationStatusId = 2;
                        mldb.UserNotitifications.Add(userNotification);
                        mldb.SaveChanges();

                        //Send SMS
                        if (userDetails.PhoneNumber != null)
                        {
                            try
                            {
                                Int64? numberToSend = userDetails.PhoneNumber;
                                //string msg = "Your Millionlights registration is successful. Loginid is " + user.EmailId + " and password is " + userReg.Password;
                                string msg = String.Format(Millionlights.Models.Constants.ExtUserRegMsgNotification, user.EmailId);
                                string url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
                                string smsRequestUrl = string.Format(url, numberToSend, msg);
                                HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
                                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                {
                                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                                    {
                                        StreamReader responseStream = new StreamReader(response.GetResponseStream());
                                        string resp = responseStream.ReadLine();
                                        // messageID = resp.Substring(33, 20);
                                    }
                                }

                                //  sms notification
                                int NotificationId = userNotification.Id;
                                UserNotitification userNotificationForSms = mldb.UserNotitifications.Find(NotificationId);
                                userNotificationForSms.SMSDate = DateTime.Now;
                                userNotificationForSms.NotificationStatusId = 2;
                                mldb.Entry(userNotificationForSms).State = EntityState.Modified;
                                mldb.SaveChanges();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    if (coupCode.Equals("null") || coupCode.Equals("") || coupCode.Equals("undefined"))
                    {
                        //return RedirectToAction("UserDashboard", "Home");
                        return RedirectToAction("Profile", "UserRegister", new { id = userId });
                    }
                    else
                    {
                        // Done by Archana 06.12.2016 For generating Referral code to new register
                        //New Code User Wallet
                        string refCode = coupCode.ToString();
                        if (refCode != null && refCode != "" && refCode.Length == 8)
                        {
                            var settings = mldb.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
                            decimal? rewardAmount = decimal.Parse(settings.RewardAmount);
                            var refCodes = mldb.ReferralCodes.Where(x => x.ReferralCode == refCode && x.IsActive == true).ToList();
                            //var emailReceiver = refCodes.Where(x => x.ReceiverEmail == email || x.ReceiverPhoneNumber == userRegister.PhoneNumber.ToString()).FirstOrDefault();
                            var emailReceiver = refCodes.Where(x => x.ReceiverEmail == email).FirstOrDefault();
                          
                            if (emailReceiver != null)
                            {
                                emailReceiver.Receiver = users.UserId;
                                mldb.Entry(emailReceiver).State = EntityState.Modified;
                                mldb.SaveChanges();

                                UserWallets uw = mldb.UserWallets.Where(x => x.UserId == users.UserId).FirstOrDefault();
                                if (uw != null)
                                {
                                    decimal? amount = uw.FinalAmountInWallet;
                                    uw.FinalAmountInWallet = amount + rewardAmount;
                                    mldb.Entry(uw).State = EntityState.Modified;
                                    mldb.SaveChanges();
                                }
                                else
                                {
                                    UserWallets wallet = new UserWallets();
                                    wallet.UserId = users.UserId;
                                    wallet.FinalAmountInWallet = rewardAmount;
                                    wallet.IsActive = true;
                                    mldb.UserWallets.Add(wallet);
                                    mldb.SaveChanges();
                                }
                            }
                        }
                        
                        // Done by Archana 06.12.2016 For Referral code
                        if (coupCode.ToString().Length == 8)
                        {
                            //New Code User Wallet
                            if (refCode != null && refCode != "")
                            {
                                return RedirectToAction("ActivateCouponCode", "CouponCode", new { vcc = coupCode.ToString(), userId = userId, isExt = "true" });
                            }
                            //var result1 = RedirectToAction("CheckReferralCode", "UserRegister", new { refCode = coupCode.ToString(), UserId = "null", isLoggedIn = false, EmailId = email, PhoneNumber = "null" });
                            else
                            {
                                return RedirectToAction("Profile", "UserRegister", new { id = userId });    
                            }
                        }

                        else
                        {
                            return RedirectToAction("ActivateCouponCode", "CouponCode", new { vcc = coupCode, userId = userId, isExt = "true" });
                        }
                    }
                    //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName});
                    //return View("~/Views/UserRegister/UserRegister.cshtml", "_Layout", new UserRegister { UserName = loginInfo.DefaultUserName, EmailId = loginInfo.Email, UserType = loginInfo.Login.LoginProvider, ProviderKey = loginInfo.Login.ProviderKey });
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }
       
        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    emailID = model.EmailId
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        SignIn(user, isPersistent: true);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {

            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [Route("Account/ResetMyPassword")]
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public ActionResult ResetMyPassword(string verificationId)
        {
            TempData["VerificationId"] = verificationId;
            return RedirectToAction("ResetPassword", "Account");
        }
        [AllowAnonymous]
        [AcceptVerbs("GET", "POST")]
        public ActionResult ResetPassword()
        {
            if (TempData["IsSuccess"] != null)
            {
                ViewBag.IsSuccess = TempData["IsSuccess"];
            }
            if (TempData["ResetPasswordMessage"] != null)
            {
                ViewBag.ResetPasswordMessage = TempData["ResetPasswordMessage"];
            }
            if (TempData["VerificationId"] != null)
            {
                ViewBag.VerificationId = TempData["VerificationId"];
            }
            if (TempData["VerificationId"] == null && TempData["IsSuccess"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [AcceptVerbs("GET", "POST")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult MyResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                if (resetPasswordViewModel.VerificationId != null)
                {
                    User user = mldb.Users.Where(x => x.EmailId == resetPasswordViewModel.Email && x.IsActive == true).FirstOrDefault();
                    if (user != null)
                    {
                        PasswordResetRequest IsUser = mldb.PasswordResetRequest.Where(x => x.VerificationId == resetPasswordViewModel.VerificationId && x.UserID == user.UserId).FirstOrDefault();
                        if (IsUser != null)
                        {
                            try
                            {
                                user.Password = userRegisterController.passwordEncrypt(resetPasswordViewModel.NewPassword);
                                mldb.Entry(user).State = EntityState.Modified;
                                mldb.SaveChanges();

                                IsUser.IsPasswordReset = true;
                                IsUser.PasswordResetDateTime = DateTime.Now;
                                mldb.Entry(IsUser).State = EntityState.Modified;
                                mldb.SaveChanges();

                                UserDetails userDetails = mldb.UsersDetails.Where(u => u.UserId == user.UserId).FirstOrDefault();
                                string cerTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "PasswordChangedEmail.htm");
                                MillionLightsEmails mEmail = new MillionLightsEmails();
                                mEmail.SendRegistrationCompleteEmail(
                                    ConfigurationManager.AppSettings["SenderName"],
                                    ConfigurationManager.AppSettings["SenderEmail"],
                                     ConfigurationManager.AppSettings["Telephone"],
                                      ConfigurationManager.AppSettings["EmailId"],
                                    "Reset password successfully",
                                    cerTemplate,
                                    userDetails.FirstName + " " + userDetails.LastName,
                                    user.EmailId,
                                   null);
                                TempData["IsSuccess"] = true;
                                TempData["ResetPasswordMessage"] = "Password Reset Successfully. You will be redirected to login page.";
                            }
                            catch (Exception)
                            {
                                TempData["ResetPasswordMessage"] = "Sorry, An error has occured while resetting the password. Please try again or contact the Edunetworks support team.";
                            }
                        }
                        else
                        {
                            TempData["ResetPasswordMessage"] = "Invalid user, the verification Id and UserId doesnt match. Please click the correct reset password link from reset password email.";
                        }
                    }
                    else
                    {
                        TempData["ResetPasswordMessage"] = "Please submit the valid registered email address";
                    }
                }
                else
                {
                    TempData["ResetPasswordMessage"] = "Please click the correct reset password link from reset password email.";
                }
            }
            else
            {
                TempData["ResetPasswordMessage"] = "Please click the correct reset password link from reset password email.";
            }
            return RedirectToAction("ResetPassword", "Account");
        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignIn(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut();
            var identity = UserManager.CreateIdentity(user, "External");
            AuthenticationManager.SignIn(
                                            new AuthenticationProperties
                                            {
                                                AllowRefresh = true,
                                                IsPersistent = isPersistent,
                                                ExpiresUtc = DateTime.UtcNow.AddDays(15)
                                            }, identity);
            return;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}