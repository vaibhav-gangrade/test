using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Millionlights.Models;
using System.IO;
using System.Net;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using System.Dynamic;
using Newtonsoft.Json;
using PagedList;
using System.Configuration;
using Millionlights.EmailService;
using System.Data.Entity;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Web.Routing;
//using EdxIntegration;


namespace Millionlights.Controllers
{
    public class UserRegisterController : Controller
    {
        
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /UserRegister/
       
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult SampleView()
        {
            return View();
        }
        public ActionResult UserRegister()
        {
            return View();
        }

        public new ActionResult Profile(int? id, int? page)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Certificate Evidence Success
            if (TempData["IsSuccess"] != null)
            {
                ViewBag.IsSuccess = TempData["IsSuccess"];
            }
            else
            {
                ViewBag.IsSuccess = false;
            }
            
            //
            var userDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == id).ToList();
            //id = 1;
            UserRegister urg = new UserRegister();
            if (id != null)
            {
                urg.AddressLine1 = userDetails[0].b.AddressLine1;
                urg.AddressLine2 = userDetails[0].b.AddressLine2;
                urg.City = userDetails[0].b.City;
                urg.Country = userDetails[0].b.Country;
                urg.EmailId = userDetails[0].a.EmailId;
                urg.UserId = userDetails[0].a.UserId;
                urg.UserName = userDetails[0].a.UserName;
                if (userDetails[0].b.ZipCode != string.Empty)
                {
                    urg.ZipCode = userDetails[0].b.ZipCode;
                }
                urg.State = userDetails[0].b.State;
                urg.LastName = userDetails[0].b.LastName;
                urg.FirstName = userDetails[0].b.FirstName;
                urg.FullName = userDetails[0].b.FirstName + " " + userDetails[0].b.LastName;
                urg.PhoneNumber = userDetails[0].b.PhoneNumber;
                urg.Biography = userDetails[0].b.Biography;
                urg.ImageURL = userDetails[0].b.ImageURL;
                ViewBag.bio = urg.Biography;
            }

            //my courses
            List<UsersCourses> userCourseList = db.UsersCourses.Where(x => x.UserId == id).ToList();
            List<Course> courseListByUserId = new List<Course>();
            foreach (var course in userCourseList)
            {
                var courseTemp = db.Courses.Where(x => x.Id == course.CourseID).FirstOrDefault();
                courseListByUserId.Add(courseTemp);
            }

            ViewBag.CourseByCategories = courseListByUserId;
            ViewBag.urg = urg;

            //Calculate and Bind Total Amounts in Wallet
            var walletAmount = db.UserWallets.Where(x => x.UserId == id && x.IsActive == true).FirstOrDefault();
            if (walletAmount != null)
            {
                ViewBag.WalletAmount = walletAmount.FinalAmountInWallet.ToString();
            }
            var refCodes = db.ReferralCodes.Where(x => x.Referrer == id && x.IsActive == true).FirstOrDefault();
            if (refCodes != null)
            {
                ViewBag.ReferralCodeToShare = refCodes.ReferralCode;
            }
            var roleID = int.Parse(Session["RoleID"].ToString());
            if (roleID == 1 || roleID == 3)
            {
                return View("AdminProfile", urg);
            }
            return View(urg);
        }

        public ActionResult GetUserCourses(string data)
        {
            int userId = int.Parse(Session["UserID"].ToString());
            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            if (data != null)
            {
                var courseId = Convert.ToInt32(data);
                var userCourses = db.UsersCourses.Where(X => X.CourseID == courseId && X.UserId == userId).FirstOrDefault();
                if (userCourses != null)
                {
                    dp.couresId = data;
                    dp.enrollCourse = "true";
                }
                else
                {

                    dp.enrollCourse = "false";
                }

                dp = JsonConvert.SerializeObject(dp);
                userStatus.Add(dp);

            }
            return Json(userStatus);
        }
        public ActionResult CheckEmailNPhoneExist(string emailid, string phone)
        {
            var userDetailsCheck = (dynamic)null;
            if (Session["UserId"] != null)
            {
                int loggedInUserId = int.Parse(Session["UserId"].ToString());
                userDetailsCheck = db.Users.Join(db.UsersDetails.Join(db.UserInRoles, ud => ud.UserId, ur => ur.UserId, (ud, ur) => new { ud, ur }), u => u.UserId, udx => udx.ud.UserId, (u, udx) => new { u, udx })
                                    .Where(x => x.u.UserId == loggedInUserId && x.u.IsActive == true && x.udx.ur.RoleId == 2).FirstOrDefault();
            }

            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            var emailExistCount = 0;
            var phoneExistCount = 0;
            if (userDetailsCheck != null)
            {
                /////
                Int64? phoneNumber = null;
                if (phone != null && phone != "")
                {
                    phoneNumber = Int64.Parse(phone);
                }
                ////
                if (emailid != null && userDetailsCheck.u.EmailId != emailid)
                {
                    emailExistCount = db.Users.Where(x => x.EmailId == emailid).Count();
                }

                if (phoneNumber != null && userDetailsCheck.udx.ud.PhoneNumber != phoneNumber)
                {
                    //Int64? phoneNumber = Int64.Parse(phone);
                    phoneExistCount = db.UsersDetails.Where(x => x.PhoneNumber == phoneNumber).Count();
                }
                if (emailExistCount > 0)
                {
                    dp.EmailExist = true;
                }
                else
                {
                    dp.EmailExist = false;
                }
                if (phoneExistCount > 0)
                {
                    dp.PhoneExist = true;
                }
                else
                {
                    dp.PhoneExist = false;
                }
            }
            else
            {
                if (emailid != null)
                {
                    emailExistCount = db.Users.Where(x => x.EmailId == emailid).Count();
                }
                //var phoneExistCount = 0;
                if (phone != null && phone != "")
                {
                    Int64? phoneNumber = Int64.Parse(phone);
                    phoneExistCount = db.UsersDetails.Where(x => x.PhoneNumber == phoneNumber).Count();
                }
                if (emailExistCount > 0)
                {
                    dp.EmailExist = true;
                }
                else
                {
                    dp.EmailExist = false;
                }
                if (phoneExistCount > 0)
                {
                    dp.PhoneExist = true;
                }
                else
                {
                    dp.PhoneExist = false;
                }
            }

            dp = JsonConvert.SerializeObject(dp);
            userStatus.Add(dp);
            return Json(userStatus);
        }
        public ActionResult GetUserCoursesByCourseIds(string data)
        {
            int userId = int.Parse(Session["UserID"].ToString());
            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            if (data != null)
            {
                string[] ids;
                ids = data.Split(',');
                int count = 0;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] != "")
                    {
                        var courseId = Convert.ToInt32(ids[i]);
                        var userCourses = db.UsersCourses.Where(X => X.CourseID == courseId && X.UserId == userId).FirstOrDefault();
                        if (userCourses != null)
                        {
                            dp.couresId = data;
                            dp.enrollCourse = "true";
                            count = count + 1;
                        }
                        else
                        {

                            dp.enrollCourse = "false";
                        }
                    }
                }
                dp.Count = count;
                dp = JsonConvert.SerializeObject(dp);
                userStatus.Add(dp);

            }
            return Json(userStatus);
        }
        public ActionResult OrderTab(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Orders> OrderList = new List<Orders>();
            var OrderDetails = db.Courses.Join(db.ItemsOrders.Join(db.Orders.Join(db.OrderStatus, IO => IO.OrderStatusID, OS => OS.OrderStatusID, (IO, OS) => new { IO, OS }), IOI => IOI.OrderID, O => O.IO.OrderID, (IOI, O) => new { IOI, O }), U => U.Id, OIO => OIO.IOI.CourseId, (OIO, U) => new { OIO, U }).Where(X => X.U.O.IO.UserID == id).ToList();  //Where(X => X.U.O.IO.IsActive == true).ToList();

            List<OrderManagement> order = new List<OrderManagement>();
            var orderNo = string.Empty;
            foreach (var orders in OrderDetails)
            {
                OrderManagement orderList = new OrderManagement();
                string coursename = string.Empty;
                if (orderNo != orders.U.O.IO.OrderNumber)
                {
                    //if (orders.U.O.IO.UserID == id)
                    //{
                    orderList.OrderID = orders.U.O.IO.OrderID;
                    orderList.TotalItems = orders.U.O.IO.TotalItems;
                    orderList.CourseId = orders.U.IOI.CourseId;
                    orderList.OrderNumber = orders.U.O.IO.OrderNumber;
                    orderList.TotalPrice = orders.U.O.IO.TotalPrice;
                    orderList.OrderStatus = orders.U.O.OS.Status;
                    orderList.OrderedDatetime = orders.U.O.IO.OrderedDatetime;
                    orderNo = orders.U.O.IO.OrderNumber;

                    foreach (var orderName in OrderDetails)
                    {
                        if (orderName.U.O.IO.OrderID == orders.U.O.IO.OrderID)
                        {
                            Course course = db.Courses.Where(x => x.Id == orderName.U.IOI.CourseId).FirstOrDefault();
                            if (course != null)
                            {
                                if (!string.IsNullOrEmpty(course.CourseName))
                                {
                                    if (!string.IsNullOrEmpty(coursename))
                                    {
                                        coursename = course.CourseName + ',' + " " + coursename;
                                    }
                                    else
                                    {
                                        coursename = course.CourseName;
                                    }
                                }
                            }

                        }
                        orderList.CourseName = coursename;
                    }
                    orderList.IsActive = orders.U.O.IO.IsActive;
                    order.Add(orderList);
                    //}
                }
            }
            IPagedList<OrderManagement> orderDetails = order.ToPagedList(pageIndex, pageSize);

            return PartialView("MyOrder", orderDetails);
        }
        public ActionResult CertificateTab(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<UsersCertificate> CertDetails = db.UsersCertificate.Where(x => x.UserID == id).ToList();

            IPagedList<UsersCertificate> certDetails = CertDetails.ToPagedList(pageIndex, pageSize);

            if (TempData["IsSuccess"] != null)
            {
                ViewBag.IsSuccess = TempData["IsSuccess"];
            }
            else
            {
                ViewBag.IsSuccess = false;
            }
            return PartialView("CertificateTab", certDetails);
        }
        public ActionResult RefCodesTab(int? page, int? id, int? OperationId)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string EmailId = Session["UserEmailId"].ToString();
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ReferralCodes> referralCodes = null;
            IPagedList<ReferralCodes> refCodes = null;
            if (OperationId != null && OperationId == 1)
            {
                referralCodes = db.ReferralCodes.Where(x => x.Referrer == id).ToList();
                refCodes = referralCodes.ToPagedList(pageIndex, pageSize);
            }
            if (OperationId != null && OperationId == 2)
            {
                referralCodes = db.ReferralCodes.Where(x => x.Receiver == id).ToList();
                if (referralCodes.Count() == 0)
                {
                    if (EmailId != null)
                    {
                        referralCodes = db.ReferralCodes.Where(x => x.ReceiverEmail == EmailId).ToList();
                    }
                }

                refCodes = referralCodes.ToPagedList(pageIndex, pageSize);
            }
            if (OperationId == null)
            {
                referralCodes = null;
                refCodes = null;
            }

            return PartialView("RefCodes", refCodes);
        }
        public ActionResult CoupanTab(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int skipRecord = (pageIndex * 10) - 10;
            IPagedList<CoupanCourseDetails> coupanDetails = null;
            List<CoupanCourseDetails> CouCorDetails = new List<CoupanCourseDetails>();
            List<CouponCourses> courseDetails = new List<CouponCourses>();
            var allUniqueUserCoupons = db.Coupons.GroupJoin(
                db.UserCoupons,
                coupon => coupon.Id,
                userCoupon => userCoupon.CouponId,
                (x, y) => new { Coupon = x, UserCoupon = y })
                .SelectMany(
                x => x.UserCoupon.DefaultIfEmpty(),
                (x, y) => new { Coupon = x.Coupon, UserCoupon = y }).Where(z => z.Coupon.UserId == id || (z.UserCoupon.ActivateBy == id && z.UserCoupon.ActivateBy != z.Coupon.UserId)).ToList();

            foreach (var item in allUniqueUserCoupons)
            {
                DateTime? ActivatedOn = null;
                string RedeemStatus = null;
                int? couponId = null;
                if (item.UserCoupon != null)
                {
                    couponId = item.UserCoupon.CouponId;
                    RedeemStatus = item.UserCoupon.RedeemStatus;
                    ActivatedOn = item.UserCoupon.ActivateOn;

                }
                // var couponCourseCount = db.CouponCourses.Where(x => x.CouponId == item.Coupon.Id).Count();
                CoupanCourseDetails CCD = new CoupanCourseDetails();
                CCD.CoupanId = item.Coupon.Id;
                CCD.CCGId = item.Coupon.CCGId;
                CCD.CouponCode = item.Coupon.CouponCode;
                CCD.PartnerName = item.Coupon.PartnerName;
                CCD.AllowedCourses = item.Coupon.AllowedCourses;
                CCD.BenefitName = item.Coupon.BenefitName;
                CCD.ValidFrom = item.Coupon.ValidFrom;
                CCD.ValidTo = item.Coupon.ValidTo;
                CCD.StatusId = item.Coupon.StatusId;
                CCD.StatusType = item.Coupon.StatusType;
                CCD.BlockedOn = item.Coupon.BlockedOn;
                CCD.BlockedReason = item.Coupon.BlockedReason;
                CCD.CreatedOn = item.Coupon.CreatedOn;
                CCD.IsPrepaid = item.Coupon.IsPrepaid;
                CCD.ActivatedOn = ActivatedOn;
                bool isCouponExpired = DateTime.Now > item.Coupon.ValidTo ? true : false;
                if (isCouponExpired)
                {
                    CCD.CouponStatus = "YES";
                }
                else
                {
                    CCD.CouponStatus = "NO";
                }
                if (item.Coupon.AllowedCourses == 1 && RedeemStatus == "Partial")
                {
                    CCD.CouponRedeemStatus = "Complete";
                }
                else
                {
                    CCD.CouponRedeemStatus = RedeemStatus;
                }
                //CCD.CourseCount = couponCourseCount;
                CouCorDetails.Add(CCD);
                //Get Courses
                var courses = db.Courses.Join(db.CouponCourses, c => c.Id, cc => cc.CourseId, (c, cc) => new { c, cc })
                    .Where(x => x.cc.CouponId == item.Coupon.Id).ToList();
                foreach (var courseItem in courses)
                {
                    CouponCourses course = new CouponCourses();
                    course.CouponId = courseItem.cc.CouponId;
                    course.CourseCode = courseItem.c.CourseCode;
                    course.CName = courseItem.c.CourseName;
                    course.ShortDescription = courseItem.c.ShortDescription;
                    course.BasePrice = courseItem.c.BasePrice;
                    //var courseRedeem = db.UsersCourses.Where(x => x.CourseID == courseItem.cc.CourseId && x.UserId == id && x.CouponApplied == item.Coupon.CouponCode).Count();
                    var courseRedeem = db.UsersCourses.Where(x => x.CourseID == courseItem.cc.CourseId && x.UserId == id).Count();
                    if (couponId != null && courseRedeem > 0)
                    {
                        course.IsCourseRedeemed = "Yes";
                    }
                    else
                    {
                        course.IsCourseRedeemed = "No";

                    }
                    courseDetails.Add(course);
                }

            }
            coupanDetails = CouCorDetails.ToPagedList(pageIndex, pageSize);
            ViewBag.CourseDetailsView = courseDetails;
            IPagedList<CoupanCourseDetails> coupan = CouCorDetails.ToPagedList(pageIndex, pageSize);
            return PartialView("MyCoupan", coupan);
        }
        public ActionResult CurrentCourseTab(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<UsersCourses> userCourseList = db.UsersCourses.Where(x => x.UserId == id).ToList();
            List<CurrentCourse> currentcourseListByUserId = new List<CurrentCourse>();
            foreach (var course in userCourseList)
            {
                var currentcourse = db.Courses.Where(x => x.Id == course.CourseID && x.CourseAvailability == 4).FirstOrDefault();
                CurrentCourse CC = new CurrentCourse();
                if (currentcourse != null)
                {
                    CC.CourseName = currentcourse.CourseName;
                    CC.ShortDescription = currentcourse.ShortDescription;
                    CC.StartDate = currentcourse.StartDate;
                    CC.EndDate = currentcourse.EndDate;
                    CC.Duration = currentcourse.Duration;
                    CC.Hours = currentcourse.Hours;
                    currentcourseListByUserId.Add(CC);
                }
            }
            IPagedList<CurrentCourse> CourseInfo = currentcourseListByUserId.ToPagedList(pageIndex, pageSize);
            ViewBag.CourseByCategories = currentcourseListByUserId;
            return PartialView("CourseTab", CourseInfo);
        }

        public ActionResult AllCourseTab(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<UsersCourses> userCourseList = db.UsersCourses.Where(x => x.UserId == id && x.IsActive == true).OrderByDescending(Y => Y.CourseID).ToList();

            List<Course> courseListByUserId = new List<Course>();
            foreach (var course in userCourseList)
            {
                var Availcourse = db.Courses.Where(x => x.Id == course.CourseID).FirstOrDefault();
                courseListByUserId.Add(Availcourse);
            }
            IPagedList<Course> CourseInfo = courseListByUserId.GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToPagedList(pageIndex, pageSize);
            ViewBag.CourseByCategories = courseListByUserId;
            return PartialView("AllCourseTab", CourseInfo);
        }
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult UserRegister(UserRegister userRegister)
        {
            List<dynamic> userStatus = new List<dynamic>();
            if (User.Identity.IsAuthenticated)
            {
                return Json(true);
            }

            AccountController account = new AccountController();
            User user = new User();
            var userReg = userRegister;
            if (ModelState.IsValid)
            {
                dynamic dp = new ExpandoObject();
                user.EmailId = userReg.EmailId;
                user.UserName = userReg.UserName;
                user.Password = passwordEncrypt(userReg.Password);
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
                Session["UserID"] = userId;
                dp.userId = userId;
                UserInRole usrInRole = new UserInRole();
                usrInRole.UserId = userId;
                usrInRole.RoleId = 2;
                db.UserInRoles.Add(usrInRole);
                db.SaveChanges();

                UserDetails userDetails = new UserDetails();
                userDetails.UserId = userId;
                userDetails.AddressLine1 = userReg.AddressLine1;
                userDetails.AddressLine2 = userReg.AddressLine2;
                userDetails.City = userReg.City;
                userDetails.State = null;
                userDetails.Country = null;
                userDetails.ZipCode = userReg.ZipCode;
                userDetails.FirstName = userReg.FirstName;
                userDetails.LastName = userReg.LastName;
                userDetails.PhoneNumber = userReg.PhoneNumber;
                userDetails.IsActive = true;
                string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                userDetails.PartnerId = int.Parse(partId);
                userDetails.RegisteredDatetime = DateTime.Now;
                db.UsersDetails.Add(userDetails);
                db.SaveChanges();
                Session["UserEmailId"] = user.EmailId;
                Session["UserMobile"] = userDetails.PhoneNumber;
                //Login
                //AuthenticationManager.SignInOut();
                AuthenticationManager.SignIn(
                    new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(15)
                    },
                    new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, user.EmailId) }, "External"));

                //var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {

                    dp.isTmpUser = "false";
                    UserDetails uesrdetails = db.UsersDetails.SingleOrDefault(u => u.UserId == userId && u.IsActive == true);
                    usrInRole = db.UserInRoles.SingleOrDefault(q => q.UserId == userId);
                    Session["UserName"] = uesrdetails.FirstName;
                    Session["UserID"] = userId;
                    Session["UserEmailId"] = user.EmailId;
                    Session["UserDetails"] = uesrdetails;
                    Session["ImagePath"] = uesrdetails.ImageURL;
                    if (userRegister.data != null)
                    {
                        var courseId = Convert.ToInt32(userRegister.data);
                        var userCourses = db.UsersCourses.Where(X => X.CourseID == courseId && X.UserId == user.UserId).SingleOrDefault();
                        if (userCourses != null)
                        {
                            dp.couresId = userRegister.data;
                            dp.enrollCourse = "true";
                        }
                        else
                        {

                            dp.enrollCourse = "false";
                        }

                    }
                    if (usrInRole != null)
                    {
                        Session["RoleID"] = usrInRole.RoleId;
                    }
                    //bool isPasswordMatch = Regex.Match(user.Password, userReg.Password, RegexOptions.None).Success;
                    bool isPasswordMatch = Regex.Match(passwordDecrypt(user.Password), userReg.Password, RegexOptions.None).Success;
                    if (!isPasswordMatch)
                    {
                        dp.isLogin = "false";

                    }
                    else
                    {
                        dp.isLogin = "true";
                    }
                    dp.roleID = usrInRole.RoleId;
                }
                //New Code User Wallet
                string refCode = userRegister.RefCode;
                if (refCode != null && refCode != "" && refCode.Length == 8)
                {
                    var settings = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
                    decimal? rewardAmount = decimal.Parse(settings.RewardAmount);
                    var refCodes = db.ReferralCodes.Where(x => x.ReferralCode == refCode && x.IsActive == true).ToList();
                    var emailReceiver = refCodes.Where(x => x.ReceiverEmail == userRegister.EmailId || x.ReceiverPhoneNumber == userRegister.PhoneNumber.ToString()).FirstOrDefault();
                    if (emailReceiver != null)
                    {
                        emailReceiver.Receiver = user.UserId;
                        db.Entry(emailReceiver).State = EntityState.Modified;
                        db.SaveChanges();

                        UserWallets uw = db.UserWallets.Where(x => x.UserId == user.UserId).FirstOrDefault();
                        if (uw != null)
                        {
                            decimal? amount = uw.FinalAmountInWallet;
                            uw.FinalAmountInWallet = amount + rewardAmount;
                            db.Entry(uw).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            UserWallets wallet = new UserWallets();
                            wallet.UserId = user.UserId;
                            wallet.FinalAmountInWallet = rewardAmount;
                            wallet.IsActive = true;
                            db.UserWallets.Add(wallet);
                            db.SaveChanges();
                        }
                    }
                }

                //New Referral Code for User Registrations
                CouponGen cc = new CouponGen();
                ReferralCodes refC = new ReferralCodes();
                refC.ReferralCode = cc.GenerateReferralCodes();
                refC.Referrer = user.UserId;
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
                db.ReferralCodes.Add(refC);
                db.SaveChanges();
                //
                //


                //Send Email
                string userName = userReg.FirstName + " " + userReg.LastName;
                string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "StudentRegistrationEmailTemplate.htm");
                int? NotificationId = null;
                if (user.EmailId != null)
                {
                    MillionLightsEmails mEmail = new MillionLightsEmails();
                    mEmail.SendStudentRegistrationCompleteEmail(
                        ConfigurationManager.AppSettings["SenderName"],
                        ConfigurationManager.AppSettings["SenderEmail"],
                         ConfigurationManager.AppSettings["Telephone"],
                          ConfigurationManager.AppSettings["EmailId"],
                        "Your Millionlights Registration Successful",
                        regTemplate,
                        userName,
                        user.EmailId
                        );
                    //email notification
                    UserNotitification userNotification = new UserNotitification();
                    userNotification.Receiver = userId;
                    userNotification.Sender = "System";
                    userNotification.Subject = Millionlights.Models.Constants.StudentRegSubNotification;
                    userNotification.Message = String.Format(Millionlights.Models.Constants.StudentRegMsgNotification, user.EmailId);
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
                    //Send SMS
                    try
                    {
                        Int64? numberToSend = userReg.PhoneNumber;
                        string msg = String.Format(Millionlights.Models.Constants.StudentRegistrationSuccessMsg, user.EmailId);
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
                    catch (Exception)
                    {
                    }
                }
                dp = JsonConvert.SerializeObject(dp);
                userStatus.Add(dp);
            }
            return Json(userStatus);
        }

        [HttpPost]
        public JsonResult Login(string userName, string password, string data)
        {
            List<dynamic> userStatus = new List<dynamic>();
            if (ModelState.IsValid)
            {
                dynamic dp = new ExpandoObject();
                User userExist = db.Users.SingleOrDefault(p => p.EmailId == userName && p.IsActive == true);
                if (userExist != null)
                {
                    dp.userExist = "true";
                    UserInRole usrInRole = null;
                    if (passwordDecrypt(userExist.Password) == password)
                    {
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(
                            new AuthenticationProperties
                            {
                                AllowRefresh = true,
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddDays(15)
                            },
                            new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) }, "External"));

                        Session["UserType"] = userExist.UserType;
                        dp.isTmpUser = "false";
                        UserDetails uesrdetails = db.UsersDetails.SingleOrDefault(u => u.UserId == userExist.UserId && u.IsActive == true);
                        usrInRole = db.UserInRoles.SingleOrDefault(q => q.UserId == userExist.UserId);
                        Session["UserName"] = uesrdetails.FirstName;
                        Session["UserID"] = userExist.UserId;
                        Session["UserEmailId"] = userExist.EmailId;
                        Session["UserDetails"] = uesrdetails;
                        Session["ImagePath"] = uesrdetails.ImageURL;
                        dp.userId = userExist.UserId;

                        if (data != null)
                        {
                            var courseId = Convert.ToInt32(data);
                            var userCourses = db.UsersCourses.Where(X => X.CourseID == courseId && X.UserId == userExist.UserId).FirstOrDefault();
                            if (userCourses != null)
                            {
                                dp.couresId = data;
                                dp.enrollCourse = "true";
                            }
                            else
                            {

                                dp.enrollCourse = "false";
                            }

                        }
                        if (usrInRole != null)
                        {
                            Session["RoleID"] = usrInRole.RoleId;
                        }
                        bool isPasswordMatch = Regex.Match(passwordDecrypt(userExist.Password), password, RegexOptions.None).Success;
                        if (!isPasswordMatch)
                        {
                            dp.isLogin = "false";

                        }
                        else
                        {
                            dp.isLogin = "true";
                            //SetCookie(1);
                        }
                        dp.roleID = usrInRole.RoleId;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                        dp.isLogin = "false";
                    }
                }
                else
                {
                    dp.userExist = "false";
                    TmpUser tmpUser = db.TmpUsers.SingleOrDefault(p => p.EmailId == userName);
                    if (tmpUser != null)
                    {
                        dp.isTmpUser = "true";
                        Session["UserName"] = tmpUser.FirsrName;
                        Session["UserID"] = tmpUser.TmpId;
                        dp.tmpEmailId = tmpUser.EmailId;
                        dp.userId = tmpUser.TmpId;
                        Session["RoleID"] = 2;
                        dp.roleID = 2;
                        dp.TmpLogin = "true";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                        dp.isLogin = "false";

                    }
                }

                dp = JsonConvert.SerializeObject(dp);
                userStatus.Add(dp);

            }
            return Json(userStatus);
        }

        [HttpPost]
        public JsonResult ForgotPwd(string userId)
        {
            var userDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(x => x.a.EmailId == userId && x.a.IsActive == true).ToList();
            var isUser = "false";

            if (userDetails.Count != 0)
            {
                if (userDetails[0].a.Password != null)
                {
                    isUser = "true";
                    string recipientsEmail = userDetails[0].a.EmailId;
                    string userPwd = userDetails[0].a.Password;
                    string fName = userDetails[0].b.FirstName;
                    string lName = userDetails[0].b.LastName;
                    var userName = fName + " " + lName;

                    int? NotificationId = null;

                    if (recipientsEmail != null)
                    {
                        PasswordResetRequest resetReq = new PasswordResetRequest();
                        resetReq.UserID = userDetails[0].a.UserId;
                        resetReq.VerificationId = Guid.NewGuid().ToString();
                        resetReq.IsPasswordReset = false;
                        resetReq.PasswordResetRequestDateTime = DateTime.Now;
                        resetReq.PasswordResetDateTime = null;
                        db.PasswordResetRequest.Add(resetReq);
                        db.SaveChanges();


                        string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ResetPassword.html");

                        MillionLightsEmails mEmail = new MillionLightsEmails();

                        mEmail.SendResetPasswordEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                            ConfigurationManager.AppSettings["Telephone"],
                            ConfigurationManager.AppSettings["EmailId"],
                            "Reset your Millionlights.org password",
                            regTemplate,
                            userName,
                            recipientsEmail,
                            resetReq.VerificationId);
                        //email  notification
                        UserNotitification userNotification = new UserNotitification();
                        userNotification.Receiver = userDetails[0].a.UserId;
                        userNotification.Sender = "System";
                        userNotification.Subject = Millionlights.Models.Constants.ForgotPwdSubNotification;
                        userNotification.Message = Millionlights.Models.Constants.ForgotPwdMsgNotification;
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
                    if (userDetails[0].b.PhoneNumber != null)
                    {
                        //Send SMS
                        try
                        {
                            Int64? numberToSend = userDetails[0].b.PhoneNumber;
                            //string msg = "Your password has been changed successfully. Your LoginId is " + userDetails[0].a.EmailId + " and password is " + userDetails[0].a.Password;
                            string msg = String.Format(Millionlights.Models.Constants.ForgotPasswordTextMsg, userDetails[0].a.EmailId);
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
                                //  sms notification
                                //int NotificationId = userNotification.Id;
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
                }
                else
                {
                    isUser = "externalUser";
                }
            }
            return Json(isUser);
        }
        [HttpPost]
        public JsonResult ResetPassword(string oldPwd, string newPwd)
        {

            int userId = int.Parse(Session["UserID"].ToString());
            //var isUserPwd = db.Users.Where(x => x.UserId == userId && x.Password == oldPwd).FirstOrDefault();
            var isUserPwd = db.Users.Where(x => x.UserId == userId).FirstOrDefault();
            var isPass = false;
            if (isUserPwd != null)
            {
                if (passwordDecrypt(isUserPwd.Password) == oldPwd)
                {
                    User userInfo = db.Users.Find(userId);
                    userInfo.Password = passwordEncrypt(newPwd);
                    db.Entry(userInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    isPass = true;

                    //insert to notification
                    UserNotitification userNotification = new UserNotitification();
                    userNotification.Receiver = userId;
                    userNotification.Sender = "System";
                    userNotification.Subject = Millionlights.Models.Constants.ResetPwdSubNotification;
                    userNotification.Message = Millionlights.Models.Constants.ResetPwdMsgNotification;
                    userNotification.IsAlert = false;
                    userNotification.DateSent = DateTime.Now;
                    userNotification.ReadDate = null;
                    userNotification.SMSDate = null;
                    userNotification.MailDate = DateTime.Now;
                    db.UserNotitifications.Add(userNotification);
                    db.SaveChanges();

                    UserDetails userDetails = db.UsersDetails.Where(u => u.UserId == userId).FirstOrDefault();

                    if (userInfo.EmailId != null)
                    {
                        string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "PasswordResetEmail.htm");

                        MillionLightsEmails mEmail = new MillionLightsEmails();

                        mEmail.SendPasswordChangedEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                             ConfigurationManager.AppSettings["Telephone"],
                              ConfigurationManager.AppSettings["EmailId"],
                            "Your password has been changed successfully",
                            regTemplate,
                            userDetails.FirstName + " " + userDetails.LastName,
                            userInfo.EmailId);
                    }

                    if (userDetails.PhoneNumber != null)
                    {
                        //Send SMS
                        try
                        {
                            Int64? numberToSend = userDetails.PhoneNumber;
                            //string msg = "Your password has been changed successfully. Your LoginId is " + userInfo.EmailId + " and password is " + userInfo.Password;
                            string msg = String.Format(Millionlights.Models.Constants.UserPasswordChange, userInfo.EmailId);
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
                                //  sms notification
                                int NotificationId = userNotification.Id;
                                UserNotitification userNotificationForSms = db.UserNotitifications.Find(NotificationId);
                                userNotificationForSms.SMSDate = DateTime.Now;
                                userNotificationForSms.NotificationStatusId = 2;
                                db.Entry(userNotificationForSms).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return Json(isPass);
        }
        public ActionResult LoginOut()
        {
            //TempData["UserName"] = null;
            Session.Abandon();

            //WebSecurity.Logout();   
            // Invalidate the Cache on the Client Side
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Request.Cookies.Remove(cookie);
            }
            foreach (var cookie in Response.Cookies.AllKeys)
            {
                Response.Cookies.Remove(cookie);
            }
            // window.localStorage.clear();
            AuthenticationManager.SignOut();
            AuthenticationManager.SignOut("External");
            ClearCoupon();
            TempData["IsLogout"] = "true";
            return RedirectToAction("Index", "Home", new { logout = TempData["IsLogout"] });
        }
        public void ClearCoupon()
        {
            TempData["CouponCode"] = null;
            if (HttpContext.Request.Cookies["CouponCode"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                cookie.Expires = DateTime.Now.AddDays(-10);
                HttpContext.Response.Cookies.Remove("CouponCode");
                cookie.Value = null;
                HttpContext.Response.SetCookie(cookie);
            }
        }
        [HttpPost]
        public ActionResult LoginOut(UserRegister userReg)
        {

            return RedirectToAction("Index", "Home");
        }
        public ActionResult SaveProfile(UserRegister reg)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                UserDetails userDetails = db.UsersDetails.Where(x => x.UserId == reg.UserId).FirstOrDefault();
                if (reg.AddressLine1 != null)
                {
                    userDetails.AddressLine1 = reg.AddressLine1;
                }
                else
                {
                    userDetails.AddressLine1 = null;
                }
                if (reg.AddressLine2 != null)
                {
                    userDetails.AddressLine2 = reg.AddressLine2;
                }
                else
                {
                    userDetails.AddressLine2 = null;
                }
                if (reg.City != null)
                {
                    userDetails.City = reg.City;
                }
                else
                {
                    userDetails.City = null;
                }
                if (reg.State != null)
                {
                    userDetails.State = reg.State;
                }
                else
                {
                    userDetails.State = null;
                }
                if (reg.Country != null)
                {
                    userDetails.Country = reg.Country;
                }
                else
                {
                    userDetails.Country = null;
                }
                if (reg.ZipCode != null)
                {
                    userDetails.ZipCode = reg.ZipCode;
                }
                else
                {
                    userDetails.ZipCode = null;
                }
                //var fullname = reg.FullName;
                //var names = fullname.Split(' ');
                string firstName = reg.FirstName;
                string lastName = reg.LastName;
                userDetails.FirstName = firstName;
                userDetails.LastName = lastName;
                userDetails.PhoneNumber = reg.PhoneNumber;
                userDetails.ImageURL = userDetails.ImageURL;
                db.Entry(userDetails).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                User userReg = db.Users.Where(x => x.UserId == reg.UserId).FirstOrDefault();
                userReg.EmailId = reg.EmailId;
                db.Entry(userReg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Profile", new { id = reg.UserId });

        }
        [HttpPost]
        public JsonResult UploadProfilePic()
        {
            int userId = int.Parse(Session["UserID"].ToString());
            string path = "";
            var file = Request.Files[0];
            if (Request.Files.Count > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Images/UsersImg/"), fileName);
                file.SaveAs(path);
                UserDetails userDetails = db.UsersDetails.Where(x => x.UserId == userId).FirstOrDefault();
                if (userDetails != null)
                {
                    userDetails.ImageURL = fileName;
                }
                db.Entry(userDetails).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return Json(path);
        }
        public ActionResult GetCoupanId(Coupon reg)
        {
            ViewBag.coupanId = reg.Id;
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public JsonResult CheckCoupon(string couponText)
        {
            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            var isUserId = db.Coupons.Where(m => (m.UserId != null) && m.CouponCode == couponText).ToList();
            var iProsId = db.Coupons.Where(m => (m.ProsUserId != null) && m.CouponCode == couponText).ToList();
            var noUser = db.Coupons.Where(m => (m.ProsUserId == null && m.UserId == null) && m.CouponCode == couponText).ToList();
            if (isUserId.Count() > 0)
            {
                dp.IsUserId = true;
            }
            else if (iProsId.Count() > 0)
            {
                dp.IsProsId = true;
            }
            else if (noUser.Count() > 0)
            {
                dp.NoUser = true;
            }
            else
            {
                dp.InvalidCoupon = true;
            }

            dp = JsonConvert.SerializeObject(dp);
            userStatus.Add(dp);
            return Json(userStatus);
        }

        [HttpPost]
        public JsonResult ValidateReferralCode(string referralCodeText)
        {
            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            var isCodeExist = db.ReferralCodes.Where(x =>  x.ReferralCode == referralCodeText && x.IsActive == true).ToList();

            if (isCodeExist.Count() < 0)
            {
                dp.InvalidCoupon = true;
            }
            else
            {
                dp.InvalidCoupon = false;
            }

            dp = JsonConvert.SerializeObject(dp);
            userStatus.Add(dp);
            return Json(userStatus);
        }

        public void ChangePassword()
        {
            var users = db.Users.ToList();
            foreach (var item in users)
            {
                if (item.Password != null && item.Password != "")
                {
                    var encrytpwd = passwordEncrypt(item.Password);
                    var user = users.Where(x => x.UserId == item.UserId).FirstOrDefault();
                    user.Password = encrytpwd;
                    //db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();
                    var decryptedPwd = passwordDecrypt(encrytpwd);
                }
            }
        }
        public string GetLMSCourseId(string EDXCourseLink)
        {
            var message = string.Format("The course with this LMSID is not found. Please check the LMS Course ID.");
            string[] edxLinksArr = EDXCourseLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);
            if (edxLinksArr.Length > 1)
            {
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
        public string passwordEncrypt(string inText)
        {
            string key = "MAKV2SPBNI99212";
            byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    inText = Convert.ToBase64String(mStream.ToArray());
                }
            }
            return inText;
        }
        //Decrypting a string
        public string passwordDecrypt(string cryptTxt)
        {
            if (cryptTxt != null && cryptTxt != "")
            {
                string key = "MAKV2SPBNI99212";
                cryptTxt = cryptTxt.Replace(" ", "+");
                byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
                using (Aes aes = Aes.Create())
                {
                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cStream.Close();
                        }
                        cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                    }
                }
                return cryptTxt;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        public JsonResult DeactivateMyAccount()
        {
            var isPass = false;

            try
            {
                if (Session["UserID"] != null && Session["UserID"].ToString() != "")
                {
                    int userId = int.Parse(Session["UserID"].ToString());
                    var users = db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                    users.IsActive = false;
                    db.Entry(users).State = EntityState.Modified;
                    db.SaveChanges();
                    var userDetails = db.UsersDetails.Where(x => x.UserId == userId).FirstOrDefault();
                    userDetails.IsActive = false;
                    db.Entry(userDetails).State = EntityState.Modified;
                    db.SaveChanges();
                    isPass = true;
                }
            }
            catch (Exception)
            {
                isPass = false;
            }
            return Json(isPass);
        }
        [HttpPost]
        public JsonResult CheckEvidence(string certificateId, string courseId)
        {
            var evidenceFound = false;
            if (certificateId != null && courseId != null)
            {
                int cId = int.Parse(courseId);
                int certId = int.Parse(certificateId);
                var course = db.Courses.Where(x => x.Id == cId).FirstOrDefault();
                var evidences = db.UserCertificateEvidenceDetails.Where(x => x.UsersCertificateId == certId && x.CourseId == cId).FirstOrDefault();
                if (course.EvidenceRequired == true && evidences.IsUploaded == true && evidences.IsActive == true)
                {
                    evidenceFound = true;
                }
                if (course.EvidenceRequired == false)
                {
                    evidenceFound = true;
                }
            }
            return Json(evidenceFound);
        }
        [HttpPost]
        public JsonResult GetEvidenceList()
        {
            List<dynamic> list = new List<dynamic>();
            var evidences = db.CertificateEvidenceLkp.Where(x => x.IsActive == true).ToList();
            foreach (var item in evidences)
            {
                dynamic d = new ExpandoObject();
                d.Id = item.Id;
                d.EvidenceName = item.EvidenceName;
                d = JsonConvert.SerializeObject(d);
                list.Add(d);
            }
            return Json(list);
        }

        public ActionResult SaveEvidenceDetails()
        {
            var profilePic = Request.Files["ImageFile"];
            string profilepath = null;
            if (profilePic != null)
            {
                var profileName = Path.GetFileName(profilePic.FileName);
                var directoryPath = Server.MapPath("~/UserEvidence/UserProfilePic/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                profilepath = Path.Combine(Server.MapPath("~/UserEvidence/UserProfilePic/"), profileName);
                profilePic.SaveAs(profilepath);
            }
            var evidencePic = Request.Files["EvidenceFile"];
            string path = null;
            if (evidencePic != null)
            {
                var fileName = Path.GetFileName(evidencePic.FileName);
                var directoryPath = Server.MapPath("~/UserEvidence/EvidencePic/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                path = Path.Combine(Server.MapPath("~/UserEvidence/EvidencePic/"), fileName);
                evidencePic.SaveAs(path);
            }
            var CourseId = int.Parse(Request.Form["CourseIdHidden"]);
            var UsersCertificateId = int.Parse(Request.Form["CertificateIdHidden"]);
            var evidenceNo = Request.Form["evidenceNo"];
            var issueDate = Request.Form["issueDate"];
            var expiryDate = Request.Form["expiryDate"];

            var evidenceDetails = db.UserCertificateEvidenceDetails.Where(x => x.CourseId == CourseId && x.UsersCertificateId == UsersCertificateId && x.IsUploaded == false && x.IsActive == false).FirstOrDefault();
            evidenceDetails.FirstName = Request.Form["firstName"];
            evidenceDetails.LastName = Request.Form["lastName"];
            evidenceDetails.Address = Request.Form["address"];
            evidenceDetails.EvidenceId = int.Parse(Request.Form["evidenceDDL"]);
            if (evidenceNo == null || evidenceNo == null)
            {
                evidenceDetails.EvidenceNumber = null;
            }
            else
            {
                evidenceDetails.EvidenceNumber = evidenceNo;
            }
            evidenceDetails.ImageUrl = "../UserEvidence/UserProfilePic/" + Path.GetFileName(profilePic.FileName);
            evidenceDetails.EvidenceUploadedPath = "../UserEvidence/EvidencePic/" + Path.GetFileName(evidencePic.FileName);
            if (evidenceNo == null || evidenceNo == "")
            {
                evidenceDetails.EvidenceIssueDate = null;
            }
            else
            {
                evidenceDetails.EvidenceIssueDate = DateTime.Parse(issueDate);
            }
            if (expiryDate == null || expiryDate == "")
            {
                evidenceDetails.EvidenceExpiry = null;
            }
            else
            {
                evidenceDetails.EvidenceExpiry = DateTime.Parse(expiryDate);
            }
            evidenceDetails.IsUploaded = true;
            evidenceDetails.IsActive = true;
            db.Entry(evidenceDetails).State = EntityState.Modified;
            db.SaveChanges();

            TempData["IsSuccess"] = true;

            int userId = int.Parse(Session["UserID"].ToString());
            return RedirectToAction("Profile", new { id = userId });
        }
        public ActionResult CheckReferralCode(string refCode, string UserId, string isLoggedIn, string EmailId, string PhoneNumber)
        {
            List<dynamic> userStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            var settings = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            decimal? rewardAmount = 0;
            if (settings != null)
            {
                if (settings.RewardAmount != null)
                {
                    rewardAmount = decimal.Parse(settings.RewardAmount); 
                }
                else
                {
                    rewardAmount = 0;
                }
            }

            int? uId = null;
            if (refCode != null && refCode != "")
            {
                if (UserId != null && UserId != "")
                {
                    uId = int.Parse(UserId);
                    int? recevrId = uId;
                    if (Session["UserID"] == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    var refCodes = db.ReferralCodes.Where(x => x.ReferralCode.ToLower() == refCode.ToLower() && x.IsActive == true).ToList();

                    if (refCodes.Count > 0)
                    {
                        dp.RefCodeFound = true;
                        var isReferee = refCodes.Where(x => x.Referrer == uId).ToList();
                        var isCodeShared = refCodes.Where(x => x.SharedOn == null && x.ReceiverEmail == null && x.ReceiverPhoneNumber == null).ToList();
                        var refCodesExpired = refCodes.Where(x => x.SharedOn != null && x.Receiver == uId && (DateTime.Now >= x.CodeValidity)).ToList();
                        if (refCodesExpired.Count > 0)
                        {
                            dp.CodeExpires = true;
                        }
                        else if (isReferee.Count > 0)
                        {
                            dp.CodeExpires = false;
                            dp.CodeShared = true;
                            dp.AllowReferralCode = false;
                        }
                        else if (isCodeShared.Count != 0)
                        {
                            dp.CodeExpires = false;
                            dp.CodeShared = false;
                        }
                        else
                        {
                            dp.CodeExpires = false;
                            dp.CodeShared = true;

                            var userDetailsEmail = (dynamic)null;
                            var isReceiverIdNull = (dynamic)null;
                            if (uId != null)
                            {
                                userDetailsEmail = db.Users.Where(x => x.UserId == uId && x.IsActive == true).FirstOrDefault();
                            }
                            if (userDetailsEmail != null)
                            {
                                isReceiverIdNull = refCodes.Where(x => x.Receiver == null && x.ReceiverEmail == userDetailsEmail.EmailId && x.IsCodeUsed == false).FirstOrDefault();
                            }

                            var isCodeAssignedToReceiver = refCodes.Where(x => x.Receiver == uId).FirstOrDefault();
                            var isReceiver = refCodes.Where(x => x.Receiver == uId && x.IsCodeUsed == false).FirstOrDefault();
                            if (isCodeAssignedToReceiver == null && isReceiverIdNull == null)
                            {
                                dp.CodeShared = false;
                            }
                            else if (isReceiver != null || (isReceiverIdNull != null))
                            {
                                dp.CodeShared = true;
                                dp.AllowReferralCode = true;
                                dp.AlreadyUsed = false;

                                try
                                {
                                    var referralCodeId = (dynamic)null;
                                    if (isReceiver == null && isReceiverIdNull != null)
                                    {
                                        referralCodeId = isReceiverIdNull.Id;
                                    }
                                    if (isReceiver != null)
                                    {
                                        referralCodeId = isReceiver.Id;
                                    }
                                    var referralCodes = refCodes.Where(x => x.Id == referralCodeId).FirstOrDefault();
                                    referralCodes.IsCodeUsed = true;
                                    if (isReceiverIdNull != null)
                                    {
                                        referralCodes.Receiver = uId;
                                    }
                                    referralCodes.CodeUsedOn = DateTime.Now;
                                    referralCodes.ReferrerRewardAmount = rewardAmount;
                                    referralCodes.ReceiverRewardAmount = rewardAmount;
                                    db.Entry(referralCodes).State = EntityState.Modified;
                                    db.SaveChanges();
                                    decimal? updatedWalletAmount = null;

                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (i == 1)
                                        {
                                            if (isReceiver == null && isReceiverIdNull != null)
                                            {
                                                uId = isReceiverIdNull.Referrer;
                                            }
                                            if (isReceiver != null)
                                            {
                                                uId = isReceiver.Referrer;
                                            }
                                        }
                                        UserWallets uw = db.UserWallets.Where(x => x.UserId == uId).FirstOrDefault();
                                        if (uw != null)
                                        {
                                            decimal? amount = uw.FinalAmountInWallet;
                                            uw.FinalAmountInWallet = amount + rewardAmount;
                                            db.Entry(uw).State = EntityState.Modified;
                                            db.SaveChanges();
                                            if (i == 0)
                                            {
                                                updatedWalletAmount = uw.FinalAmountInWallet;
                                            }
                                        }
                                        else
                                        {
                                            UserWallets wallet = new UserWallets();
                                            wallet.UserId = uId;
                                            wallet.FinalAmountInWallet = rewardAmount;
                                            wallet.IsActive = true;
                                            db.UserWallets.Add(wallet);
                                            db.SaveChanges();
                                            if (i == 0)
                                            {
                                                updatedWalletAmount = wallet.FinalAmountInWallet;
                                            }
                                        }
                                    }
                                    dp.Success = true;

                                    //Send email and SMS to both referrer and receiver
                                    int? senderId = null;
                                    int? receiverId = null;
                                    var RefCodeUserDetails = (dynamic)null;
                                    var senderPhNumber = (dynamic)null;
                                    var receiverPhNumber = (dynamic)null;
                                    var senderEmail = (dynamic)null;
                                    var receiverEmail = (dynamic)null;
                                    string senderName = "";
                                    string receiverName = "";

                                    if (referralCodes != null)
                                    {
                                        senderId = referralCodes.Referrer;
                                        receiverId = referralCodes.Receiver;
                                    }
                                    if (senderId != null)
                                    {
                                        RefCodeUserDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(x => x.a.UserId == senderId).FirstOrDefault();
                                        if (RefCodeUserDetails != null)
                                        {
                                            senderPhNumber = RefCodeUserDetails.b.PhoneNumber;
                                            senderEmail = RefCodeUserDetails.a.EmailId;
                                            senderName = RefCodeUserDetails.b.FirstName + " " + RefCodeUserDetails.b.LastName;
                                        }
                                    }
                                    if (receiverId != null)
                                    {
                                        RefCodeUserDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == receiverId).FirstOrDefault();
                                        if (RefCodeUserDetails != null)
                                        {
                                            receiverPhNumber = RefCodeUserDetails.b.PhoneNumber;
                                            receiverEmail = RefCodeUserDetails.a.EmailId;
                                            receiverName = RefCodeUserDetails.b.FirstName + " " + RefCodeUserDetails.b.LastName;
                                        }
                                    }
                                    if (receiverId == null)
                                    {
                                        if (PhoneNumber != null && PhoneNumber != "")
                                        {
                                            receiverPhNumber = PhoneNumber;
                                        }
                                        if (EmailId != null && EmailId != "")
                                        {
                                            receiverEmail = EmailId;
                                        }
                                    }
                                    //SMS
                                    if (senderPhNumber != null)
                                    {
                                        //SendReferralCodeSMS(refCode, rewardAmount, uId, senderPhNumber,senderName);
                                        SendReferralCodeSMS(senderPhNumber.ToString(), receiverName.ToString(), refCode.ToString(), rewardAmount.ToString(), "Sender", "ApplyCode");
                                    }
                                    if (receiverPhNumber != null)
                                    {
                                        //SendReferralCodeSMS(refCode, rewardAmount, uId, receiverPhNumber,receiverName);
                                        SendReferralCodeSMS(receiverPhNumber.ToString(), receiverName.ToString(), refCode.ToString(), rewardAmount.ToString(), "Receiver", "ApplyCode");
                                    }

                                    //Email
                                    if (senderEmail != null)
                                    {
                                        SendReferralCodeEmail(receiverName.ToString(), "Sender", senderEmail.ToString(), "ApplyCode", refCode.ToString(), rewardAmount.ToString(), referralCodes.CodeValidityString);
                                    }
                                    if (receiverEmail != null)
                                    {
                                        SendReferralCodeEmail(receiverName.ToString(), "Receiver", receiverEmail.ToString(), "ApplyCode", refCode.ToString(), rewardAmount.ToString(), referralCodes.CodeValidityString);
                                    }


                                    dp.RewardAmount = rewardAmount;
                                    dp.UpdatedWalletAmount = updatedWalletAmount;
                                }
                                catch (Exception)
                                {
                                    dp.Success = false;
                                }

                            }
                            else
                            {

                                dp.AllowReferralCode = true;
                                dp.AlreadyUsed = true;

                            }
                        }
                    }
                    else
                    {
                        dp.RefCodeFound = false;
                    }

                }
                else
                {
                    if (isLoggedIn == "false")
                    {
                        if (EmailId != null && EmailId != "")
                        {
                            var refCodes = db.ReferralCodes.Where(x => x.ReferralCode.ToLower() == refCode.ToLower() && x.IsActive == true).ToList();
                            var refCodesExpired = (dynamic)null;
                            var isCodeShared = (dynamic)null;
                            if (refCodes.Count > 0)
                            {
                                //refCodesExpired = refCodes.Where(x => x.SharedOn != null && (DateTime.Now >= x.CodeValidity)).ToList();
                                refCodesExpired = refCodes.Where(x => x.SharedOn != null && x.ReceiverEmail == EmailId && (DateTime.Now >= x.CodeValidity)).ToList();
                                isCodeShared = refCodes.Where(x => x.SharedOn == null && x.ReceiverEmail == null && x.ReceiverPhoneNumber == null).ToList();
                            }

                            if (refCodes.Count == 0)
                            {
                                dp.RefCodeFound = false;
                            }
                            else if (refCodesExpired != null && refCodesExpired.Count > 0)
                            {
                                dp.CodeExpires = true;
                                dp.RefCodeFound = true;
                                dp.CodeShared = false;
                            }
                            else if (isCodeShared != null && isCodeShared.Count > 0)
                            {
                                dp.CodeExpires = false;
                                dp.RefCodeFound = true;
                                dp.CodeShared = false;
                            }
                            else
                            {
                                dp.CodeExpires = false;
                                dp.CodeShared = true;
                                dp.RefCodeFound = true;
                                var checkRecExist = refCodes.Where(x => x.ReceiverEmail == EmailId && x.IsCodeUsed == false).FirstOrDefault();
                                var checkRecExist2 = (dynamic)null;
                                if (PhoneNumber != null && PhoneNumber != "")
                                {
                                    checkRecExist2 = refCodes.Where(x => x.ReceiverPhoneNumber == PhoneNumber && x.IsCodeUsed == false).FirstOrDefault();
                                }

                                if (checkRecExist != null || checkRecExist2 != null)
                                {
                                    dp.NotAReceiver = false;
                                    try
                                    {
                                        checkRecExist.IsCodeUsed = true;
                                        checkRecExist.CodeUsedOn = DateTime.Now;
                                        checkRecExist.ReferrerRewardAmount = rewardAmount;
                                        checkRecExist.ReceiverRewardAmount = rewardAmount;
                                        db.Entry(checkRecExist).State = EntityState.Modified;
                                        db.SaveChanges();
                                        decimal? updatedWalletAmount = null;

                                        //Update Wallet
                                        uId = checkRecExist.Referrer;
                                        UserWallets uw = db.UserWallets.Where(x => x.UserId == uId).FirstOrDefault();
                                        if (uw != null)
                                        {
                                            decimal? amount = uw.FinalAmountInWallet;
                                            uw.FinalAmountInWallet = amount + rewardAmount;
                                            db.Entry(uw).State = EntityState.Modified;
                                            db.SaveChanges();
                                            updatedWalletAmount = uw.FinalAmountInWallet;
                                        }
                                        else
                                        {
                                            UserWallets wallet = new UserWallets();
                                            wallet.UserId = uId;
                                            wallet.FinalAmountInWallet = rewardAmount;
                                            wallet.IsActive = true;
                                            db.UserWallets.Add(wallet);
                                            db.SaveChanges();
                                            updatedWalletAmount = wallet.FinalAmountInWallet;
                                        }

                                        dp.Success = true;

                                        //Send Email and SMS to both referrer and receiver
                                        int? senderId = null;
                                        int? receiverId = null;
                                        var RefCodeUserDetails = (dynamic)null;
                                        var senderPhNumber = (dynamic)null;
                                        var receiverPhNumber = (dynamic)null;
                                        var senderEmail = (dynamic)null;
                                        var receiverEmail = (dynamic)null;
                                        string senderName = "";
                                        string receiverName = "";

                                        if (checkRecExist != null)
                                        {
                                            senderId = checkRecExist.Referrer;
                                            receiverId = checkRecExist.Receiver;
                                        }
                                        if (senderId != null)
                                        {
                                            RefCodeUserDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(x => x.a.UserId == senderId).FirstOrDefault();
                                            if (RefCodeUserDetails != null)
                                            {
                                                senderPhNumber = RefCodeUserDetails.b.PhoneNumber;
                                                senderEmail = RefCodeUserDetails.a.EmailId;
                                                senderName = RefCodeUserDetails.b.FirstName + " " + RefCodeUserDetails.b.LastName;
                                            }
                                        }
                                        if (receiverId != null)
                                        {
                                            RefCodeUserDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == receiverId).FirstOrDefault();
                                            if (RefCodeUserDetails != null)
                                            {
                                                receiverPhNumber = RefCodeUserDetails.b.PhoneNumber;
                                                receiverEmail = RefCodeUserDetails.a.EmailId;
                                                receiverName = RefCodeUserDetails.b.FirstName + " " + RefCodeUserDetails.b.LastName;
                                            }
                                        }
                                        if (receiverId == null)
                                        {
                                            var phnNumber = checkRecExist.ReceiverPhoneNumber;

                                            if (PhoneNumber != null && PhoneNumber != "")
                                            {
                                                receiverPhNumber = PhoneNumber;
                                            }
                                            else
                                            {
                                                if (phnNumber != null)
                                                {
                                                    receiverPhNumber = phnNumber;
                                                }
                                            }
                                            if (EmailId != null && EmailId != "")
                                            {
                                                receiverEmail = EmailId;
                                            }
                                        }
                                        //SMS
                                        if (senderPhNumber != null)
                                        {
                                            SendReferralCodeSMS(senderPhNumber.ToString(), receiverName.ToString(), refCode.ToString(), rewardAmount.ToString(), "Sender", "ApplyCode");
                                        }
                                        if (receiverPhNumber != null)
                                        {
                                            SendReferralCodeSMS(receiverPhNumber.ToString(), receiverName.ToString(), refCode.ToString(), rewardAmount.ToString(), "Receiver", "ApplyCode");
                                        }

                                        //Email
                                        if (senderEmail != null)
                                        {
                                            SendReferralCodeEmail(receiverName.ToString(), "Sender", senderEmail.ToString(), "ApplyCode", refCode.ToString(), rewardAmount.ToString(), checkRecExist.CodeValidityString);
                                        }
                                        if (receiverEmail != null)
                                        {
                                            SendReferralCodeEmail(receiverName.ToString(), "Receiver", receiverEmail.ToString(), "ApplyCode", refCode.ToString(), rewardAmount.ToString(), checkRecExist.CodeValidityString);
                                        }


                                        dp.RewardAmount = rewardAmount;
                                        dp.UpdatedWalletAmount = updatedWalletAmount;
                                    }
                                    catch (Exception)
                                    {
                                        dp.Success = false;
                                    }
                                }
                                else
                                {
                                    dp.NotAReceiver = true;
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                dp.RefCodeFound = false;
            }

            dp = JsonConvert.SerializeObject(dp);
            userStatus.Add(dp);
            return Json(userStatus);
        }
        private void SendReferralCodeSMS(string PhoneNumber, string Name, string refCode, string rewardAmount, string userType, string operationType)
        {
            if (PhoneNumber != null && PhoneNumber != "")
            {
                try
                {
                    Int64? numberToSend = Int64.Parse(PhoneNumber);
                    string msg = "";
                    if (userType == "Sender" && operationType == "ShareCode")
                    {
                        msg = String.Format(Millionlights.Models.Constants.RefCodeSharedSuccessMsgFromSenderToReceiver, Name, refCode, rewardAmount);
                    }
                    if (userType == "Sender" && operationType == "ApplyCode")
                    {
                        msg = String.Format(Millionlights.Models.Constants.RefCodeRedeemBenefitSuccessMsgToReferrer, Name, refCode, rewardAmount);
                    }
                    if (userType == "Receiver" && operationType == "ApplyCode")
                    {
                        msg = String.Format(Millionlights.Models.Constants.RefCodeRedeemBenefitSuccessMsgToReceiver, refCode, rewardAmount);
                    }

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
                }
                catch (Exception)
                {
                }
            }
        }
        private void SendReferralCodeEmail(string Name, string userType, string EmailId, string operationType, string referralCode, string rewardAmount, string validity)
        {
            //Send Email
            string userName = "";
            string subject = null;
            if (Name != null && Name != "")
            {
                userName = Name;
            }
            string regTemplate = null;
            if (userType == "Sender" && operationType == "ShareCode")
            {
                regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ShareCodeEmailToReceiver.htm");
                subject = "Invitation from your friend to register to MillionLights and earn rewards.";
            }

            if (userType == "Sender" && operationType == "ApplyCode")
            {
                regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ApplyCodeEmailToReferrer.htm");
                subject = "The shared code has been used by the receiver and the reward amount has been added to your wallet.";
            }

            if (userType == "Receiver" && operationType == "ApplyCode")
            {
                regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "ApplyCodeEmailToReceiver.htm");
                subject = "Referral Code successfully applied. The reward amount has been added to your wallet.";
            }

            if (EmailId != null)
            {
                MillionLightsEmails mEmail = new MillionLightsEmails();
                mEmail.SendRefCodeEmail(
                    ConfigurationManager.AppSettings["SenderName"],
                    ConfigurationManager.AppSettings["SenderEmail"],
                     ConfigurationManager.AppSettings["Telephone"],
                      ConfigurationManager.AppSettings["EmailId"],
                    subject,
                    regTemplate,
                    userName,
                    EmailId,
                    referralCode,
                    rewardAmount,
                    validity,
                    userType,
                    operationType
                    );
            }
        }
        public ActionResult ShareCode(string codeToShare, string UserId, string EmailId, string PhoneNumber)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<dynamic> shareCodeStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            var settings = db.HomePageConfigurations.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            //decimal rewardAmount=settings.RewardAmount;
            decimal? rewardAmount = 0;
            if (settings != null)
            {
                if (settings.RewardAmount != null)
                {
                    rewardAmount = decimal.Parse(settings.RewardAmount);
                }
                else
                {
                    rewardAmount = 0.00M;
                }
               
            }

            if (codeToShare != null && codeToShare != "")
            {
                dp.CodeToShare = true;
                if (UserId != null && UserId != "")
                {
                    dp.UserSession = true;

                    int? uId = int.Parse(UserId);
                    if (Session["UserID"] == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    if ((EmailId == null || EmailId == "") && (PhoneNumber == null || PhoneNumber == ""))
                    {
                        dp.InputProvided = false;
                    }
                    else
                    {
                        dp.InputProvided = true;

                        var refCodes = db.ReferralCodes.Where(x => x.ReferralCode == codeToShare && x.IsActive == true).ToList();
                        var codeSharedToSel = refCodes.Where(x => x.Referrer == uId && x.Receiver == uId && x.IsActive == true).ToList();
                        if (refCodes.Count == 0)
                        {
                            dp.CodeValid = false;
                        }
                        else
                        {
                            dp.CodeValid = true;
                            var userDetailsEmail = (dynamic)null;
                            var userDetailsPhone = (dynamic)null;
                            if (EmailId != null && EmailId != "")
                            {
                                userDetailsEmail = db.Users.Where(x => x.EmailId == EmailId && x.IsActive == true).FirstOrDefault();
                            }
                            else if (PhoneNumber != null && PhoneNumber != "")
                            {
                                Int64? phNO = Int64.Parse(PhoneNumber);
                                userDetailsPhone = db.UsersDetails.Where(x => x.PhoneNumber == phNO && x.IsActive == true).FirstOrDefault();
                            }


                            if (userDetailsEmail != null || userDetailsPhone != null)
                            {
                                dp.ReceiverExist = true;
                            }
                            else if (codeSharedToSel.Count > 0)
                            {
                                dp.ReceiverExist = false;
                                dp.CantShareCodeWithSelf = true;
                            }
                            else
                            {
                                dp.ReceiverExist = false;
                                dp.CantShareCodeWithSelf = false;
                                var codeAlreadySharedEmail = 0;
                                if (EmailId != null && EmailId != "")
                                {
                                    codeAlreadySharedEmail = refCodes.Where(x => x.ReceiverEmail == EmailId).ToList().Count();
                                }
                                var codeAlreadySharedPhone = (dynamic)null;
                                if (PhoneNumber != null && PhoneNumber != "")
                                {
                                    codeAlreadySharedPhone = refCodes.Where(x => x.ReceiverPhoneNumber == PhoneNumber).ToList().Count();
                                }

                                if (codeAlreadySharedEmail > 0 || codeAlreadySharedPhone > 0)
                                {
                                    dp.CodeAlreadyShared = true;
                                }
                                else
                                {
                                    dp.CodeAlreadyShared = false;
                                    try
                                    {
                                        var refCC = refCodes.Where(x => x.Referrer == uId && x.SharedOn == null && x.ReceiverEmail == null && x.ReceiverPhoneNumber == null).FirstOrDefault();
                                        if (refCC != null)
                                        {
                                            refCC.ReferralCode = codeToShare;
                                            refCC.Referrer = uId;
                                            refCC.ReceiverEmail = EmailId;
                                            refCC.ReceiverPhoneNumber = PhoneNumber;
                                            refCC.SharedOn = DateTime.Now;
                                            refCC.IsCodeUsed = false;
                                            refCC.CodeUsedOn = null;
                                            refCC.Receiver = null;
                                            refCC.ReferrerRewardAmount = null;
                                            refCC.ReceiverRewardAmount = null;
                                            refCC.CodeValidity = DateTime.Now.AddMonths(1);
                                            refCC.IsActive = true;
                                            db.Entry(refCC).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var rCodes = refCodes.Where(x => x.Referrer == uId).FirstOrDefault();
                                            ReferralCodes refC = new ReferralCodes();
                                            refC.ReferralCode = codeToShare;
                                            refC.Referrer = uId;
                                            refC.ReceiverEmail = EmailId;
                                            refC.ReceiverPhoneNumber = PhoneNumber;
                                            refC.SharedOn = DateTime.Now;
                                            refC.IsCodeUsed = false;
                                            refC.CodeUsedOn = null;
                                            refC.Receiver = null;
                                            refC.ReferrerRewardAmount = null;
                                            refC.ReceiverRewardAmount = null;
                                            refC.CodeGeneratedOn = rCodes.CodeGeneratedOn;
                                            refC.CodeValidity = DateTime.Now.AddMonths(1);
                                            refC.IsActive = true;
                                            db.ReferralCodes.Add(refC);
                                            db.SaveChanges();
                                        }

                                        dp.IsSuccess = true;

                                        //Send Email to the code receiver
                                        string Name = "";
                                        if (uId != null)
                                        {
                                            var userReg = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
                                            Name = userReg.b.FirstName + " " + userReg.b.LastName + " (" + userReg.a.EmailId + ")";
                                        }
                                        if (EmailId != null && EmailId != "")
                                        {
                                            SendReferralCodeEmail(Name.ToString(), "Sender", EmailId.ToString(), "ShareCode", codeToShare.ToString(), rewardAmount.ToString(), DateTime.Now.AddMonths(1).ToString(@"dd/MM/yyyy H:mm:ss"));
                                        }
                                        //Send SMS to the code receiver
                                        if (PhoneNumber != null && PhoneNumber != "")
                                        {
                                            SendReferralCodeSMS(PhoneNumber.ToString(), Name.ToString(), codeToShare.ToString(), rewardAmount.ToString(), "Sender", "ShareCode");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        dp.IsSuccess = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    dp.UserSession = false;
                }
            }
            else
            {
                dp.CodeToShare = false;
            }

            dp = JsonConvert.SerializeObject(dp);
            shareCodeStatus.Add(dp);
            return Json(shareCodeStatus);
        }

        public ActionResult SubmitUsersRatings(string ratingStars, string courseId, string comments, string userId)
        {
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int? uId = int.Parse(userId);
            List<dynamic> courseRatingStatus = new List<dynamic>();
            dynamic dp = new ExpandoObject();

            decimal? rating = null;
            if (ratingStars != null && ratingStars != "")
            {
                rating = decimal.Parse(ratingStars);
            }

            int? cId = null;
            if (courseId != null && courseId != "")
            {
                cId = int.Parse(courseId);
            }

            var isRated = (dynamic)null;
            isRated = db.UsersCourseRatings.Where(x => x.RatingsProvidedBy == uId && x.CourseId == cId && x.IsActive == true).ToList();
            if (isRated.Count > 0)
            {
                dp.AlreadyRated = true;
            }
            else
            {
                dp.AlreadyRated = false;
                try
                {
                    UsersCourseRatings usersCourseRatings = new UsersCourseRatings();
                    usersCourseRatings.RatingsProvidedBy = uId;
                    usersCourseRatings.CourseId = cId;
                    usersCourseRatings.CourseRatings = rating;
                    usersCourseRatings.UsersComments = comments;
                    usersCourseRatings.RatingDatetime = DateTime.Now;
                    usersCourseRatings.IsActive = true;
                    db.UsersCourseRatings.Add(usersCourseRatings);
                    db.SaveChanges();
                    dp.IsSuccess = true;

                    //Update the Average Course Rating
                    SaveCourseRatings((int)cId);
                }
                catch (Exception)
                {
                    dp.IsSuccess = false;
                }
            }

            dp = JsonConvert.SerializeObject(dp);
            courseRatingStatus.Add(dp);
            return Json(courseRatingStatus);
        }

        private void SaveCourseRatings(int cId)
        {
            int averageRatings = 0;
            //Course Ratings Calculations
            var courseRatings = db.UsersCourseRatings.Where(x => x.CourseId == cId).ToList();
            if (courseRatings.Count() > 0)
            {
                var rating = courseRatings.GroupBy(x => x.CourseId).Select(y => y.Sum(x => x.CourseRatings)).FirstOrDefault();
                if (rating != null)
                {
                    var finalCourseRating = rating / courseRatings.Count;
                    averageRatings = (int)Math.Ceiling(decimal.Parse(finalCourseRating.ToString()));
                }
            }
            //Save Ratings
            var course = db.Courses.Where(x => x.Id == cId).FirstOrDefault<Course>();
            course.CourseRatings = averageRatings;
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            return;
        }
    }
}