using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Millionlights.Models;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using System.Configuration;
using System.IO;
using Millionlights.EmailService;
using System.Net.Mime;
using Microsoft.Owin.Security;
using System.Security.Claims;
//using EdxIntegration;
using System.Web;
using System.Threading.Tasks;
using System.Globalization;

namespace Millionlights.Controllers
{
    public class UsersController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        UserRegisterController userRegisterController = new UserRegisterController();
        // GET: /Users/
        public ActionResult Index(int? page)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> UserRoles = new List<SelectListItem>();
            IEnumerable<Role> userRole = db.Roles.ToList();
            foreach (var item in userRole)
            {
                UserRoles.Add(new SelectListItem() { Text = item.RoleName, Value = item.RoleId.ToString() });
            }
            ViewBag.UserRoleList = UserRoles;

            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partnerType = db.Partners.Where(X => X.IsActive == true).ToList();

            foreach (var item in partnerType)
            {
                partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.PartnerID = partnerList;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserDetails model)
        {
            var cd = new ContentDisposition
            {
                FileName = "UserExportData.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        [HttpPost]
        public JsonResult GetUserDetails(int role, string daterange, string city, string state, string country, string radioval, int partner)
        {
            List<GetUserDetailsForViewResult> result = null;
            try
            {
                DateTime? fromDate1 = new DateTime();
                DateTime? toDate1 = new DateTime();
                if (daterange != "")
                {
                    var dates = daterange.Split('-');
                    var toDate = dates[1];
                    var fromDate = dates[0];
                    fromDate1 = DateTime.Parse(fromDate);
                    toDate1 = DateTime.Parse(toDate).AddDays(1).AddSeconds(-1);

                }
                else
                {
                    fromDate1 = null;
                    toDate1 = null;
                }
                int? roleId = null;
                if (role == 0)
                {
                    roleId = null;
                }
                else
                {
                    roleId = role;
                }
                int? partnerId = null;
                if (partner == 0)
                {
                    partnerId = null;
                }
                else
                {
                    partnerId = partner;
                }
                if (city == "")
                {
                    city = null;
                }
                if (country == "")
                {
                    country = null;
                }
                if (state == "")
                {
                    state = null;
                }

                List<UserRegister> userRegisterDetails = new List<UserRegister>();
                if (radioval == "self")
                {
                    string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                    int PId = int.Parse(partId);
                    MillionlightsDataContext md = new MillionlightsDataContext();
                    md.CommandTimeout = 0;
                    //result = md.GetUserDetails(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                    result = md.GetUserDetailsForView(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                }
                else
                {
                    string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                    int PId = int.Parse(partId);
                    MillionlightsDataContext md = new MillionlightsDataContext();
                    md.CommandTimeout = 0;
                    //result = md.GetUserDetails(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                    result = md.GetUserDetailsForView(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            var test = result;
            foreach(var res in result)
            {
                var regTime = res.RegisteredDatetime;
                res.RegisteredDatetimeString = regTime.Day + "/" + regTime.Month + "/" + regTime.Year + " " + regTime.Hour+":"+regTime.Minute+" "+ regTime.ToString("tt", CultureInfo.InvariantCulture); ;
            }
            //return Json(result, JsonRequestBehavior.AllowGet);
            var resultSet = Json(result, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;

        }
        // GET: /Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userdetails = db.UsersDetails.Find(id);
            if (userdetails == null)
            {
                return HttpNotFound();
            }
            return View(userdetails);
        }

        public ActionResult DeleteMultiplePartner(List<SearchUserModel> model)
        {
            User userModel = null;
            foreach (var users in model)
            {
                userModel = db.Users.Where(x => x.UserId == users.Id).FirstOrDefault();
                userModel.IsActive = false;
                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Users");
        }

        public class SearchUserModel
        {
            public int Id { get; set; }
        }


        // GET: /Users/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login","Account");
            }
            List<SelectListItem> UserRoles = new List<SelectListItem>();
            IEnumerable<Role> userRole = db.Roles.ToList();
            foreach (var item in userRole)
            {
                UserRoles.Add(new SelectListItem() { Text = item.RoleName, Value = item.RoleId.ToString() });
            }
            ViewBag.UserRoleList = UserRoles;
            //var UserInfo = db.Users.Where(X => X.IsActive == true).ToList();
            //ViewBag.UserDetail = UserInfo;
            return View();
        }

        // POST: /Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserRegister userReg)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User();
                    user.EmailId = userReg.EmailId;
                    user.UserName = userReg.UserName;
                    user.Password = userRegisterController.passwordEncrypt(userReg.Password);
                    user.IsActive = true;
                    if (userReg.UserType != null)
                    {
                        user.UserType = userReg.UserType;
                        user.ProviderKey = userReg.ProviderKey;
                    }
                    else
                    {
                        user.UserType = "Edunetwork";
                        user.ProviderKey = "00000";
                    }
                    db.Users.Add(user);
                    db.SaveChanges();
                    int userId = user.UserId;

                    UserInRole usrInRole = new UserInRole();
                    usrInRole.UserId = userId;
                    usrInRole.RoleId = userReg.RoleId;
                    db.UserInRoles.Add(usrInRole);
                    db.SaveChanges();

                    UserDetails userDetails = new UserDetails();
                    userDetails.UserId = userId;
                    userDetails.AddressLine1 = userReg.AddressLine1;
                    userDetails.AddressLine2 = userReg.AddressLine2;
                    userDetails.City = userReg.City;
                    userDetails.State = userReg.State;
                    userDetails.Country = userReg.Country;
                    userDetails.ZipCode = userReg.ZipCode;
                    userDetails.FirstName = userReg.FirstName;
                    userDetails.LastName = userReg.LastName;
                    userDetails.PhoneNumber = userReg.PhoneNumber;
                    userDetails.RegisteredDatetime = DateTime.Now;
                    string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                    userDetails.PartnerId = int.Parse(partId);
                    userDetails.IsActive = true;
                    db.UsersDetails.Add(userDetails);
                    db.SaveChanges();
                    //Create User in LMS
                    string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
                    string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
                    //Login
                    AuthenticationManager.SignIn(
                        new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddDays(15)
                        },
                        new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, user.EmailId) }, "External"));

                    //Send Email
                    string decryptedPass = userRegisterController.passwordDecrypt(user.Password);
                    string userName = userReg.FirstName + " " + userReg.LastName;
                    int? NotificationId = null;
                    if (user.EmailId != null)
                    {
                        string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "RegistrationEmail.htm");
                        MillionLightsEmails mEmail = new MillionLightsEmails();
                        mEmail.SendRegistrationCompleteEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                             ConfigurationManager.AppSettings["Telephone"],
                              ConfigurationManager.AppSettings["EmailId"],
                            "Your Millionlights Registration Successful",
                            regTemplate,
                            userName,
                            user.EmailId,
                            decryptedPass
                            );

                        //email notification
                        UserNotitification userNotification = new UserNotitification();
                        userNotification.Receiver = userId;
                        userNotification.Sender = "System";
                        userNotification.Subject = Constants.UserRegSubNotification;
                        userNotification.Message = String.Format(Constants.UserRegMsgNotification, user.EmailId, decryptedPass);
                        userNotification.IsAlert = false;
                        userNotification.DateSent = DateTime.Now;
                        userNotification.ReadDate = null;
                        userNotification.SMSDate = null;
                        userNotification.MailDate = DateTime.Now;
                        userNotification.NotificationStatusId = 2;
                        db.UserNotitifications.Add(userNotification);
                        db.SaveChanges();
                        NotificationId = userNotification.Id;
                    }
                    if (userReg.PhoneNumber != null)
                    {
                        Int64? numberToSend = userReg.PhoneNumber;
                        //string msg = "Your Millionlights registration is successful. Loginid is " + user.EmailId + " and password is " + userReg.Password;
                        string msg = String.Format(Constants.UserRegistrationSuccessMsg, user.EmailId, decryptedPass);
                        string url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
                        string smsRequestUrl = string.Format(url, numberToSend, msg);
                        HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                StreamReader responseStream = new StreamReader(response.GetResponseStream());
                                string resp = responseStream.ReadLine();
                            }
                        }
                        //  sms notification
                        if (NotificationId != null)
                        {
                            UserNotitification userNotificationForSms = db.UserNotitifications.Find(NotificationId);
                            userNotificationForSms.SMSDate = DateTime.Now;
                            userNotificationForSms.NotificationStatusId = 2;
                            db.Entry(userNotificationForSms).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                }
                catch (Exception)
                {
                }
            }
            return RedirectToAction("Index", "Users");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userdetails = db.UsersDetails.Where(X => X.UserId == id).FirstOrDefault();
            if (userdetails == null)
            {
                return HttpNotFound();
            }
            return View(userdetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserDetails usersDetails, int? id)
        {
            usersDetails = db.UsersDetails.Find(usersDetails.UserDetailsId);

            usersDetails.FirstName = Request["FirstName"];
            usersDetails.LastName = Request["LastName"];
            usersDetails.AddressLine1 = Request["AddressLine1"];
            usersDetails.AddressLine2 = Request["AddressLine2"];
            usersDetails.City = Request["City"];
            usersDetails.ZipCode = Request["ZipCode"];
            usersDetails.PhoneNumber = Convert.ToInt64(Request["PhoneNumber"]);
            //usersDetails.IsActive = true;
            db.Entry(usersDetails).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
          // return View(usersDetails);

        }
            
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userdetails = db.UsersDetails.Find(id);
            if (userdetails == null)
            {
                return HttpNotFound();
            }
            return View(userdetails);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserDetails userdetails = db.UsersDetails.Find(id);
            db.UsersDetails.Remove(userdetails);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Disabled(int? id, bool status)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            User userDisable = db.Users.Find(id);
            UserDetails userDetailDisable = db.UsersDetails.Where(X => X.UserId == id).FirstOrDefault();
            if (status == true)
            {
                userDisable.IsActive = false;
                userDetailDisable.IsActive = false;
            }
            else 
            {
                userDisable.IsActive = true;
                userDetailDisable.IsActive = true;
            }
         
            
            db.Entry(userDisable).State = System.Data.Entity.EntityState.Modified; 
            db.Entry(userDetailDisable).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Remove user from Edx
            string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
            return RedirectToAction("Index");
        }
        public ActionResult DeleteMultipleUsers(List<SearchUsersModel> userList)
        {
            User user  = null;

            //Remove user from Edx
            string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];

            foreach (var users in userList)
            {
                user = db.Users.Where(x => x.UserId == users.Id).FirstOrDefault();
                user.IsActive = false;
                UserDetails userDetailDisable = db.UsersDetails.Where(X => X.UserId == user.UserId).FirstOrDefault();
                userDetailDisable.IsActive = false;
                db.Entry(userDetailDisable).State = System.Data.Entity.EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        public ActionResult SendCredentials(List<SearchUsersModel> userList)
        {
            User user = null;
            var isSuccess = "false";
            foreach (var users in userList)
            {
                user = db.Users.Where(x => x.UserId == users.Id).FirstOrDefault();
                UserDetails userDetail = db.UsersDetails.Where(X => X.UserId == user.UserId).FirstOrDefault();
                try
                {
                    if (user.EmailId != null)
                    {
                        string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ResendPasswordEmailTemplate.htm");
                        string decryptedPass = userRegisterController.passwordDecrypt(user.Password);
                        MillionLightsEmails mEmail = new MillionLightsEmails();

                        mEmail.SendPasswordResendEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                             ConfigurationManager.AppSettings["Telephone"],
                            ConfigurationManager.AppSettings["EmailId"],
                            "Your MillionLights Password !!",
                            regTemplate,
                            userDetail.FirstName,
                            user.EmailId,
                            decryptedPass);
                        isSuccess = "true";
                    }
                }
                catch (Exception )
                {
                    isSuccess = "false";
                }
            }

            return Json(isSuccess);
        }
        public class SearchUsersModel
        {
            public int Id { get; set; }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<TmpUser> ReadCSVUsers(string csvFile )
        {
            return System.IO.File.ReadLines(csvFile)
                            .Select(line => line.Split(','))
                            .Select(tokens => new TmpUser { 
                                FirsrName = tokens[0],
                                LastName = tokens[1],
                                EmailId = tokens[2],
                                MobileNumber = tokens[3]
                            }).ToList();
        }

        private static void ImportUsers(List<TmpUser> tusers, string partnerType, out List<TmpUser> successList, out List<TmpUser> erroList)
        {
            successList = new List<TmpUser>();
            erroList = new List<TmpUser>();
    
            if (tusers[0].FirsrName.ToLower().Equals("FirstName"))
            {
                tusers.RemoveAt(0);
            }

            if (partnerType.ToLowerInvariant().Contains("telecom"))
            {
                erroList.AddRange(tusers.FindAll(tu => tu.MobileNumber.Trim() == string.Empty));
                successList.AddRange(tusers.FindAll(tu => tu.MobileNumber.Trim() != string.Empty));
            }
            else
            {
                erroList.AddRange(tusers.FindAll(tu => tu.EmailId.Trim() == string.Empty));
                successList.AddRange(tusers.FindAll(tu => tu.EmailId.Trim() != string.Empty));
            }
      
            using (var ctx = new MillionlightsContext())
            {
                using (var transactionScope = new TransactionScope())
                {
                    // some stuff in dbcontext
                    ctx.BulkInsert(successList);
                    ctx.SaveChanges();
                    transactionScope.Complete();
                }
            }
            return;
        }
    }
}
