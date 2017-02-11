using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;
using System.Net;
using Millionlights.EmailService;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

namespace Millionlights.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public ActionResult StarRatings()
        {
            return View();
        }

        private MillionlightsContext db = new MillionlightsContext();
        [AllowAnonymous]
        public ActionResult Index(int? id, string type,string logout, int page = 1)
        {
            if (Session["RoleID"] != null)
            {
                //Goto Admin Dashboard if user is Admin
                int roleId = Convert.ToInt32(Session["RoleID"]);
                var usr = db.Roles.Where(r => r.RoleId == roleId).FirstOrDefault();
               
                if (usr.RoleName == "Admin")
                {
                    return View("Dashboard");
                }
            }
            else
            {
                //Logut the OAuth
                AuthenticationManager.SignOut();
                AuthenticationManager.SignOut("External");
            }

            List<ShortCourse> activeCourses = new List<ShortCourse>();

            //// Commented by Archana 09.11.2016
            //if (Session["AllCourses"] == null)
            //{
            //    activeCourses = GetAllActiveCourses();
            //    Session["AllCourses"] = activeCourses;
            //}
            //else
            //{
            //    activeCourses = (List<ShortCourse>)Session["AllCourses"];
            //}

            //// Done by Archana 09.11.2016
            activeCourses = GetAllActiveCourses();
            Session["AllCourses"] = activeCourses;

            List<PartnerImage> activePartners = GetActivePartnerImages();

            ViewBag.AllPartner = activePartners;

            //Free and Paid Courses
            var freeCourses = activeCourses.Where(x => x.BasePrice == decimal.Parse("0.00") && x.DisplayonHomePage == true).OrderBy(x => x.CoursePriority).Take(8).ToList();
            ViewBag.FreeCourses = freeCourses;
            ViewBag.FreeCoursesCounter = freeCourses.Count;

            //int? uId = null;
            //if (Session["UserID"] != null)
            //{
            //    uId = int.Parse(Session["UserID"].ToString());
            //    foreach (var fc in freeCourses)
            //    {
            //        var isCourseEnrolled = db.UsersCourses.Where(x => x.CourseID == fc.Id && x.UserId == uId).ToList();
            //        if (isCourseEnrolled.Count() > 0)
            //        {
            //            fc.IsCourseEnrolled = true;
            //            fc.EmailCourseName = "Hello, I have enrolled for the online course " + fc.CourseName.Trim() + " , from Millionlights. This course might interest you too. Please have a look on Millionlights.org";
            //        }
            //    }
            //}

            var paidCourses = activeCourses.Where(x => x.BasePrice != decimal.Parse("0.00") && x.DisplayonHomePage == true).OrderBy(x => x.CoursePriority).Take(8).ToList();
            ViewBag.PaidCourses = paidCourses;
            ViewBag.PaidCoursesCounter = paidCourses.Count;


            //if (Session["UserID"] != null)
            //{
            //    uId = int.Parse(Session["UserID"].ToString());
            //    foreach (var pc in paidCourses)
            //    {
            //        var isCourseEnrolled = db.UsersCourses.Where(x => x.CourseID == pc.Id && x.UserId == uId).ToList();
            //        if (isCourseEnrolled.Count() > 0)
            //        {
            //            pc.IsCourseEnrolled = true;
            //            pc.EmailCourseName = "Hello, I have enrolled for the online course " + pc.CourseName.Trim() + " , from Millionlights. This course might interest you too. Please have a look on Millionlights.org";
            //        }
            //    }
            //}

            var user = db.Users.Where(x => x.IsActive == true).Count();
            ViewBag.UserCount = user;

            var courses = db.Courses.Where(X => X.IsActive == true).Count();
            ViewBag.CoursesCount = courses;

            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            HomePageConfiguration homePage = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.Video = homePage;
            ViewBag.IsLogout = logout;

            //Get Categories Async for the Courses page
            Task.Run(() => {
                LoadCourseCategories();
            });

            return View();
        }

        private void LoadCourseCategories()
        {
            if( Session["CourseCategories"] != null )
            {
                return;
            }

            try
            {
                List<CourseDetailsCategory> courseCatList = new List<CourseDetailsCategory>();

                var courseCat = db.CourseCategories.ToList();
                foreach (var item in courseCat)
                {
                    CourseDetailsCategory courseDetailsCategory = new CourseDetailsCategory();
                    courseDetailsCategory.ID = item.Id;
                    courseDetailsCategory.CategoryName = item.Name;

                    // Done by Archana 25.01.2017
                    var listCount = db.Courses.Where(delegate(Course cor)
                    {

                        List<int> listTemp = cor.CourseCategory.Split(',').Select(Int32.Parse).ToList();

                        return listTemp.Contains(Convert.ToInt32(item.Id));
                    }).ToList().Count();

                    courseDetailsCategory.CourseCount = listCount;
                    courseCatList.Add(courseDetailsCategory);

                    // Commented by archana 25.01.2017
                    //courseDetailsCategory.CourseCount = db.Courses.Where(x => x.CourseCategory == item.Id.ToString() && x.IsActive == true).Count();    

                    // Logic by Archana 25.01.2017
                    //var courseList = db.Courses.Where(x => x.IsActive == true).ToList();
                    //int courseCount = 0;
                    //foreach (var c in courseList)
                    //{

                    //    string[] tempId;
                    //    tempId = c.CourseCategory.ToString().Split(',');
                    //    for (int i = 0; i < tempId.Length; i++)
                    //    {
                    //        if (tempId[i] == item.Id.ToString())
                    //        {
                    //            courseCount = courseCount + 1;
                    //        }
                    //    }
                    //}
                    //courseDetailsCategory.CourseCount = courseCount;
                   
                }

                var courseLevel = db.CourseLevels.ToList();
                foreach (var item in courseLevel)
                {
                    CourseDetailsCategory courseDetailsCategory = new CourseDetailsCategory();
                    courseDetailsCategory.ID = item.Id;
                    courseDetailsCategory.CourseLevel = item.Name;
                    courseDetailsCategory.CourseCount = db.Courses.Where(x => x.CourseLevels == item.Id && x.IsActive == true).Count();
                    courseCatList.Add(courseDetailsCategory);
                }
                var courseLanguage = db.CourseLanguages.ToList();
                foreach (var item in courseLanguage)
                {
                    CourseDetailsCategory courseDetailsCategory = new CourseDetailsCategory();
                    courseDetailsCategory.ID = item.Id;
                    courseDetailsCategory.CourseLanguage = item.Name;
                    courseDetailsCategory.CourseCount = db.Courses.Where(x => x.CourseLanguage == item.Id && x.IsActive == true).Count();
                    courseCatList.Add(courseDetailsCategory);
                }
                var courseType = db.CourseTypes.ToList();
                foreach (var item in courseType)
                {
                    CourseDetailsCategory courseDetailsCategory = new CourseDetailsCategory();
                    courseDetailsCategory.ID = item.Id;
                    courseDetailsCategory.CourseType = item.Name;
                    courseDetailsCategory.CourseCount = db.Courses.Where(x => x.CourseTypes == item.Id && x.IsActive == true).Count();
                    courseCatList.Add(courseDetailsCategory);
                }
                var courseAvail = db.CourseAvailability.ToList();
                foreach (var item in courseAvail)
                {
                    CourseDetailsCategory courseDetailsCategory = new CourseDetailsCategory();
                    courseDetailsCategory.ID = item.Id;
                    courseDetailsCategory.CourseAvailability = item.Name;
                    courseDetailsCategory.CourseCount = db.Courses.Where(x => x.CourseAvailability == item.Id && x.IsActive == true).Count();
                    courseCatList.Add(courseDetailsCategory);
                }
                Session["CourseCategories"] = courseCatList;
            }
            catch(Exception )
            {

            }
            return;
        }

        public ActionResult Career()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.IsSuccess = TempData["Success"];
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetCourseEmailext(string courseId, string courseName)
        {
            int cId;
            int.TryParse(courseId, out cId);

            int uId = -1;
            if (Session["UserID"] != null)
            {
                int.TryParse(Session["UserID"].ToString(), out uId);
            }

            string emailText = "";

            var isCourseEnrolled = db.UsersCourses.Where(x => x.CourseID == cId && x.UserId == uId).ToList();

            if (isCourseEnrolled.Count() > 0)
            {
                emailText = "Hello, I have enrolled for the online course " + courseName +
                    " , from Millionlights. This course might interest you too. Please have a look on Millionlights.org";
            }
            else
            {
                emailText = "Hello, I came across the online course " + courseName +
                    " , from Millionlights. This course might interest you. Please have a look on Milionlights.org";
            }

            return Json(emailText, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HonorCode()
        {
            var terms = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.TermsAndCondition = terms;
            return View();
        }
        public ActionResult MLPrivacyPolicies()
        {
            return View();
        }
        public ActionResult SetIsRegister(string key, string value)
        {
            Session[key] = value;

            return this.Json(new { success = true });
        }
        public ActionResult GetLocation()
        {
            return View();
        }
        public ActionResult Settings()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            HomePageConfiguration settingsPageData = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.message = TempData["Msg"];
            return View(settingsPageData);

           
            //return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(HomePageConfiguration model,int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            HomePageConfiguration pageConfigurationInfo = new HomePageConfiguration();
            if (model.IsPromoEnabled == true)
            {
                pageConfigurationInfo.Promotext = model.Promotext;
                pageConfigurationInfo.IsPromoEnabled = true;

            }
            else
            {
                pageConfigurationInfo.Promotext = null;
                pageConfigurationInfo.IsPromoEnabled = false;

            }
            pageConfigurationInfo.VideoUrl = model.VideoUrl;
            pageConfigurationInfo.TermsAndCondition = model.TermsAndCondition;
            pageConfigurationInfo.IsActive = true;
            pageConfigurationInfo.RewardAmount = model.RewardAmount;
            db.HomePageConfigurations.Add(pageConfigurationInfo);
            db.SaveChanges();
            ViewBag.message = "Success";
            TempData["Msg"] = ViewBag.message;
            return RedirectToAction("Settings");
        }

        public List<ShortCourse> GetHomePageActiveCourses()
        {
            var courses = (from course in db.Courses
                           where course.DisplayOnHomePage == true && (double) course.BasePrice == 0.00 && course.IsActive == true
                           select new ShortCourse
                           {
                               Id = course.Id,
                               CourseImageLink = course.CourseImageLink,
                               Duration = course.Duration,
                               Category = course.CourseCategory,
                               CourseName = course.CourseName,
                               DisplayonHomePage = course.DisplayOnHomePage,
                               ShortDescription = course.ShortDescription,
                               BasePrice = course.BasePrice,
                               Availability = course.CourseAvailabilityId.Name,
                               CreatedOn = course.CreatedOn,
                               CoursePriority = course.CoursePriority,
                               EDXCourseLink = course.EDXCourseLink,
                               CourseRatings = course.CourseRatings
                           }).Take(8)
                           .Union
                           ((from course in db.Courses
                            where course.DisplayOnHomePage == true && (double)course.BasePrice != 0.00 && course.IsActive == true
                            select new ShortCourse
                            {
                                Id = course.Id,
                                CourseImageLink = course.CourseImageLink,
                                Duration = course.Duration,
                                Category = course.CourseCategory,
                                //CategoryName = course.CategoryName,
                                CourseName = course.CourseName,
                                DisplayonHomePage = course.DisplayOnHomePage,
                                ShortDescription = course.ShortDescription,
                                BasePrice = course.BasePrice,
                                Availability = course.CourseAvailabilityId.Name,
                                CreatedOn = course.CreatedOn,
                                CoursePriority = course.CoursePriority,
                                EDXCourseLink = course.EDXCourseLink,
                                CourseRatings = course.CourseRatings
                            }).Take(8)
                            ).OrderByDescending(x => x.CreatedOn).ToList();
            return courses;
        }

        public List<ShortCourse> GetAllActiveCourses()
        {
            var courses = (from course in db.Courses
                           where course.IsActive == true
                           select new ShortCourse
                           {
                               Id = course.Id,
                               CourseImageLink = course.CourseImageLink,
                               Duration = course.Duration,
                               DisplayonHomePage = course.DisplayOnHomePage,
                               Category = course.CourseCategory,
                               CourseName = course.CourseName,
                               ShortDescription = course.ShortDescription,
                               BasePrice = course.BasePrice,
                               AvailabilityId = course.CourseAvailabilityId.Id,
                               Availability = course.CourseAvailabilityId.Name,
                               CourseLanguage = course.CourseLanguageId.Id,
                               CourseLevel = course.CourseLevel.Id,
                               CourseType = course.CourseTypesId.Id,
                               CreatedOn = course.CreatedOn,
                               CoursePriority = course.CoursePriority,
                               EDXCourseLink = course.EDXCourseLink,
                               CourseRatings = course.CourseRatings,
                               //Below values are default
                               IsCourseEnrolled = false,
                               EmailCourseName = "Hello, I have came across the online course " + course.CourseName.Trim() + " , from Millionlights. This course might interest you too. Please have a look on Millionlights.org"
                           }).ToList();
            return courses;
        }

        private string DirectCourseURL( string cName)
        {
            string cc = System.Web.HttpContext.Current.Request.Url.Host + "/Course/AboutCourse/" + System.Uri.EscapeDataString(cName.Trim()) + "/";
            if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
            {
                cc = "http://" + cc;
            }
            else
            {
                cc = "https://" + cc;
            }
            return cc;
        }

        public List<PartnerImage> GetActivePartnerImages()
        {
            var partners = (from partner in db.Partners
                            where partner.IsActive == true && partner.DisplayOnHomePage == true
                           select new PartnerImage
                           {
                               Id = partner.Id,
                               ImageLink = partner.ImageLink,
                               DisplayOnHomePage = partner.DisplayOnHomePage,
                           }).ToList();

            return partners;
        }
        [ActionName("massive-open-online-courses-India")]
        public ActionResult About()
        {
            //List<ShortCourse> activeCourses = GetHomePageActiveCourses();
            List<PartnerImage> activePartners = GetActivePartnerImages();
            ViewBag.AllPartner = activePartners;
            //ViewBag.AllCourses = activeCourses;
            return View("About");
        }
        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Users Section
            var users = db.Users.ToList();
            ViewBag.TotalUsersCount = users.Count();
            ViewBag.ActiveUsersCount = users.Where(x => x.IsActive == true).Count();
            ViewBag.InActiveUsersCount = users.Where(x => x.IsActive == false).Count();
            ViewBag.MLUsersCount = users.Where(x => x.UserType == "MillionLight").Count();
            ViewBag.OtherUsersCount = users.Where(x => x.UserType != "MillionLight").Count();
            ViewBag.FBUsersCount = users.Where(x => x.UserType == "Facebook").Count();
            ViewBag.TwitterUsersCount = users.Where(x => x.UserType == "Twitter").Count();
            ViewBag.LinkedInUsersCount = users.Where(x => x.UserType == "LinkedIn").Count();
            ViewBag.GoogleUsersCount = users.Where(x => x.UserType == "Google").Count();
            ViewBag.MicrosoftUsersCount = users.Where(x => x.UserType == "Microsoft").Count();

            //Coupons Section
            var coupons = db.Coupons.ToList();
            ViewBag.TotalCouponsCount = coupons.Count;
            ViewBag.ActivatedCouponsCount = coupons.Where(x => x.StatusId == 1).Count();
            ViewBag.PartiallyActivatedCouponsCount = coupons.Where(x => x.StatusId == 2).Count();
            ViewBag.ExpiredCouponsCount = coupons.Where(x => x.StatusId == 3).Count();
            ViewBag.InActivatedCouponsCount = coupons.Where(x => x.StatusId == 4).Count();
            //Courses Section
            var courses = db.Courses.ToList();
            ViewBag.TotalCoursesCount = courses.Count();
            ViewBag.TotalActiveCoursesCount = courses.Where(x => x.IsActive == true).Count();
            ViewBag.TotalInactiveCoursesCount = courses.Where(x => x.IsActive == false).Count();
            //Orders / Sales Section
            var orders = db.Orders.ToList();
            ViewBag.TotalOrdersCount = orders.Count();
            ViewBag.OrdersCompletedCount = orders.Where(x => x.OrderStatusID == 1).Count();
            ViewBag.OrdersPendingCount = orders.Where(x => x.OrderStatusID == 2).Count();
            ViewBag.OrdersCancelledCount = orders.Where(x => x.OrderStatusID == 6).Count();
            ViewBag.OrdersRefunded = orders.Where(x => x.OrderStatusID == 7).Count();
            decimal totalSales = 0;
            foreach (var order in orders)
            {
                totalSales = totalSales + order.TotalPrice;
            }
            ViewBag.TotalSale = "Rs. " + " " + totalSales;
            return View();
        }
        public ActionResult UserDashboard()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? uid = Convert.ToInt32(Session["UserID"]);

            var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uid).FirstOrDefault();
            UserRegister urg = new UserRegister();
            urg.AddressLine1 = userRegister.b.AddressLine1;
            urg.AddressLine2 = userRegister.b.AddressLine2;
            urg.City = userRegister.b.City;
            urg.Country = userRegister.b.Country;
            urg.EmailId = userRegister.a.EmailId;
            urg.UserId = userRegister.a.UserId;
            urg.UserName = userRegister.a.UserName;
            urg.ZipCode = userRegister.b.ZipCode;
            urg.State = userRegister.b.State;
            urg.LastName = userRegister.b.LastName;
            urg.FirstName = userRegister.b.FirstName;
            urg.FullName = userRegister.b.FirstName + " " + userRegister.b.LastName;
            urg.PhoneNumber = userRegister.b.PhoneNumber;
            urg.Biography = userRegister.b.Biography;
            urg.ImageURL = userRegister.b.ImageURL;

            ViewBag.bio = urg.Biography;

            var userNotification = db.UserNotitifications.Where(X => X.IsRead == false && X.Receiver == uid).OrderByDescending(x => x.Id).ToList();
            ViewBag.Notification = userNotification;

            var homePage = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.Video = homePage;

            var alert = db.UserNotitifications.Where(X => X.IsAlert == true && X.Receiver == uid).OrderByDescending(x => x.Id).ToList();
            ViewBag.Alert = alert;

            return View(urg);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Contacts";
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View();
        }
       
        public ActionResult FAQ()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Contact(ContactUs contact)
        {
           
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult AirtelHowItWorks()
        {
          
            return View();
        }
        public ActionResult SnapdealHowItWorks()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult FlipkartHowItWorks()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Home()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public ActionResult JobApplication()
        {
            var jobTitle = Request.Form["job_title"];
            var firstName = Request.Form["name_contact"];
            var lastName = Request.Form["lastname_contact"];
            var email = Request.Form["email_contact"];
            var phone = Request.Form["phone_contact"];
            var message = Request.Form["message_contact"];
            var resume = Request.Files[0];
            var fileName = Request.Files[0].FileName;
            HttpPostedFileBase hpf = Request.Files["file"];
            string path = null;
            if (Request.Browser.Browser != "InternetExplorer")
            {
                if (hpf.ContentLength > 0)
                {
                    path = System.Web.HttpContext.Current.Server.MapPath("~/JobApplications/");
                    var directory = new DirectoryInfo(path);
                    if (directory.Exists == false)
                    {
                        directory.Create();
                    }
                    path = path + fileName;
                    hpf.SaveAs(path);
                }
            }
            else
            {
                path = Request.Files[0].FileName;
            }
            //Send email with attachment
            //string filePath = System.Web.HttpContext.Current.Server.MapPath(resume);
            string cerTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "JobApplication.html");
            string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
            string toEmail = senderEmail;
            MillionLightsEmails mEmail = new MillionLightsEmails();
            mEmail.SendJobApplicationEmail(
                ConfigurationManager.AppSettings["SenderName"],
                ConfigurationManager.AppSettings["SenderEmail"],
                 ConfigurationManager.AppSettings["Telephone"],
                  ConfigurationManager.AppSettings["EmailId"],
                "Job Application",
                cerTemplate,
                toEmail,
                path,
                fileName,
                firstName + " " + lastName,
                email,
                phone,
                message,
                jobTitle
                );
            TempData["Success"] = true;
            return RedirectToAction("Career", "Career");
        }
        public ActionResult UserFeedback()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.IsSuccess = TempData["Success"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult UserFeedbackEmail()
        {
            var name = Request.Form["name_feedback"];
            var email = Request.Form["email_feedback"];
            var message = Request.Form["message_feedback"];
            List<string> recipients = email.Split(';').Select(x => Convert.ToString(x)).ToList();
            try
            {
                if(name=="" || name==null || name=="null")
                {
                    name = "User";
                }
                //Send Feeback Email Message To User & ML Support Team
                string subject = "Thank you for submitting the feedback to Millionlights, we'll get back to you soon!";
                string txtMessage = "Dear " + name + "," + "<br/> " + " We have submitted your feedback and will get back to you soon, Thank you!" + "<br/> " + "Warm Regards," + "<br/> " + "MillionLights Team" + "<br/> " + "Telephone:+91 9890122592" + "<br/> " + "Email:support@millionlights.org";
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string senderName = ConfigurationManager.AppSettings["SenderName"];
                MandrillEmailService.EmailService.SendFormattedEmail(senderEmail, senderName, subject, recipients, txtMessage);
                
                //Email to Support team
                string supportSubject = "Feedback message from -" + " " + name;
                string supportMessage = "Name:" + name + "<br/> " + "Email:" + email + "<br/> " + "Message:" + message;
                MandrillEmailService.EmailService.SendIndividualEmail(senderEmail, name, supportSubject, senderEmail, supportMessage);
                TempData["Success"] = true;
            }
            catch (Exception)
            {
                TempData["Success"] = false;
            }

            return RedirectToAction("UserFeedback");
        }
    }

}