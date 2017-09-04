using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using EntityFramework.BulkInsert.Extensions;
using Millionlights.EmailService;
using System.Configuration;
using System.Net;
using System.Diagnostics;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
namespace Millionlights.Controllers
{
    public class MarketingCampaignController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        UserRegisterController userRegisterController = new UserRegisterController();
        List<dynamic> importUserStatus = new List<dynamic>();
        dynamic dp = new ExpandoObject();
        //
        // GET: /MarketingCampaign/
        public ActionResult ImportUsers()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partnerType = db.Partners.Where(X => X.IsActive == true).ToList();

            foreach (var item in partnerType)
            {
                partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.PartnerID = partnerList;
            return View();
        }
        public ActionResult EmailCampaign()
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
        public ActionResult SMSCampaign()
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
        public ActionResult SelectUsers()
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
            return View();
        }
        public JsonResult UploadBankFile()
        {
            string path = "";
            int RecordCount = 0;
            var fileName = "";
            var file = Request.Files[0];
            if (Request.Files.Count > 0)
            {

                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/Content/CSVFiles/"), fileName);
                    file.SaveAs(path);
                    var lines = System.IO.File.ReadLines(path).Select(line => line.Split(';')).Skip(1);
                    RecordCount = lines.Count();
                }
            }
            int partnerId=int.Parse(Request.Form["partnerValue"]);
            string partnerName = Request.Form["partnerName"];
            //ImportBankUsers(path, partnerId, partnerName);Old
            //return Json(importUserStatus);


            //New Import Changes (Template and Selected Courses)
            string isNewTemplateUploaded = Request.Form["IsNewTemplateUploaded"];
            string selectedCourseIds = null;
            if (Request.Form["SelectedCourseIds"] != "")
            {
                selectedCourseIds = Request.Form["SelectedCourseIds"];
            }
            var fileName2 = "";
            string path2 = null;
            string emailTempPath = null;
            if (Request.Files.Count > 1)
            {
                var file2 = Request.Files[1];
                if (file2 != null && file2.ContentLength > 0)
                {
                    fileName2 = Path.GetFileName(file2.FileName);
                    path2 = Server.MapPath("~/ImportUserTemplates/");
                    if (!Directory.Exists(path2))
                    {
                        Directory.CreateDirectory(path2);
                    }
                    path2 = Path.Combine(path2, fileName2);
                    file2.SaveAs(path2);
                    emailTempPath = "ImportUserTemplates/" + fileName2;
                }
            }
            ImportBankUsers(path, partnerId, partnerName, selectedCourseIds, isNewTemplateUploaded, emailTempPath);
            return Json(importUserStatus);
        }
        //New Import Changes (Template and Selected Courses)
        private void ImportBankUsers(string csvFile, int partnerId, string partnerName, string selectedCourseIds, string isNewTemplateUploaded, string emailTempPath)
        {
            //New one
            MillionlightsDataContext md = new MillionlightsDataContext();
            try
            {
                md.ReseedIdentityOfUsersTable();
                // Read CSV file. skip header
                var lines = System.IO.File.ReadLines(csvFile).Select(line => line.Split(',')).Skip(1);
                int userId = 1;
                if (db.Users.Any())
                {
                    userId = db.Users.OrderByDescending(x => x.UserId).FirstOrDefault().UserId + 1;
                }
                int dupTmpuserId = 1;
                if (db.DuplicateRecords.Any())
                {
                    dupTmpuserId = db.DuplicateRecords.OrderByDescending(x => x.TmpId).FirstOrDefault().TmpId + 1;
                }
                List<UsersTable> tusers = lines
                    .Select(tokens => new UsersTable
                    {
                        UserId = userId++,
                        FirstName = tokens[0],
                        LastName = tokens[1],
                        EmailId = tokens[2].ToLower(),
                        PhoneNumber = ReturnPhoneNumber(tokens[3]),
                        AddressLine1 = tokens[5],
                        AddressLine2 = tokens[6],
                        City = tokens[7],
                        State = tokens[8],
                        Country = tokens[9],
                        ZipCode = tokens[10],
                        UserType = tokens[11],
                        PartnerId = partnerId,
                        RegisteredDatetime = DateTime.Now,
                        RoleId = 2,
                        IsActive = true,
                        Password = userRegisterController.passwordEncrypt(System.Guid.NewGuid().ToString().Substring(0, 6))
                    }).GroupBy(x => x.EmailId).Select(x => x.First()).ToList();
                
                TmpUser tmpUser = new TmpUser();
                var transactionScopeOptions = new TransactionOptions();
                transactionScopeOptions.Timeout = TimeSpan.MaxValue;
                using (var ctx = new MillionlightsContext())
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionScopeOptions))
                    {
                        // some stuff in dbcontext
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        ctx.BulkInsert(tusers, SqlBulkCopyOptions.KeepIdentity);
                        //ctx.BulkInsert(duplicate, SqlBulkCopyOptions.KeepIdentity);
                        ctx.SaveChanges();
                        stopWatch.Stop();
                        transactionScope.Complete();
                    }
                }
                md.CommandTimeout = 0;
                List<BatchExecuteUserDataResult> result = md.BatchExecuteUserData().ToList();
                var test = result;
                dp.DistinctUserCount = result.Count();
                dp.DuplicateUserCount = lines.Count() - result.Count();
                dp.TotalRecordsUploaded = lines.Count();
                dp = JsonConvert.SerializeObject(dp);
                importUserStatus.Add(dp);

                if (result.Count() > 0)
                {
                    SendBatchUserEmail(result, partnerName, selectedCourseIds, isNewTemplateUploaded, emailTempPath);
                }
            }
            catch (Exception)
            {
                md.ExecuteCommand("DELETE FROM UsersTable");
            }
        }

        public Int64? ReturnPhoneNumber(string phone)
        {
            if (phone != null && phone != "")
            {
                return Int64.Parse(phone);
            }
            else
            {
                return null;
            }
        }

        //New Import Changes (Template and Selected Courses)
        private void SendBatchUserEmail(List<BatchExecuteUserDataResult> finalUserList, string partnerName, string selectedCourseIds, string isNewTemplateUploaded, string emailTempPath)
        {
            using (var client = new MLApiController())
            {
                Task.Factory.StartNew(() =>
                {
                    client.SendEmailAsynch(finalUserList, partnerName, selectedCourseIds, isNewTemplateUploaded, emailTempPath);
                });
            }
        }

        //private void SendBatchUserEmail(List<BatchExecuteUserDataResult> finalUserList, string partnerName)
        //{
        //    using (var client = new MLApiController())
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            client.SendEmailAsynch(finalUserList,partnerName);
        //        });
        //        //bool response = client.SendEmailAsynch(finalUserList);
        //    }
        //    //var finalUserList=distinct.Join(db.UsersDetails,us=>us.UserId,ud=>ud.UserId, (us, ud) => new { us, ud }).ToList();
        //    //foreach (var item in finalUserList)
        //    //{
        //    //    //Send Email
        //    //    string userName = item.FirstName + " " + item.LastName;
        //    //string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ImportUsersRegistrationEmail.htm");
        //    //    string decryptedPass = userRegisterController.passwordDecrypt(item.Password);
        //    //    int? NotificationId = null;
        //    //    if (item.EmailId != null)
        //    //    {
        //    //        MillionLightsEmails mEmail = new MillionLightsEmails();
        //    //        mEmail.SendRegistrationCompleteEmail(
        //    //            ConfigurationManager.AppSettings["SenderName"],
        //    //            ConfigurationManager.AppSettings["SenderEmail"],
        //    //             ConfigurationManager.AppSettings["Telephone"],
        //    //              ConfigurationManager.AppSettings["EmailId"],
        //    //            "Your Millionlights Registration Is Successful",
        //    //            regTemplate,
        //    //            userName,
        //    //            item.EmailId,
        //    //            decryptedPass
        //    //            );
        //    //        //email notification
        //    //        UserNotitification userNotification = new UserNotitification();
        //    //        userNotification.Receiver = item.UserId;
        //    //        userNotification.Sender = "System";
        //    //        userNotification.Subject = Millionlights.Models.Constants.UserRegSubNotification;
        //    //        userNotification.Message = String.Format(Millionlights.Models.Constants.UserRegMsgNotification, item.EmailId, decryptedPass);
        //    //        userNotification.IsAlert = false;
        //    //        userNotification.DateSent = DateTime.Now;
        //    //        userNotification.ReadDate = null;
        //    //        userNotification.SMSDate = null;
        //    //        userNotification.MailDate = DateTime.Now;
        //    //        userNotification.NotificationStatusId = 2;
        //    //        db.UserNotitifications.Add(userNotification);
        //    //        db.SaveChanges();
        //    //        NotificationId = userNotification.Id;
        //    //    }
        //    //    //if (item.ud.PhoneNumber != null && item.ud.PhoneNumber != "null" && item.ud.PhoneNumber != "NULL" && item.ud.PhoneNumber != "" && item.ud.PhoneNumber != "Null")
        //    //    Int64? phNumber = null;
        //    //    if (item.PhoneNumber != null)
        //    //    {
        //    //        phNumber=Int64.Parse(item.PhoneNumber);
        //    //    }
        //    //    if (phNumber != null && phNumber != 0)
        //    //    {
        //    //        //Send SMS
        //    //        try
        //    //        {
        //    //            Int64? numberToSend = phNumber;
        //    //            //string msg = "Your Millionlights registration is successful. Loginid is " + user.EmailId + " and password is " + userReg.Password;
        //    //            string msg = String.Format(Millionlights.Models.Constants.UserRegistrationSuccessMsg, item.EmailId, decryptedPass);
        //    //            string url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
        //    //            string smsRequestUrl = string.Format(url, numberToSend, msg);
        //    //            HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
        //    //            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    //            {
        //    //                if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    //                {
        //    //                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
        //    //                    string resp = responseStream.ReadLine();
        //    //                    // messageID = resp.Substring(33, 20);
        //    //                }
        //    //            }

        //    //            //  sms notification
        //    //            if (NotificationId != null)
        //    //            {
        //    //                UserNotitification userNotificationForSms = db.UserNotitifications.Find(NotificationId);
        //    //                userNotificationForSms.SMSDate = DateTime.Now;
        //    //                userNotificationForSms.NotificationStatusId = 2;
        //    //                db.Entry(userNotificationForSms).State = EntityState.Modified;
        //    //                db.SaveChanges();
        //    //            }
        //    //        }
        //    //        catch (Exception)
        //    //        {
        //    //            continue;
        //    //        }
        //    //    }
        //    //    //New Requirement - 
        //    //    //Registered the imported user directly &
        //    //    //Enrolled them to a few courses
        //    //    //var list = new List<Tuple<int, int>>();
        //    //    //list.Add(new Tuple<int, int>(17, 32));
        //    //    string courseIdsList = "17;32";
        //    //    string[] courseIdsArray = courseIdsList.Split(';');
        //    //    for (int i = 0; i < courseIdsArray.Count(); i++)
        //    //    {
        //    //        int cId = int.Parse(courseIdsArray[i]);
        //    //        var course = db.Courses.Where(x => x.Id == cId).FirstOrDefault();
        //    //        if (course != null)
        //    //        {
        //    //            UsersCourses userCourse = new UsersCourses();
        //    //            userCourse.UserId = item.UserId;
        //    //            userCourse.CourseID = course.Id;
        //    //            userCourse.CreatedOn = DateTime.Now;
        //    //            userCourse.Status = true;
        //    //            userCourse.VoucherAssignedOn = DateTime.Now;
        //    //            userCourse.CanAccessLMS = true;
        //    //            userCourse.CanPay = true;
        //    //            userCourse.CanAccessCertification = true;
        //    //            userCourse.IsActive = true;
        //    //            db.UsersCourses.Add(userCourse);
        //    //            db.SaveChanges();

        //    //            //API - Sergiy
        //    //            //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");

        //    //            string lmsCourseId = GetLMSCourseId(course.EDXCourseLink);
        //    //            if (lmsCourseId != null)
        //    //            {
        //    //                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
        //    //                httpWebRequest.ContentType = "application/json";
        //    //                httpWebRequest.Method = "POST";
        //    //                httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
        //    //                try
        //    //                {
        //    //                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    //                    {
        //    //                        //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

        //    //                        //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
        //    //                        string json = "{\"email\":\"" + item.EmailId + "\"," +

        //    //                                      "\"course_id\":\"" + lmsCourseId + "\"}";
        //    //                        streamWriter.Write(json);
        //    //                        streamWriter.Flush();
        //    //                        streamWriter.Close();
        //    //                    }
        //    //                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    //                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    //                    {
        //    //                        var result = streamReader.ReadToEnd();
        //    //                    }
        //    //                }
        //    //                catch (Exception e)
        //    //                {
        //    //                    continue;
        //    //                }

        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}
        public string GetLMSCourseId(string EDXCourseLink)
        {
            var message = string.Format("The course with this LMSID is not found. Please check the LMS Course ID.");
            string[] edxLinksArr = EDXCourseLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);
            if (edxLinksArr.Length > 1)
            {
                //string[] splitLMSCourseId = edxLinksArr[1].Split('/');
                ////EDXCourseLink = edxLinksArr[1];
                //if (splitLMSCourseId.Length > 1)
                //{
                //    EDXCourseLink = splitLMSCourseId[0] + "/" + splitLMSCourseId[1] + "/" + splitLMSCourseId[2];
                //    EDXCourseLink = EDXCourseLink.EndsWith('/');
                //}
                if (edxLinksArr[1].EndsWith("/"))
                {
                    EDXCourseLink = edxLinksArr[1].TrimEnd(new[] { '/' });
                }
                else
                {
                    EDXCourseLink = edxLinksArr[1];
                }
            }
            else
            {
                EDXCourseLink = null;
            }
            return EDXCourseLink;
        }
        [HttpPost]
        public JsonResult GetUserDetails(int role, string daterange, string city, string state, string country, string radioval, int partner, string radioval2)
        {
            List<GetUserDetailsResult> result = null;
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
                    toDate1 = DateTime.Parse(toDate);

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
                ;
                if (radioval == "self")
                {
                    string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                    int PId = int.Parse(partId);
                    //fromDate1 = DateTime.Parse(fromDate1.ToShortDateString());
                    //toDate1 = DateTime.Parse(toDate1.ToShortDateString());
                    MillionlightsDataContext md = new MillionlightsDataContext();
                    result = md.GetUserDetails(roleId, fromDate1, toDate1, city, state, country, PId).ToList();
                    //var userInfo = db.UsersDetails.Join(db.UserInRoles, ud => ud.UserId, uir => uir.UserId, (ud, uir) => new { ud, uir })
                    //                .Where(Y => string.IsNullOrEmpty(roleId.ToString()) || Y.uir.RoleId == roleId)
                    //                .Select(X => X.ud)
                    //                .Join(db.Users, ud => ud.UserId, u => u.UserId, (ud, u) => new { ud, u })
                    //                .Where((X => (string.IsNullOrEmpty(X.ud.PartnerId.ToString()) || X.ud.PartnerId.Equals(PId)) && (string.IsNullOrEmpty(city) || X.ud.City.Equals(city)) && (string.IsNullOrEmpty(country) || X.ud.Country.Equals(country)) && (string.IsNullOrEmpty(state) || X.ud.State.Equals(state))))
                    //                .Select(X => X.ud)
                    //                .Where(X => X.IsActive == true)
                    //                .ToList();

                    //var tempUserInfo = userInfo;
                    //if (!string.IsNullOrEmpty(fromDate1.ToString()) && !string.IsNullOrEmpty(toDate1.ToString()) && fromDate1 != DateTime.MinValue && toDate1 != DateTime.MinValue)
                    //{
                    //    tempUserInfo = tempUserInfo.Where(x => x.RegisteredDatetime.Date >= fromDate1.Date && x.RegisteredDatetime.Date <= toDate1.Date).ToList();
                    //}

                    //foreach (var user in tempUserInfo)
                    //{
                    //    UserRegister userNew = new UserRegister();
                    //    userNew.FullName = user.FirstName + " " + user.LastName;
                    //    userNew.State = user.State;
                    //    userNew.Country = user.Country;
                    //    userNew.City = user.City;
                    //    userNew.PhoneNumber = user.PhoneNumber;
                    //    userNew.PartnerId = user.PartnerId;
                    //    userNew.UserId = user.UserId;
                    //    userNew.RegisteredDatetime = user.RegisteredDatetime;
                    //    var data = db.Users.Find(user.UserId);
                    //    userNew.EmailId = data.EmailId;
                    //    userNew.UserName = data.UserName;
                    //    userNew.IsActive = user.IsActive;
                    //    var roles = db.UserInRoles.Where(x => x.UserId == user.UserId).FirstOrDefault();
                    //    userNew.RoleId = roles.RoleId;
                    //    userRegisterDetails.Add(userNew);
                    //}
                    //UserRegs = userRegisterDetails;

                }
                else
                {
                    if(radioval2=="partnerTempUser"){
                        MillionlightsDataContext md = new MillionlightsDataContext();
                        result = md.GetUserDetails(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                        //var userInfo = db.UsersDetails.Join(db.UserInRoles, ud => ud.UserId, uir => uir.UserId, (ud, uir) => new { ud, uir }).Where(Y => string.IsNullOrEmpty(roleId.ToString()) || Y.uir.RoleId == roleId)
                        //          .Select(X => X.ud)
                        //          .Join(db.Users, ud => ud.UserId, u => u.UserId, (ud, u) => new { ud, u })
                        //          .Where((X => (string.IsNullOrEmpty(X.ud.PartnerId.ToString()) || X.ud.PartnerId == partnerId) &&
                        //          (string.IsNullOrEmpty(city) || X.ud.City.Equals(city)) &&
                        //          (string.IsNullOrEmpty(country) || X.ud.Country.Equals(country)) &&
                        //          (string.IsNullOrEmpty(state) || X.ud.State.Equals(state))))
                        //          .Select(X => X.ud)
                        //          .Where(X => X.IsActive == true)
                        //          .ToList();
                        //var tempUserInfo = userInfo;
                        //if (!string.IsNullOrEmpty(fromDate1.ToString()) && !string.IsNullOrEmpty(toDate1.ToString()) && fromDate1 != DateTime.MinValue && toDate1 != DateTime.MinValue)
                        //{
                        //    tempUserInfo = tempUserInfo.Where(x => x.RegisteredDatetime.Date >= fromDate1.Date && x.RegisteredDatetime.Date <= toDate1.Date).ToList();
                        //}

                        //foreach (var user in tempUserInfo)
                        //{
                        //    UserRegister userNew = new UserRegister();
                        //    userNew.FullName = user.FirstName + " " + user.LastName;
                        //    userNew.State = user.State;
                        //    userNew.Country = user.Country;
                        //    userNew.City = user.City;
                        //    userNew.PhoneNumber = user.PhoneNumber;
                        //    userNew.PartnerId = user.PartnerId;
                        //    userNew.UserId = user.UserId;
                        //    userNew.RegisteredDatetime = user.RegisteredDatetime;
                        //    var data = db.Users.Find(user.UserId);
                        //    userNew.EmailId = data.EmailId;
                        //    userNew.UserName = data.UserName;
                        //    userNew.IsActive = user.IsActive;
                        //    var roles = db.UserInRoles.Where(x => x.UserId == user.UserId).FirstOrDefault();
                        //    userNew.RoleId = roles.RoleId;
                        //    userRegisterDetails.Add(userNew);
                        //}
                        //UserRegs = userRegisterDetails;
                    }
                    else{
                        MillionlightsDataContext md = new MillionlightsDataContext();
                        result = md.GetUserDetails(roleId, fromDate1, toDate1, city, state, country, partnerId).ToList();
                        //var userInfo = db.UsersDetails.Join(db.UserInRoles, ud => ud.UserId, uir => uir.UserId, (ud, uir) => new { ud, uir }).Where(Y => string.IsNullOrEmpty(roleId.ToString()) || Y.uir.RoleId == roleId)
                        //                                  .Select(X => X.ud)
                        //                                  .Join(db.Users, ud => ud.UserId, u => u.UserId, (ud, u) => new { ud, u })
                        //                                  .Where((X => (string.IsNullOrEmpty(X.ud.PartnerId.ToString()) || X.ud.PartnerId == partnerId) &&
                        //                                  (string.IsNullOrEmpty(city) || X.ud.City.Equals(city)) &&
                        //                                  (string.IsNullOrEmpty(country) || X.ud.Country.Equals(country)) &&
                        //                                  (string.IsNullOrEmpty(state) || X.ud.State.Equals(state))))
                        //                                  .Select(X => X.ud)
                        //                                  .Where(X => X.IsActive == true)
                        //                                  .ToList();
                        //var tempUserInfo = userInfo;
                        //if (!string.IsNullOrEmpty(fromDate1.ToString()) && !string.IsNullOrEmpty(toDate1.ToString()) && fromDate1 != DateTime.MinValue && toDate1 != DateTime.MinValue)
                        //{
                        //    tempUserInfo = tempUserInfo.Where(x => x.RegisteredDatetime.Date >= fromDate1.Date && x.RegisteredDatetime.Date <= toDate1.Date).ToList();
                        //}

                        //foreach (var user in tempUserInfo)
                        //{
                        //    UserRegister userNew = new UserRegister();
                        //    userNew.FullName = user.FirstName + " " + user.LastName;
                        //    userNew.State = user.State;
                        //    userNew.Country = user.Country;
                        //    userNew.City = user.City;
                        //    userNew.PhoneNumber = user.PhoneNumber;
                        //    userNew.PartnerId = user.PartnerId;
                        //    userNew.UserId = user.UserId;
                        //    userNew.RegisteredDatetime = user.RegisteredDatetime;
                        //    var data = db.Users.Find(user.UserId);
                        //    userNew.EmailId = data.EmailId;
                        //    userNew.UserName = data.UserName;
                        //    userNew.IsActive = user.IsActive;
                        //    var roles = db.UserInRoles.Where(x => x.UserId == user.UserId).FirstOrDefault();
                        //    userNew.RoleId = roles.RoleId;
                        //    userRegisterDetails.Add(userNew);
                        //}
                        //UserRegs = userRegisterDetails;
                    }
                    
                }
            }
            catch (Exception)
            {
            }
            //return Json(result, JsonRequestBehavior.AllowGet);
            var resultSet = Json(result, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;

        }
        [HttpPost]
        public JsonResult SendEmail(string recipient, string subject, string message)
        {
            List<string> recipients = recipient.Split(';').Select(x=>Convert.ToString(x)).ToList();
               var isUser  = "false";
                try
                {
                    string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                    string senderName= ConfigurationManager.AppSettings["SenderName"];
                    MandrillEmailService.EmailService.SendFormattedEmail(senderEmail, senderName, subject, recipients, message);
                     isUser = "true";

                    //Send Email to MillionLights Support
                     //string querySubject = "Contact Request";
                     //string queryMsg = "";
                     //MandrillEmailService.EmailService.SendFormattedEmail(senderEmail, senderName, querySubject, recipients, message);
                }
                catch (Exception)
                {
                    isUser = "false";
                }
          
            return Json(isUser);
        }
        [HttpPost]
        public JsonResult SendSMS(string recipient,string message)
        {
            string[] recipients = recipient.Split(';');
            var isUser = "false";
            try
            {
                for (int i = 0; i < recipients.Count(); i++)
                {
                    if (recipients[i] == " ")
                    {

                    }
                    else
                    {
                        Int64? numberToSend = Int64.Parse(recipients[i]);
                        if (numberToSend != null)
                        {
                            string msg = message;
                            string url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];

                            string smsRequestUrl = string.Format(url, numberToSend, msg);
                            HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.Equals(HttpStatusCode.OK))
                                {
                                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
                                    string resp = responseStream.ReadLine();
                                    //messageID = resp.Substring(33, 20);
                                    isUser = "true";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                isUser = "false";
            }

            return Json(isUser);
        }

        [HttpPost]
        public JsonResult CustomersQuery(string queryEmail, string fullName, string message, string phoneNumber)
        {

            List<string> recipients = queryEmail.Split(';').Select(x => Convert.ToString(x)).ToList();
            var isUser = "false";
            try
            {
                if (fullName == null || fullName == "")
                {
                    fullName = "User";
                }
                
                string subject = "Thank you for submitting the query to Edunetworks, we'll get back to you soon!";
                string txtMessage = "Dear " + fullName + "," + "<br/> <br/> " + " We have submitted your query and will get back to you soon, Thank you!"+ "<br/> <br/>" + "Warm Regards," + "<br/> <br/> " + "Edunetworks Team" + "<br/> <br/>" + "Telephone: 011 025 0089 / 011 795 3771" + "<br/> <br/>" + "Email:support@edunetworks.com";
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string senderName = ConfigurationManager.AppSettings["SenderName"];
                MandrillEmailService.EmailService.SendFormattedEmail(senderEmail, senderName, subject, recipients, txtMessage);
                string supportSubject = "Contact request from -" + " " + fullName;
                string supportMessage = "Name:" + fullName + "<br/> " + "Email:" + queryEmail + "<br/> " + "PhoneNumber:" + phoneNumber + "<br/> " + "Message:" + message;
                //MandrillEmailService.EmailService.SendIndividualEmail(queryEmail, fullName, supportSubject, senderEmail, supportMessage);
                MandrillEmailService.EmailService.SendIndividualEmail(senderEmail, fullName, supportSubject, senderEmail, supportMessage);
                isUser = "true";
            }
            catch (Exception)
            {
                isUser = "false";
            }
            if (phoneNumber != null)
            {
                //Send SMS
                Int64? numberToSend = Int64.Parse(phoneNumber);
                string msg = String.Format(Millionlights.Models.Constants.ContactUsSMSMsg);
                var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
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
            }
            return Json(isUser);
        }

        public ActionResult ReferralCode()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public JsonResult GetRefCodes(string dateRange, string statusId)
        {
            DateTime? fromDate1 = new DateTime();
            DateTime? toDate1 = new DateTime();
            if (dateRange != "" && dateRange != "null")
            {
                var dates = dateRange.Split('-');
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
            //int? status = null;
            //if (statusId != null && statusId != "")
            //{
            //    status = int.Parse(statusId);
            //}
            List<ReferralCodes> result = null;
            //Shared but not used
            if(statusId=="1"){
                if(dateRange==null || dateRange=="null"){
                    result = db.ReferralCodes.Where(x=>x.SharedOn!=null && x.IsCodeUsed==false).ToList();
                }
                else{
                    result = db.ReferralCodes.Where(x=>x.SharedOn!=null && x.IsCodeUsed==false && (x.CodeGeneratedOn>=fromDate1 && x.CodeGeneratedOn<=toDate1)).ToList();
                }
            }
            //Shared and Used
            if(statusId=="2"){
                if(dateRange==null || dateRange=="null"){
                    result = db.ReferralCodes.Where(x=>x.SharedOn!=null && x.IsCodeUsed==true).ToList();
                }
                else{
                    result = db.ReferralCodes.Where(x=>x.SharedOn!=null && x.IsCodeUsed==true && (x.CodeGeneratedOn>=fromDate1 && x.CodeGeneratedOn<=toDate1)).ToList();
                }
            }
            //Not shared yet
            if(statusId=="3"){
                if (dateRange == null || dateRange == "null")
                {
                    result = db.ReferralCodes.Where(x => x.SharedOn == null && x.IsCodeUsed == false).ToList();
                }
                else
                {
                    result = db.ReferralCodes.Where(x => x.SharedOn == null && x.IsCodeUsed == false && (x.CodeGeneratedOn >= fromDate1 && x.CodeGeneratedOn <= toDate1)).ToList();
                }
            }
            if(statusId=="null"){
                result = db.ReferralCodes.Where(x=>(x.CodeGeneratedOn>=fromDate1 && x.CodeGeneratedOn<=toDate1)).ToList();
            }
            var resultSet = Json(result, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }

        //New Change
        [HttpPost]
        public JsonResult GetFreeCourseDetails()
        {
            decimal? freeCoursePrice=decimal.Parse("0.00");
            IEnumerable<Course> courseDetails = db.Courses.Where(x => x.IsActive == true && x.BasePrice <1).ToList();
            List<dynamic> miniCourses = new List<dynamic>();
            foreach (var item in courseDetails)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.CourseName = item.CourseName;
                //p.stDateString = item.stDateString;
                //p.endDateString = item.endDateString;
                p.BasePrice = item.BasePrice;
                p.Duration = item.Duration;
                p.NoOfSessions = item.NoOfSessions;
                p.CourseCode = item.CourseCode;
                p.EnableForCertification = item.EnableForCertification;
                p.DisplayOnHomePage = item.DisplayOnHomePage;
                //p.IsActive = item.IsActive;
                p = JsonConvert.SerializeObject(p);
                miniCourses.Add(p);
            }
            //return Json(miniCourses);

            var resultSet = Json(miniCourses, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }

        public string CreateHTMLFile(string emailTemplateData)
        {
            string merchantEmailPath = "/ImportUserTemplates/";
            string path = Path.Combine(Server.MapPath(merchantEmailPath));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = string.Format("{0}_{1}.htm", "ImportUsersRegistrationEmail", (DateTime.Now.Ticks));
            string fileLoc = string.Format(@"{0}\{1}", path, fileName);

            using (StreamWriter sw = new StreamWriter(fileLoc))
                using (HtmlTextWriter writer = new HtmlTextWriter(sw))
                {
                    //writer.RenderBeginTag(HtmlTextWriterTag.Html);
                    //writer.RenderBeginTag(HtmlTextWriterTag.Body);
                    string emailContent = emailTemplateData;
                    writer.Write(emailContent);
                    //writer.RenderEndTag();
                    //writer.RenderEndTag();
                }
            return fileLoc;
        }
	}
}