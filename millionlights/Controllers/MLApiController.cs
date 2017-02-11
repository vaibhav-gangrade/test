using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Millionlights.Models;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Millionlights.EmailService;
using System.Web.Configuration;
using System.Web.UI;
using iTextSharp.text.html;
using iTextSharp.text.xml;
using iTextSharp.text.xml.simpleparser;
using iTextSharp.text.xml.simpleparser.handler;
//using iTextSharp.tool.xml;

namespace Millionlights.Controllers
{

    public class MLApiController : ApiController
    {
        private MillionlightsContext db = new MillionlightsContext();
        private const string LinkedInUrl = "https://www.linkedin.com/profile/add?_ed=0_-jC6n2zx9sgwZAnN2pxOpec75f3dYLvjm5QuhUDwl00J-aRcCdrc3AhiDaqDnloqaSgvthvZk7wTBMS3S-m0L6A6mLjErM6PJiwMkk6nYZylU7__75hCVwJdOTZCAkdv&pfCertificationName={0}&pfCertificationUrl=https://www.millionlights.org/{1}&pfLicenseNo={2}&pfCertStartDate={3}&trk=onsite_longurl";
        [Authorize]
        [Route("api/me")]
        public UserViewModel GetUserDetails()
        {
            string req = Request.RequestUri.AbsoluteUri;
            string query = Request.RequestUri.Query;
            string content = Request.Content.ToString();

            UserViewModel userViewModel = null;

            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                var userInfo = db.Users.Join(db.UsersDetails, u => u.UserId, ud => ud.UserId, (u, ud) => new { u, ud })
                                .Where(x => x.u.EmailId == User.Identity.Name).FirstOrDefault();
                if (userInfo != null)
                {
                    userViewModel = new UserViewModel();
                    userViewModel.Firstname = userInfo.ud.FirstName;
                    userViewModel.Lastname = userInfo.ud.LastName;
                    userViewModel.Email = this.User.Identity.Name;
                    userViewModel.UserId = userInfo.u.UserId;
                    if (userInfo.ud.ImageURL != null)
                    {
                        //userViewModel.UserImgUrl = "https://www.millionlights.org/Images/UsersImg/" + userInfo.ud.ImageURL;
                        userViewModel.UserImgUrl = HttpContext.Current.Request.Url.Host + "/Images/UsersImg/" + userInfo.ud.ImageURL;
                    }
                    else
                    {
                        //userViewModel.UserImgUrl = "https://www.millionlights.org/Content/assets/avatars/ProfileImage.png";
                        userViewModel.UserImgUrl = HttpContext.Current.Request.Url.Host + "/Content/assets/avatars/ProfileImage.png";
                    }
                    userViewModel.UserName = userInfo.u.UserName;
                    return userViewModel;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User details are not found. Please check and try again.."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/MyCourses")]
        public List<UserCoursesModel> GetUserCourses()
        {
            List<UserCoursesModel> userCourse = null;
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                userCourse = new List<UserCoursesModel>();

                var userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();

                List<UsersCourses> userCourseList = db.UsersCourses.Where(x => x.UserId == userInfo.UserId && x.IsActive == true).ToList();

                foreach (var course in userCourseList)
                {
                    UserCoursesModel userCourseViewModel = new UserCoursesModel();

                    var availcourse = db.Courses.Where(x => x.Id == course.CourseID).FirstOrDefault();
                    if (availcourse != null)
                    {
                        var CompleteEDXLink = availcourse.EDXCourseLink;
                        string[] edxLinksArr = null;
                        try
                        {
                            edxLinksArr = CompleteEDXLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);

                            if (edxLinksArr.Length > 0)
                            {
                                int lastSlash = edxLinksArr[1].LastIndexOf('/');
                                string LMSCourseId = (lastSlash > -1) ? edxLinksArr[1].Substring(0, lastSlash) : edxLinksArr[1];
                                userCourseViewModel.CourseName = availcourse.CourseName;
                                userCourseViewModel.CourseLMSId = LMSCourseId;
                                userCourseViewModel.MLCourseId = availcourse.Id;
                                userCourse.Add(userCourseViewModel);
                            }
                        }
                        catch (Exception)
                        {
                            userCourseViewModel.CourseName = availcourse.CourseName;
                            userCourseViewModel.CourseLMSId = GetLMSCourseId(availcourse.EDXCourseLink);
                            userCourseViewModel.MLCourseId = availcourse.Id;
                            userCourse.Add(userCourseViewModel);

                        }
                    }
                }
                return userCourse;
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/GetCourseDetails")]
        public List<UserCoursesModel> GetCourseDetailsByLMSID(string lmsCourseId)
        {
            if (lmsCourseId != null && lmsCourseId != "")
            {
                List<UserCoursesModel> userCourse = null;
                int lastSlash = lmsCourseId.LastIndexOf('/');
                string LMSCourseId = (lastSlash > -1) ? lmsCourseId.Substring(0, lastSlash) : lmsCourseId;
                string[] splitLMSIDString = LMSCourseId.Split('/');
                var message = string.Format("The course with this LMSID is not found. Please check the LMS Course ID.");

                if (splitLMSIDString.Length > 1 && (splitLMSIDString[1] != null || splitLMSIDString[1] != ""))
                {
                    userCourse = new List<UserCoursesModel>();
                    UserCoursesModel userCourseViewModel = new UserCoursesModel();
                    string lmsCourseID = splitLMSIDString[1];
                    var availcourse = db.Courses.Where(x => x.LMSCourseId == lmsCourseID).Select(x => new UserCoursesModel
                    {
                        CourseName = x.CourseName,
                        CourseLMSId = x.LMSCourseId,
                        EDXCourseLink = x.EDXCourseLink,
                        MLCourseId = x.Id
                    }).FirstOrDefault();
                    if (availcourse != null)
                    {
                        string[] edxLinksArr = availcourse.EDXCourseLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);
                        if (edxLinksArr.Length > 1)
                        {
                            string LMSCourseId2 = string.Empty;
                            if (edxLinksArr[1].EndsWith("/"))
                            {
                                LMSCourseId2 = edxLinksArr[1].Substring(0, edxLinksArr[1].Length - 1);
                                if (lmsCourseId.EndsWith("/"))
                                {
                                    lmsCourseId = lmsCourseId.Substring(0, lmsCourseId.Length - 1);
                                }
                                //string LMSCourseId2 = (lastSlash > -1) ? edxLinksArr[1].Substring(0, lastSlash) : edxLinksArr[1];
                                if (LMSCourseId2.ToLower() == lmsCourseId.ToLower())
                                {
                                    userCourseViewModel.CourseName = availcourse.CourseName;
                                    //userCourseViewModel.CourseLMSId = availcourse.CourseLMSId;changed to below as per Srini
                                    userCourseViewModel.CourseLMSId = GetLMSCourseId(availcourse.EDXCourseLink);
                                    userCourseViewModel.EDXCourseLink = availcourse.EDXCourseLink;
                                    userCourseViewModel.MLCourseId = availcourse.MLCourseId;
                                    userCourse.Add(userCourseViewModel);
                                    return userCourse;
                                }
                                else
                                {
                                    //return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     message));
                                }
                            }
                            else
                            {
                                LMSCourseId2 = edxLinksArr[1];
                                if (lmsCourseId.EndsWith("/"))
                                {
                                    lmsCourseId = lmsCourseId.Substring(0, lmsCourseId.Length - 1);
                                }
                                if (LMSCourseId2.ToLower() == lmsCourseId.ToLower())
                                {
                                    userCourseViewModel.CourseName = availcourse.CourseName;
                                    //userCourseViewModel.CourseLMSId = availcourse.CourseLMSId;changed to below as per Srini
                                    userCourseViewModel.CourseLMSId = GetLMSCourseId(availcourse.EDXCourseLink);
                                    userCourseViewModel.EDXCourseLink = availcourse.EDXCourseLink;
                                    userCourseViewModel.MLCourseId = availcourse.MLCourseId;
                                    userCourse.Add(userCourseViewModel);
                                    return userCourse;
                                }
                                else
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     message));
                                }
                            }
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     message));
                        }
                    }
                    else
                    {

                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     message));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     message));
                    //throw new JsonException("The course with this LMSID is not found. Please check the LMS Course ID.");
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "Please provide the lmsCourseId to find the course"));
            }
        }

        [Route("api/GetAllCourses")]
        public List<AllCoursesModel> GetAllCoursesPublic()
        {
            List<AllCoursesModel> userCourse = new List<AllCoursesModel>();

            List<Course> courseList = db.Courses.Where(x => x.IsActive == true).ToList();
            if (courseList.Count() > 0)
            {
                foreach (var course in courseList)
                {
                    AllCoursesModel courseViewModel = new AllCoursesModel();
                    courseViewModel.Id = course.Id;
                    courseViewModel.CourseCode = course.CourseCode;
                    courseViewModel.CourseName = course.CourseName;
                    courseViewModel.ShortDescription = course.ShortDescription;
                    courseViewModel.LongDescription = course.LongDescription;
                    courseViewModel.Objective = course.Objective;
                    courseViewModel.ExamObjective = course.ExamObjective;
                    courseViewModel.EDXCourseLink = course.EDXCourseLink;
                    courseViewModel.StartDate = course.StartDate;
                    courseViewModel.EndDate = course.EndDate;
                    courseViewModel.CreatedOn = course.CreatedOn;
                    courseViewModel.BasePrice = course.BasePrice;
                    //courseViewModel.LMSCourseId = course.LMSCourseId;changed to below line as per Srini
                    courseViewModel.LMSCourseId = GetLMSCourseId(course.EDXCourseLink);
                    courseViewModel.Instructor = course.Instructor;
                    userCourse.Add(courseViewModel);
                }
                return userCourse;
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "No Courses are found.."));
            }

        }

        [Route("api/MyOrders")]
        public List<OrderManagement> GetMyOrders()
        {
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                var userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    //var OrderDetails = db.Orders.Join(db.OrderStatus, O => O.OrderStatusID, OS => OS.OrderStatusID, (O, OS) => new { O, OS }).Where(X => X.O.UserID == id).OrderBy(x => x.O.OrderID).ToList();
                    var OrderDetails = db.Courses.Join(db.ItemsOrders.Join(db.Orders.Join(db.OrderStatus, IO => IO.OrderStatusID, OS => OS.OrderStatusID, (IO, OS) => new { IO, OS }), IOI => IOI.OrderID, O => O.IO.OrderID, (IOI, O) => new { IOI, O }), U => U.Id, OIO => OIO.IOI.CourseId, (OIO, U) => new { OIO, U }).Where(X => X.U.O.IO.UserID == userInfo.UserId).ToList();  //Where(X => X.U.O.IO.IsActive == true).ToList();
                    if (OrderDetails.Count() > 0)
                    {
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
                            }
                        }
                        return order;
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "No orders are found.."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/MyCoupons")]
        public List<MyCoupon> GetMyCoupons()
        {
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                var userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    List<MyCoupon> MyCoupon = new List<MyCoupon>();
                    //List<CoupanCourseDetails> CouCorDetails = new List<CoupanCourseDetails>();
                    List<CouponCourses> courseDetails = new List<CouponCourses>();

                    var allUniqueUserCoupons = db.Coupons.GroupJoin(
                        db.UserCoupons,
                        coupon => coupon.Id,
                        userCoupon => userCoupon.CouponId,
                        (x, y) => new { Coupon = x, UserCoupon = y })
                        .SelectMany(
                        x => x.UserCoupon.DefaultIfEmpty(),
                        (x, y) => new { Coupon = x.Coupon, UserCoupon = y }).Where(z => z.Coupon.UserId == userInfo.UserId || (z.UserCoupon.ActivateBy == userInfo.UserId && z.UserCoupon.ActivateBy != z.Coupon.UserId)).ToList();
                    string cNames = string.Empty;
                    string cCodes = string.Empty;
                    string cIds = string.Empty;

                    if (allUniqueUserCoupons.Count() > 0)
                    {
                        foreach (var item in allUniqueUserCoupons)
                        {
                            DateTime? ActivatedOn = null;
                            string RedeemStatus = null;
                            int? couponId = null;
                            int? ActivateByUserId = null;
                            if (item.UserCoupon != null)
                            {
                                couponId = item.UserCoupon.CouponId;
                                RedeemStatus = item.UserCoupon.RedeemStatus;
                                ActivatedOn = item.UserCoupon.ActivateOn;
                                ActivateByUserId = item.UserCoupon.ActivateBy;
                            }
                            // var couponCourseCount = db.CouponCourses.Where(x => x.CouponId == item.Coupon.Id).Count();
                            MyCoupon mc = new MyCoupon();
                            //CoupanCourseDetails CCD = new CoupanCourseDetails();
                            mc.CoupanId = item.Coupon.Id;
                            mc.CCGId = item.Coupon.CCGId;
                            mc.CouponCode = item.Coupon.CouponCode;
                            mc.PartnerName = item.Coupon.PartnerName;
                            mc.CreatedOn = item.Coupon.CreatedOn;
                            mc.ValidFrom = item.Coupon.ValidFrom;
                            mc.ValidTo = item.Coupon.ValidTo;
                            mc.BenefitName = item.Coupon.BenefitName;
                            mc.DiscountPrice = item.Coupon.DiscountPrice;
                            mc.IsPrepaid = item.Coupon.IsPrepaid;
                            mc.AllowedCourses = item.Coupon.AllowedCourses;


                            if (item.Coupon.AllowedCourses == 1 && RedeemStatus == "Partial")
                            {
                                mc.CouponRedeemStatus = "Complete";
                            }
                            else
                            {
                                mc.CouponRedeemStatus = RedeemStatus;
                            }
                            bool isCouponExpired = DateTime.Now > item.Coupon.ValidTo ? true : false;
                            if (isCouponExpired)
                            {
                                mc.CouponExpired = "YES";
                            }
                            else
                            {
                                mc.CouponExpired = "NO";
                            }
                            mc.ActivatedByUserId = ActivateByUserId.ToString();
                            mc.ActivatedOn = ActivatedOn;
                            mc.BlockedOn = item.Coupon.BlockedOn;
                            mc.BlockedReason = item.Coupon.BlockedReason;


                            //CCD.CourseCount = couponCourseCount;

                            //Get Courses
                            //var couponInfo = db.Courses.Join(db.CouponCourses, c => c.Id, cc => cc.CourseId, (c, cc) => new { c, cc })
                            //                  .Join(db.Coupons, ccc => ccc.cc.CouponId, coup => coup.Id, (ccc, coup) => new { ccc, coup })
                            //                 .Where(x => x.ccc.cc.CouponId == item.Coupon.Id).ToList();


                            var courses = db.Courses.Join(db.CouponCourses, c => c.Id, cc => cc.CourseId, (c, cc) => new { c, cc })
                                .Where(x => x.cc.CouponId == item.Coupon.Id).ToList();
                            if (courses.Count() > 0)
                            {
                                foreach (var courseItem in courses)
                                {
                                    string cId = string.Empty;
                                    CouponCourses course = new CouponCourses();
                                    course.CouponId = courseItem.cc.CouponId;
                                    course.CourseCode = courseItem.c.CourseCode;
                                    course.CName = courseItem.c.CourseName;
                                    course.ShortDescription = courseItem.c.ShortDescription;
                                    course.BasePrice = courseItem.c.BasePrice;
                                    //var courseRedeem = db.UsersCourses.Where(x => x.CourseID == courseItem.cc.CourseId && x.UserId == id && x.CouponApplied == item.Coupon.CouponCode).Count();
                                    var courseRedeem = db.UsersCourses.Where(x => x.CourseID == courseItem.cc.CourseId && x.UserId == userInfo.UserId).Count();
                                    if (couponId != null && courseRedeem > 0)
                                    {
                                        course.IsCourseRedeemed = "Yes";
                                    }
                                    else
                                    {
                                        course.IsCourseRedeemed = "No";

                                    }
                                    if (!string.IsNullOrEmpty(cNames))
                                    {
                                        cNames = courseItem.c.CourseName + ' ' + ',' + ' ' + cNames;
                                    }
                                    else
                                    {
                                        cNames = courseItem.c.CourseName;
                                    }
                                    if (!string.IsNullOrEmpty(cCodes))
                                    {
                                        cCodes = courseItem.c.CourseCode + ' ' + ',' + ' ' + cCodes;
                                    }
                                    else
                                    {
                                        cCodes = courseItem.c.CourseCode;
                                    }
                                    if (!string.IsNullOrEmpty(cIds))
                                    {
                                        cId = courseItem.c.Id.ToString();
                                        cIds = cId + ' ' + ',' + ' ' + cIds;
                                    }
                                    else
                                    {
                                        cId = courseItem.c.Id.ToString();
                                        cIds = cId;
                                    }
                                    courseDetails.Add(course);
                                }
                            }

                            mc.ApplicableCourseIds = cIds;
                            mc.ApplicableCourseCodes = cCodes;
                            mc.ApplicableCourseNames = cNames;
                            MyCoupon.Add(mc);
                        }
                        return MyCoupon;
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                                              "No Coupons are found.."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/MyNotifications")]
        public List<UserNotitification> GetMyNotifications()
        {
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                var userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    var notifications = db.UserNotitifications.Where(x => x.Receiver == userInfo.UserId && x.IsAlert == false).ToList();
                    if (notifications.Count() > 0)
                    {
                        List<UserNotitification> uNot = new List<UserNotitification>();
                        foreach (var notific in notifications)
                        {
                            UserNotitification uNotification = new UserNotitification();
                            uNotification.Id = notific.Id;
                            uNotification.Receiver = userInfo.UserId;
                            uNotification.Sender = notific.Sender;
                            uNotification.Subject = notific.Subject;
                            uNotification.Message = notific.Message;
                            uNotification.DateSent = notific.DateSent;
                            uNotification.IsRead = notific.IsRead;
                            uNotification.ReadDate = notific.ReadDate;
                            uNotification.IsAlert = notific.IsAlert;
                            uNotification.SMSDate = notific.SMSDate;
                            uNotification.MailDate = notific.MailDate;
                            uNotification.NotificationStatusId = notific.NotificationStatusId;
                            uNot.Add(uNotification);

                        }
                        return uNot;
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                                              "No user notifications are found.."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }
        [Route("api/ChangePassword")]
        [AcceptVerbs("GET", "POST")]
        public string ChangeUserPassword(string currentPass, string newPass, string confirmPass)
        {

            //string returnMsg = "";
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                var userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();

                if (userInfo != null)
                {
                    if (currentPass != null && newPass != null && confirmPass != null)
                    {
                        if (confirmPass == newPass)
                        {
                            var isUserPwd = db.Users.Where(x => x.UserId == userInfo.UserId && x.Password == currentPass).FirstOrDefault();
                            if (isUserPwd != null)
                            {
                                User uInfo = db.Users.Find(userInfo.UserId);
                                if (uInfo != null)
                                {
                                    uInfo.Password = newPass;
                                    db.Entry(uInfo).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "Password changed successfully"));
                                }
                                else
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "User account does not exist."));
                                }

                                //returnMsg="Password changed successfully";
                            }
                            else
                            {
                                //returnMsg="Please provide correct current password";
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "Please provide correct current password"));
                            }
                        }
                        else
                        {
                            //returnMsg="New password and password doesnt match";
                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "New password and confirm password doesnt match"));
                        }
                    }
                    else
                    {
                        //returnMsg="Please provide valid current password, new password and confirm password";
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "Please provide valid current password, new password and confirm password"));
                    }
                    //ChangePasswordMessage msg = new ChangePasswordMessage
                    //{
                    //    Message = returnMsg
                    //};
                    //string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                    //return jsonString;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/UpdateProfile")]
        [AcceptVerbs("GET", "POST")]
        public string UpdateProfile(string fullName, string add1, string add2, string city, string state, string country, string zipCode, string phoneNumber)
        {

            //string returnMsg = "";
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                User userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    UserDetails userDetails = db.UsersDetails.Where(x => x.UserId == userInfo.UserId).FirstOrDefault();
                    if (userDetails != null)
                    {
                        var names = (dynamic)null;
                        string firstName = null;
                        string lastName = null;

                        if (fullName != null && fullName != "")
                        {
                            names = fullName.Split(' ');
                            firstName = names[0];
                            lastName = names[1];
                            userDetails.FirstName = firstName;
                            userDetails.LastName = lastName;
                        }
                        else
                        {
                            userDetails.FirstName = firstName;
                            userDetails.LastName = lastName;
                        }
                        userDetails.AddressLine1 = add1;
                        userDetails.AddressLine2 = add2;
                        userDetails.City = city;
                        userDetails.State = state;
                        userDetails.Country = country;
                        userDetails.ZipCode = zipCode;
                        userDetails.PhoneNumber = Int64.Parse(phoneNumber);
                        //Update UserDetails table record
                        db.Entry(userDetails).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //Update Users table record
                        userInfo.EmailId = userInfo.EmailId;
                        db.Entry(userInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //returnMsg = "Profile Updated Successfully";
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "Profile Updated Successfully"));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                                             "User account does not exist."));
                    }

                    //ChangePasswordMessage msg = new ChangePasswordMessage
                    //{
                    //    Message = returnMsg
                    //};
                    //string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                    //return jsonString;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                //throw new JsonException("Access is denied due to invalid credentials");
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/UserRegister")]
        [AcceptVerbs("GET", "POST")]
        public string UserRegister(string firstName, string lastName, string phoneNumber, string email, string userName, string password, string confirmPassword, string add1, string add2, string city, string zipCode)
        {
            UserRegisterController uregCont = new UserRegisterController();
            string returnMsg = null;
            if (email != null && email != "")
            {
                var userInfo = db.Users.Where(u => u.EmailId == email).FirstOrDefault();
                if (userInfo == null)
                {
                    if (password != null && confirmPassword != null)
                    {
                        if (password == confirmPassword)
                        {
                            try
                            {
                                User user = new User();
                                user.EmailId = email;
                                user.UserName = userName;
                                user.Password = uregCont.passwordEncrypt(password);
                                user.IsActive = true;
                                //if (userReg.UserType != null)
                                //{
                                //    user.UserType = userReg.UserType;
                                //    user.ProviderKey = userReg.ProviderKey;
                                //}
                                //else
                                //{
                                user.UserType = "MillionLight";
                                user.ProviderKey = "00000";
                                //}

                                db.Users.Add(user);
                                db.SaveChanges();
                                int userId = user.UserId;
                                //Session["UserID"] = userId;
                                UserInRole usrInRole = new UserInRole();
                                usrInRole.UserId = userId;
                                usrInRole.RoleId = 2;
                                db.UserInRoles.Add(usrInRole);
                                db.SaveChanges();

                                UserDetails userDetails = new UserDetails();
                                userDetails.UserId = userId;
                                userDetails.AddressLine1 = add1;
                                userDetails.AddressLine2 = add2;
                                userDetails.City = city;
                                userDetails.State = null;
                                userDetails.Country = null;
                                userDetails.ZipCode = zipCode;
                                userDetails.FirstName = firstName;
                                userDetails.LastName = lastName;
                                userDetails.PhoneNumber = Int64.Parse(phoneNumber);
                                userDetails.IsActive = true;
                                string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                                userDetails.PartnerId = int.Parse(partId);
                                userDetails.RegisteredDatetime = DateTime.Now;
                                db.UsersDetails.Add(userDetails);
                                db.SaveChanges();

                                //Send Email
                                string uName = firstName + " " + lastName;
                                string regTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "StudentRegistrationEmailTemplate.htm");
                                int? NotificationId = null;
                                    MillionLightsEmails mEmail = new MillionLightsEmails();
                                    mEmail.SendStudentRegistrationCompleteEmail(
                                        ConfigurationManager.AppSettings["SenderName"],
                                        ConfigurationManager.AppSettings["SenderEmail"],
                                         ConfigurationManager.AppSettings["Telephone"],
                                          ConfigurationManager.AppSettings["EmailId"],
                                        "Your Millionlights Registration Successful",
                                        regTemplate,
                                        uName,
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
                                if (userDetails.PhoneNumber != null)
                                {
                                    //Send SMS
                                    try
                                    {
                                        Int64? numberToSend = Int64.Parse(phoneNumber);
                                        //string msg = "Your Millionlights registration is successful. Loginid is " + user.EmailId + " and password is " + userReg.Password;
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
                                                // messageID = resp.Substring(33, 20);
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

                                //New Referral Code for User Registrations
                                CouponGen cc = new CouponGen();
                                ReferralCodes refC = new ReferralCodes();
                                //refC.ReferralCode = cc.GenerateVouchers();
                                refC.ReferralCode = cc.GenerateReferralCodes();
                                refC.Referrer = userId;
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

                                returnMsg = "User registered successfully";
                            }
                            catch (Exception)
                            {
                                returnMsg = "An error has occured while user registration. Please try again..";
                            }
                        }
                        else
                        {
                            returnMsg = "Password and Confirm Password doesnt match";
                        }
                    }
                    else
                    {
                        returnMsg = "Please provide valid data";
                    }
                }
                else
                {
                    returnMsg = "User already exist, Please submit the valid data";
                }
            }
            else
            {
                returnMsg = "Valid EmailId is required for registration. Please provide the required details";
            }
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     returnMsg));
        }

        [Route("api/EnrollCourses")]
        [AcceptVerbs("GET", "POST")]
        public CourseEnrollment EnrollCourses(string checkOutCourses, string orderAmount, string couponCode)
        {
            string returnMsg = string.Empty;
            //if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            //{
                string userEmail = User.Identity.Name;
                User userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    //Save order
                    decimal orderPrice;
                    string orderNumber = null;
                    bool? coupStatus;
                    bool? userCrsExist;
                    CourseEnrollment txn = new CourseEnrollment();
                    SaveOrderDetails(checkOutCourses, userInfo.UserId, orderAmount, couponCode, out orderPrice, out orderNumber, out coupStatus, out userCrsExist);

                    if (coupStatus == false)
                    {
                        returnMsg = "Please check the coupon code is valid and active(unused)";
                        //throw new JsonException("Please check the coupon code is valid and active(unused)");
                    }
                    else if (userCrsExist == true)
                    {
                        returnMsg = "User is already enrolled to one/all of these courses, please check..";
                        //throw new JsonException("User is already enrolled to one/all of these courses, please check..");
                    }
                    else
                    {
                        //Change order status to success
                        returnMsg = "Your order is placed successfully";
                        Orders order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
                        if (order != null)
                        {
                            order.OrderStatusID = 1;
                            order.OrderStatus = "Completed";
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();

                            User user = db.Users.Where(u => u.UserId == order.UserID).FirstOrDefault();
                            string cName = string.Empty;
                            string cNameForEmail = string.Empty;

                            List<ItemsOrdered> io = db.ItemsOrders.Where(x => x.OrderID == order.OrderID).ToList();
                            foreach (var item in io)
                            {
                                var courseId = item.CourseId;
                                UsersCourses userCourse = new UsersCourses();
                                userCourse.UserId = order.UserID;
                                userCourse.CourseID = item.CourseId;
                                userCourse.CreatedOn = DateTime.Now;
                                userCourse.Status = true;
                                if (couponCode != null && couponCode != "" && couponCode != "null")
                                {
                                    userCourse.CouponApplied = couponCode;
                                }
                                userCourse.VoucherAssignedOn = DateTime.Now;
                                userCourse.CanAccessLMS = true;
                                userCourse.CanPay = true;
                                userCourse.CanAccessCertification = true;
                                userCourse.IsActive = true;
                                db.UsersCourses.Add(userCourse);
                                db.SaveChanges();

                                //Prashant Sir
                                //Enroll into LMS
                                //courseAPI.EnrollUser(user.EmailId, course.LMSCourseId);

                                Course course = db.Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                                string viewCourse = "<br />(<a href=" + course.EDXCourseLink + " target='_blank'> View Course </a>)<br />";

                                if (!string.IsNullOrEmpty(cName))
                                {
                                    cName = course.CourseName + ' ' + ',' + ' ' + cName;
                                }
                                else
                                {
                                    cName = course.CourseName;
                                }


                                if (!string.IsNullOrEmpty(cNameForEmail))
                                {
                                    cNameForEmail = course.CourseName + viewCourse + ' ' + ',' + ' ' + cNameForEmail;
                                }
                                else
                                {
                                    cNameForEmail = course.CourseName + viewCourse;
                                }

                                //API - Sergiy
                                //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");
                                string lmsCourseId = GetLMSCourseId(course.EDXCourseLink);
                                if (lmsCourseId != null)
                                {
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
                                    try
                                    {
                                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                        {
                                            //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

                                            //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
                                            string json = "{\"email\":\"" + user.EmailId + "\"," +

                                                          "\"course_id\":\"" + lmsCourseId + "\"}";
                                            streamWriter.Write(json);
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                        }
                                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                        {
                                            var result = streamReader.ReadToEnd();
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }
                                returnMsg = "Your order is executed successfully and the enrolled courses are added to your dashboard";
                            }

                            long orderNo = Convert.ToInt64(order.OrderNumber);
                            var orderDate = order.OrderedDatetime;
                            decimal finalPrice = order.TotalPrice;
                            DateTime orderDateTime = DateTime.Parse(orderDate.ToString());
                            if (order.OrderStatus == "Completed")
                            {
                                OrderSuccessEmail(cName, orderNo, orderDateTime, finalPrice, finalPrice, cNameForEmail, order.UserID.ToString());
                                //CouponSuccessEmail(cName, orderNo, orderDate, finalPrice, finalPrice);
                            }

                            //Check if the Coupon is completely used
                            if (couponCode != null && couponCode != "" && couponCode != "null")
                            {
                                //Coupon
                                var coup = db.Coupons.Where(c => c.CouponCode == couponCode).FirstOrDefault();
                                //Get the user Courses Enrolled
                                var usrCourses = db.UsersCourses.Where(uc => uc.UserId == user.UserId && uc.CouponApplied == couponCode).ToList();
                                if (coup != null)
                                {
                                    UserCoupons userCoupon = db.UserCoupons.Where(uc => uc.CouponId == coup.Id && uc.ActivateBy == user.UserId).FirstOrDefault();
                                    if (userCoupon == null)
                                    {
                                        UserCoupons newUserCoupon = new UserCoupons();
                                        newUserCoupon.ActivateBy = user.UserId;
                                        newUserCoupon.ActivateOn = DateTime.Now;
                                        newUserCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
                                        newUserCoupon.CouponId = coup.Id;
                                        db.UserCoupons.Add(newUserCoupon);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        userCoupon.ActivateOn = DateTime.Now;
                                        userCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
                                        db.Entry(userCoupon).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            txn.OrderID = order.OrderID;
                            txn.OrderNumber = order.OrderNumber;
                            txn.UserID = order.UserID;
                            txn.TotalItems = order.TotalItems;
                            txn.TotalPrice = order.TotalPrice;
                            txn.OrderStatus = order.OrderStatus;
                            txn.OrderedDatetime = order.OrderedDatetime;
                            txn.couponCode = couponCode;
                            txn.enrolledCourses = cName;

                            return txn;
                        }
                    }
                }
                else
                {
                    returnMsg = "User account does not exist.";
                }
            //}
            //else
            //{
            //    returnMsg = "You are not authenticated to access the details. Please login..";
            //}
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                  returnMsg));
          
        }

        private void SaveOrderDetails(string checkOutCourses, int userId, string orderAmount, string couponCode, out decimal orderPrice, out string OrderNumber, out bool? couponStatus, out bool? userCourseExist)
        {
            DateTime convertedDate = DateTime.SpecifyKind(
            DateTime.Parse(DateTime.UtcNow.ToString()),
            DateTimeKind.Local);
            DateTime dt = convertedDate.ToLocalTime();

            couponStatus = null;
            userCourseExist = null;
            orderPrice = decimal.Parse(orderAmount);
            byte[] buffer = Guid.NewGuid().ToByteArray();
            OrderNumber = BitConverter.ToUInt32(buffer, 8).ToString();
            Orders ord = new Orders();
            ord.UserID = userId;
            ord.OrderNumber = OrderNumber;
            string CourseIds = checkOutCourses;
            string[] CourseIdsArray = CourseIds.Split(',');
            ord.TotalItems = CourseIdsArray.Length;

            ord.ShippingAddress1 = null;
            ord.ShippingAddress2 = null;
            ord.ShippingCity = null;
            ord.ShippingCountry = null;
            ord.ShippingState = null;
            ord.ShippingZipCode = null;

            ord.BillingAddress1 = null;
            ord.BillingAddress2 = null;
            ord.BillingCity = null;
            ord.BillingState = null;
            ord.BillingCountry = null;
            ord.BillingZipCode = null;


            ord.OrderStatusID = 2;
            ord.OrderStatus = "Pending";
            ord.OrderedDatetime = dt;
            ord.OrderModifiedOn = dt;
            ord.IsActive = true;
            ord.TotalPrice = orderPrice;
            db.Orders.Add(ord);
            db.SaveChanges();

            for (int i = 0; i < CourseIdsArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(CourseIdsArray[i]))
                {
                    int crsId = int.Parse(CourseIdsArray[i]);
                    //Check user already enrolled
                    var userCourses = db.UsersCourses.Where(X => X.CourseID == crsId && X.UserId == userId).FirstOrDefault();
                    if (userCourses != null)
                    {
                        userCourseExist = true;
                    }
                    else
                    {
                        userCourseExist = false;
                    }

                    var courseById = db.Courses.Find(crsId);

                    //Suraj
                    string cc = string.Empty;
                    Coupon coup = null;
                    decimal effPrice = 0;
                    if (couponCode != null && couponCode != "" && couponCode != "null")
                    {
                        cc = couponCode;
                        coup = db.Coupons.Where(x => x.CouponCode == cc).FirstOrDefault();
                        if (coup != null)
                        {
                            //For Individual Discounted Price
                            effPrice = (courseById.BasePrice) - (courseById.BasePrice * (coup.DiscountPrice / 100));
                            couponStatus = true;

                        }
                        else
                        {
                            couponStatus = false;
                        }
                    }
                    else
                    {
                        effPrice = courseById.BasePrice;
                    }
                    if (couponStatus != false && userCourseExist != true)
                    {
                        //db.Orders.Add(ord);
                        //db.SaveChanges();
                        //Suraj
                        ItemsOrdered itemOrder = new ItemsOrdered();
                        itemOrder.OrderID = ord.OrderID;
                        itemOrder.CourseId = crsId;
                        itemOrder.UnitPrice = effPrice;
                        itemOrder.Quantity = 1;
                        itemOrder.IsActive = true;
                        db.ItemsOrders.Add(itemOrder);
                        db.SaveChanges();
                    }
                }
            }

        }
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

        //New APIs Developed as per the requirements

        [Route("api/GetAllPublicCourses")]
        //PublicCoursesModel
        public List<PublicCoursesDetailsModel> GetAllPublicCourses()
        {
            List<PublicCoursesDetailsModel> userCourse = new List<PublicCoursesDetailsModel>();

            List<Course> courseList = db.Courses.Where(x => x.IsActive == true).ToList();
            if (courseList != null)
            {
                foreach (var course in courseList)
                {
                    //string imgUrl = "https://www.millionlights.org/Images/CourseImg/{0}/{1}";
                    string imgUrl = HttpContext.Current.Request.Url.Host + "/Images/CourseImg/{0}/{1}";
                    PublicCoursesDetailsModel courseViewModel = new PublicCoursesDetailsModel();
                    courseViewModel.Id = course.Id;
                    courseViewModel.CourseCode = course.CourseCode;
                    courseViewModel.CourseName = course.CourseName;
                    courseViewModel.ShortDescription = course.ShortDescription;
                    courseViewModel.EDXCourseLink = course.EDXCourseLink;
                    courseViewModel.LongDescription = course.LongDescription;
                    courseViewModel.Objective = course.Objective;
                    courseViewModel.ExamObjective = course.ExamObjective;
                    courseViewModel.StartDate = course.StartDate;
                    courseViewModel.EndDate = course.EndDate;
                    courseViewModel.CreatedOn = course.CreatedOn;
                    courseViewModel.BasePrice = course.BasePrice;
                    //courseViewModel.LMSCourseId = course.LMSCourseId;changed to below as per Srini
                    courseViewModel.LMSCourseId = GetLMSCourseId(course.EDXCourseLink);
                    courseViewModel.Instructor = course.Instructor;
                    courseViewModel.CourseImageURL = string.Format(imgUrl, course.Id, course.CourseImageLink);
                    userCourse.Add(courseViewModel);
                }
                return userCourse;
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "Sorry! No Courses are found"));
            }
        }
        [Route("api/GetPublicCourseDetails")]
        public List<PublicCoursesDetailsModel> GetPublicCourseDetails(string lmsCourseId)
        {
            string returnMsg = string.Empty;
            if (lmsCourseId != null && lmsCourseId != "")
            {
                List<PublicCoursesDetailsModel> userCourse = null;
                int lastSlash = lmsCourseId.LastIndexOf('/');
                string LMSCourseId = (lastSlash > -1) ? lmsCourseId.Substring(0, lastSlash) : lmsCourseId;
                string[] splitLMSIDString = LMSCourseId.Split('/');
                var message = string.Format("The course with this LMSID is not found. Please check the LMS Course ID.");

                if (splitLMSIDString.Length > 1 && (splitLMSIDString[1] != null || splitLMSIDString[1] != ""))
                {
                    userCourse = new List<PublicCoursesDetailsModel>();
                    PublicCoursesDetailsModel userCourseViewModel = new PublicCoursesDetailsModel();
                    string lmsCourseID = splitLMSIDString[1];
                    //string imgUrl = "https://www.millionlights.org/Images/CourseImg/{0}/{1}";
                    string imgUrl = HttpContext.Current.Request.Url.Host + "/Images/CourseImg/{0}/{1}";
                    var availcourse = db.Courses.Where(x => x.LMSCourseId == lmsCourseID).Select(x => new PublicCoursesDetailsModel
                    {
                        Id = x.Id,
                        CourseCode = x.CourseCode,
                        CourseName = x.CourseName,
                        CourseImageURL = x.CourseImageLink,
                        ShortDescription = x.ShortDescription,
                        LongDescription = x.LongDescription,
                        Objective = x.Objective,
                        ExamObjective = x.ExamObjective,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        CreatedOn = x.CreatedOn,
                        BasePrice = x.BasePrice,
                        LMSCourseId = x.LMSCourseId,
                        //LMSCourseId = GetLMSCourseId(x.EDXCourseLink),
                        Instructor = x.Instructor,
                        EDXCourseLink = x.EDXCourseLink,
                        CourseStructure = null
                    }).FirstOrDefault();
                    if (availcourse != null)
                    {
                        string[] edxLinksArr = availcourse.EDXCourseLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);
                        if (edxLinksArr.Length > 1)
                        {
                            string LMSCourseId2 = string.Empty;
                            if (edxLinksArr[1].EndsWith("/"))
                            {
                                LMSCourseId2 = edxLinksArr[1].Substring(0, edxLinksArr[1].Length - 1);
                                if (lmsCourseId.EndsWith("/"))
                                {
                                    lmsCourseId = lmsCourseId.Substring(0, lmsCourseId.Length - 1);
                                }
                                //string LMSCourseId2 = (lastSlash > -1) ? edxLinksArr[1].Substring(0, lastSlash) : edxLinksArr[1];
                                if (LMSCourseId2.ToLower() == lmsCourseId.ToLower())
                                {
                                    userCourseViewModel.Id = availcourse.Id;
                                    userCourseViewModel.CourseCode = availcourse.CourseCode;
                                    userCourseViewModel.CourseName = availcourse.CourseName;
                                    userCourseViewModel.CourseImageURL = string.Format(imgUrl, availcourse.Id, availcourse.CourseImageURL);
                                    userCourseViewModel.ShortDescription = availcourse.ShortDescription;
                                    userCourseViewModel.LongDescription = availcourse.LongDescription;
                                    userCourseViewModel.Objective = availcourse.Objective;
                                    userCourseViewModel.ExamObjective = availcourse.ExamObjective;
                                    userCourseViewModel.StartDate = availcourse.StartDate;
                                    userCourseViewModel.EndDate = availcourse.EndDate;
                                    userCourseViewModel.CreatedOn = availcourse.CreatedOn;
                                    userCourseViewModel.BasePrice = availcourse.BasePrice;
                                    //userCourseViewModel.LMSCourseId = availcourse.LMSCourseId;changed to below as per Srini
                                    userCourseViewModel.LMSCourseId = GetLMSCourseId(availcourse.EDXCourseLink);
                                    userCourseViewModel.Instructor = availcourse.Instructor;
                                    userCourseViewModel.EDXCourseLink = availcourse.EDXCourseLink;
                                    userCourseViewModel.CourseStructure = null;
                                    userCourse.Add(userCourseViewModel);
                                    return userCourse;
                                }
                                else
                                {
                                    //return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                    returnMsg = message;
                                }
                            }
                            else
                            {
                                LMSCourseId2 = edxLinksArr[1];
                                if (lmsCourseId.EndsWith("/"))
                                {
                                    lmsCourseId = lmsCourseId.Substring(0, lmsCourseId.Length - 1);
                                }
                                if (LMSCourseId2.ToLower() == lmsCourseId.ToLower())
                                {
                                    userCourseViewModel.Id = availcourse.Id;
                                    userCourseViewModel.CourseCode = availcourse.CourseCode;
                                    userCourseViewModel.CourseName = availcourse.CourseName;
                                    userCourseViewModel.CourseImageURL = string.Format(imgUrl, availcourse.Id, availcourse.CourseImageURL);
                                    userCourseViewModel.ShortDescription = availcourse.ShortDescription;
                                    userCourseViewModel.LongDescription = availcourse.LongDescription;
                                    userCourseViewModel.Objective = availcourse.Objective;
                                    userCourseViewModel.ExamObjective = availcourse.ExamObjective;
                                    userCourseViewModel.StartDate = availcourse.StartDate;
                                    userCourseViewModel.EndDate = availcourse.EndDate;
                                    userCourseViewModel.CreatedOn = availcourse.CreatedOn;
                                    userCourseViewModel.BasePrice = availcourse.BasePrice;
                                    //userCourseViewModel.LMSCourseId = availcourse.LMSCourseId;changed to below as per Srini
                                    userCourseViewModel.LMSCourseId = GetLMSCourseId(availcourse.EDXCourseLink);
                                    userCourseViewModel.Instructor = availcourse.Instructor;
                                    userCourseViewModel.EDXCourseLink = availcourse.EDXCourseLink;
                                    userCourseViewModel.CourseStructure = null;
                                    userCourse.Add(userCourseViewModel);
                                    return userCourse;
                                }
                                else
                                {
                                    returnMsg = message;
                                }
                            }
                        }
                        else
                        {
                            returnMsg = message;
                        }
                    }
                    else
                    {
                        returnMsg = message;
                    }
                }
                else
                {
                    returnMsg = message;
                }
            }
            else
            {
                returnMsg = "Sorry! No Courses are found";
            }
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 returnMsg));
        }

        [Route("api/GetCategoryList")]
        public List<CourseCategoriesList> GetCategoryList()
        {
            List<CourseCategoriesList> categories = new List<CourseCategoriesList>();

            List<CourseCategory> courseCatList = db.CourseCategories.Where(x => x.IsActive == true).ToList();
            if (courseCatList != null)
            {
                foreach (var course in courseCatList)
                {
                    CourseCategoriesList courseCat = new CourseCategoriesList();
                    courseCat.Id = course.Id;
                    courseCat.Name = course.Name;
                    categories.Add(courseCat);
                }
                return categories;
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "Sorry! No categories are found"));
            }
        }
        [Route("api/GetCourseListByCatId")]
        public List<CourseListByCategoryId> GetAllCategories(string catId)
        {
            List<CourseListByCategoryId> categories = new List<CourseListByCategoryId>();
            if (catId != null)
            {
                string[] cats = catId.Split(',');

                var list = new List<string>();
                for (int i = 0; i < cats.Count(); i++)
                {
                    list.Add(cats[i]);
                }

                List<Course> courseList = new List<Course>();
                for (int i = 0; i < cats.Count(); i++)
                {
                    
                    var catid = cats[i];
                    List<Course> courseLista = db.Courses.Where(x => x.IsActive == true && x.CourseCategory.Contains(catid)).ToList();
                    courseList.AddRange(courseLista);
                }

                courseList = (from mci in courseList select mci).Distinct().ToList();
                 
                    if (courseList.Count() > 0)
                    {
                        foreach (var course in courseList)
                        {
                            //string imgUrl = "https://www.millionlights.org/Images/CourseImg/{0}/{1}";
                            string imgUrl = HttpContext.Current.Request.Url.Host + "Images/CourseImg/{0}/{1}";
                            CourseListByCategoryId courseCat = new CourseListByCategoryId();
                            courseCat.Id = course.Id;
                            courseCat.CourseCode = course.CourseCode;
                            courseCat.CourseName = course.CourseName;
                            courseCat.CourseImageURL = string.Format(imgUrl, course.Id, course.CourseImageLink);
                            courseCat.ShortDescription = course.ShortDescription;
                            courseCat.LMSCourseId = GetLMSCourseId(course.EDXCourseLink);
                            courseCat.EDXCourseLink = course.EDXCourseLink;
                            categories.Add(courseCat);
                        }
                        return categories;
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                        "Course(s) not found."));
                    }
                }
            
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                "Please provide the category Id"));
            }
        }


        //http://localhost:8094/api/UpdateCourseStatus?email=priyanka@ncorpuscle.com&courseId=129&status=Completed
        [Route("api/UpdateCourseStatus")]
        [AcceptVerbs("GET", "POST")]
        //public void UpdateCourseStatus(int courseId, string status)
        //http://localhost:8094/api/UpdateCourseStatus?organization=DartmouthX&courseNumber=EVS2015&courseRun=2015&status=Completed
        //public void UpdateCourseStatus(string organization, string courseNumber, string courseRun,  string status)
        //http://localhost:8094/api/UpdateCourseStatus?EdxCompleteCourseId=DartmouthX/EVS2015/2015&status=Completed
        public void UpdateCourseStatus(string EdxCompleteCourseId, string status)
        {
            //string edxCourseId = organization + "/" + courseNumber + "/" + courseRun;
            string edxCourseId = EdxCompleteCourseId;
            var courseDet = db.Courses.Where(e => e.IsActive == true && e.EDXCourseLink.Contains(edxCourseId)).FirstOrDefault();
            var test = courseDet;

            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                 //string userEmailId = "priyanka.rangari@ncorpuscle.com";
                string userEmailId = User.Identity.Name;
                var userInfo = db.Users.Join(db.UsersDetails, u => u.UserId, ud => ud.UserId, (u, ud) => new { u, ud })
                                .Where(x => x.u.EmailId == userEmailId).FirstOrDefault();
                if (userInfo != null)
                {
                    int userId = userInfo.u.UserId;

                    if (courseDet != null)
                    {
                        var course = db.UsersCourses.Where(x => x.UserId == userId && x.CourseID == courseDet.Id).FirstOrDefault();
                        if (course != null)
                        {
                            course.CourseStatus = status;
                            db.Entry(course).State = EntityState.Modified;
                            db.SaveChanges();

                            if (status == "Completed" && courseDet.EnableForCertification == true)
                            {
                                //Create Html certificate to PDF
                                string regTemplate = string.Empty;

                                if (!string.IsNullOrEmpty(courseDet.CertificateTemplateHtmFile))
                                {
                                    string pathTemp = string.Format("../Images/Certificate/HtmlTemplate/{0}", courseDet.Id);
                                    regTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(pathTemp), courseDet.CertificateTemplateHtmFile);
                                }
                                else
                                {
                                    regTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "MLCertificate.htm"); 
                                }
                                Millionlights.EmailService.MillionLightsEmails emails = new Millionlights.EmailService.MillionLightsEmails();
                                var certificateTemp = emails.ReadTemplate(regTemplate);
                                //StudentName
                                var fullname = userInfo.ud.FirstName + " " + userInfo.ud.LastName;
                                certificateTemp = certificateTemp.Replace("[StudentName]", fullname);
                                //CourseName
                                //var courseName = db.Courses.Where(x => x.Id == courseId).FirstOrDefault();
                                certificateTemp = certificateTemp.Replace("[CourseName]", courseDet.CourseName);

                                certificateTemp = certificateTemp.Replace("[Date]", DateTime.Now.ToString("dd-MMM-yyyy"));

                                string imageFilePath = string.Empty;
                                string imageTemplatePath = string.Empty;
                                // Attached the Certificate Template

                                if (!string.IsNullOrEmpty(courseDet.CertificateTemplate))
                                {
                                    imageTemplatePath = string.Format("../Images/Certificate/Template/{0}/{1}" , courseDet.Id, courseDet.CertificateTemplate);
                                    Uri currentUrlTemplate1 = new Uri(HttpContext.Current.Request.Url, imageTemplatePath );
                                    certificateTemp = certificateTemp.Replace("[Template]", currentUrlTemplate1.AbsoluteUri);
                                    imageFilePath = currentUrlTemplate1.AbsoluteUri;
                                }
                                else
                                {
                                    //imageTemplatePath = "https://www.millionlights.org/Images/ML_Certificate.png";
                                    imageTemplatePath = "../Images/ML_Certificate.jpg";

                                    //imageTemplatePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images"), "ML_Certificate.jpg"); 

                                    Uri currentUrlTemplate2 = new Uri(HttpContext.Current.Request.Url, imageTemplatePath);
                                    certificateTemp = certificateTemp.Replace("[Template]", currentUrlTemplate2.AbsoluteUri);
                                    imageFilePath = currentUrlTemplate2.AbsoluteUri;
                                }

                                // Attached the Certificate Logo
                              

                                if (!string.IsNullOrEmpty(courseDet.CertificateLogo))
                                {
                                    Uri currentUrl1 = new Uri(HttpContext.Current.Request.Url, string.Format("../Images/Certificate/Logo/{0}/{1}" , courseDet.Id, courseDet.CertificateLogo));
                                    certificateTemp = certificateTemp.Replace("[Logo]", currentUrl1.AbsoluteUri);
                                }
                                else
                                {
                                    Uri currentUrl2 = new Uri(HttpContext.Current.Request.Url, "../Images/Millionlights.png");
                                   
                                    certificateTemp = certificateTemp.Replace("[Logo]", currentUrl2.AbsoluteUri);
                                }
                               
                                // Attached the Certificate Signature

                                //Uri currentUrlSignature = new Uri(HttpContext.Current.Request.Url, "../Images/Millionlights.png");
                                //certificateTemp = certificateTemp.Replace("", currentUrlSignature.AbsoluteUri);

                                if (!string.IsNullOrEmpty(courseDet.CertificateSignature))
                                {
                                    Uri currentUrlSignature = new Uri(HttpContext.Current.Request.Url,string.Format("../Images/Certificate/Signature/{0}/{1}" , courseDet.Id, courseDet.CertificateSignature));
                                    certificateTemp = certificateTemp.Replace("[Signature]", currentUrlSignature.AbsoluteUri);
                                }
                                else 
                                {
                                    Uri currentUrlSignature = new Uri(HttpContext.Current.Request.Url, string.Format("../Images/Signature.jpg"));
                                    certificateTemp = certificateTemp.Replace("[Signature]", currentUrlSignature.AbsoluteUri);
                                }
                                // Create a Document object
                                var document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                                document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                                // Create a new PdfWrite object, writing the output to a MemoryStream
                                Random rnd = new Random();
                                int myRandomNo = rnd.Next(10000000, 99999999);
                                var pdfName = userId.ToString() + myRandomNo.ToString();
                                var path = System.Web.HttpContext.Current.Server.MapPath("~/Certificates/");
                                var directory = new DirectoryInfo(path);
                                if (directory.Exists == false)
                                {
                                    directory.Create();
                                }
                                var output = new FileStream(path + "/" + pdfName + ".pdf", FileMode.Create, FileAccess.ReadWrite);
                                var writer = PdfWriter.GetInstance(document, output);

                                // Open the Document for writing
                                document.Open();

                                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
                                jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                                jpg.ScaleToFit(1470,630);
                                jpg.SetAbsolutePosition(20f, 0);
                                document.Add(jpg);

                                // convert the HTML File into PDF using XMLWorkerHelper

                                //iTextSharp.text.xml.simpleparser.SimpleXMLParser.Parse()

                                //List<IElement> ie1 = iTextSharp.text.xml.simpleparser.SimpleXMLParser.Parse();  //XMLWorker.parseToElementList(certificateTemp, null);

                                //StyleSheet styles = new StyleSheet ();
                                //styles.LoadTagStyle("span", "font-family", "Old English Text MT");

                                //string css = styles.ToString();

                                //var  cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css));

                                //var worker1 = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml‌(writer, document, certificateTemp, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css))); 

                                //XMLWorkerHelper.getInstance().parseXHtml(writer, document, new FileInputStream("/html/loremipsum.html"));
                                //List<IElement> ie1 = XMLWorkerHelper.GetInstance().ParseXHtml(writer, document,new FileInputStream);
                               
                                List<IElement> ie = HTMLWorker.ParseToList(new StringReader(certificateTemp), null);
                                foreach (IElement element in ie)
                                {
                                    document.Add(element);
                                }
                                document.Close();

                                //Save details to UsersCertificate
                                var count = db.UsersCertificate.Where(x => x.UserID == userId && x.CourseID == courseDet.Id).Count();
                                if (count == 0)
                                {

                                    UsersCertificate certificates = new UsersCertificate();
                                    certificates.UserID = userId;
                                    certificates.CourseID = courseDet.Id;
                                    certificates.CertificationID = pdfName;
                                    certificates.CertificatePath = "/Certificates/" + pdfName + ".pdf";
                                    certificates.IssuedDate = DateTime.Now;
                                    db.UsersCertificate.Add(certificates);
                                    db.SaveChanges();

                                    //Send email with attachment
                                    string cerTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "UsersCertification.html");

                                    MillionLightsEmails mEmail = new MillionLightsEmails();
                                    mEmail.SendCertificationEmail(
                                        ConfigurationManager.AppSettings["SenderName"],
                                        ConfigurationManager.AppSettings["SenderEmail"],
                                            ConfigurationManager.AppSettings["Telephone"],
                                            ConfigurationManager.AppSettings["EmailId"],
                                        "Your Millionlights Certification",
                                        cerTemplate,
                                        userInfo.u.EmailId,
                                        System.Web.HttpContext.Current.Server.MapPath("../Certificates/" + pdfName + ".pdf"),
                                        pdfName + ".pdf",
                                        fullname,
                                        courseDet.CourseName,
                                        certificates.PdfPath,
                                        string.Format(LinkedInUrl, certificates.CourseNameEncode, certificates.CertificationID, "1234546", certificates.CertiStartDateString)
                                        );

                                    UserCertificateEvidenceDetails evidence = new UserCertificateEvidenceDetails();
                                    evidence.CourseId = courseDet.Id;
                                    evidence.UsersCertificateId = certificates.Id;
                                    evidence.EvidenceId = null;
                                    evidence.FirstName = null;
                                    evidence.LastName = null;
                                    evidence.Address = null;
                                    evidence.EvidenceExpiry = null;
                                    evidence.EvidenceNumber = null;
                                    evidence.ImageUrl = null;
                                    evidence.EvidenceUploadedPath = null;
                                    evidence.EvidenceIssueDate = null;
                                    evidence.IsUploaded = false;
                                    evidence.IsActive = false;
                                    db.UserCertificateEvidenceDetails.Add(evidence);
                                    db.SaveChanges();

                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                                                        "Certificate is generated successfully and email is sent to the respective users"));

                                }
                                else
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                        "Certificate is already issued to the user"));
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                        "The course status is not completed or the course has disabled the certification - please check or contact ML support team."));
                            }
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                       "You haven't enrolled this course. Please check the details."));
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                   "Course not found. Please check the input parameters."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User details are not found. Please check and try again.."));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("{certificationID:long}")]
        [AcceptVerbs("GET", "POST")]
        public System.Web.Http.Results.RedirectResult ViewCertificate(long certificationID)
        {
            if (certificationID != 0)
            {
                var details = db.UsersCertificate.Where(x => x.CertificationID == certificationID.ToString()).FirstOrDefault();
                if (details != null)
                {
                    return Redirect(details.PdfPath);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }


        //New Import Changes (Template and Selected Courses)
        [Route("api/SendEmailAsynch")]
        [AcceptVerbs("GET", "POST")]
        public void SendEmailAsynch(List<BatchExecuteUserDataResult> finalUserList, string partnerName, string selectedCourseIds, string isNewTemplateUploaded, string emailTempPath)
        {
            UserRegisterController userRegisterController = new UserRegisterController();
            foreach (var item in finalUserList)
            {
                //Send Email
                try
                {
                    string userName = item.FirstName + " " + item.LastName;

                    //string regTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/ImportUsersRegistrationEmail.htm");
                    string regTemplate = null;
                    if (isNewTemplateUploaded == "YES" && emailTempPath!=null)
                    {
                        regTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, emailTempPath);
                    }
                    else{
                        regTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/ImportUsersRegistrationEmail.htm");
                    }

                    string decryptedPass = userRegisterController.passwordDecrypt(item.Password);

                    string courseIdsList = "";
                    if (selectedCourseIds!=null)
                    {
                        courseIdsList = selectedCourseIds;
                    }
                    else
                    {
                        //courseIdsList = "17;32";
                        courseIdsList = "";
                    }
                    //Test
                    //string courseIdsList = "129;131";
                    string courseNameForEmail = "";
                    string courseCount = "0";
                    if (courseIdsList != "")
                    {
                        string[] courseIdsArray = courseIdsList.Split(';');
                        courseCount = courseIdsArray.Count().ToString();
                        for (int i = 0; i < courseIdsArray.Count(); i++)
                        {
                           
                            int cId = int.Parse(courseIdsArray[i]);
                            var course = db.Courses.Where(x => x.Id == cId).FirstOrDefault();
                            if (course != null)
                            {
                                if (courseNameForEmail != "")
                                {
                                    courseNameForEmail = "<li>" + course.CourseName + "</li>" + courseNameForEmail;
                                }
                                else
                                {
                                    courseNameForEmail = "<li>" + course.CourseName + "</li>";
                                }
                                UsersCourses userCourse = new UsersCourses();
                                userCourse.UserId = item.UserId;
                                userCourse.CourseID = course.Id;
                                userCourse.CreatedOn = DateTime.Now;
                                userCourse.Status = true;
                                userCourse.VoucherAssignedOn = DateTime.Now;
                                userCourse.CanAccessLMS = true;
                                userCourse.CanPay = true;
                                userCourse.CanAccessCertification = true;
                                userCourse.IsActive = true;
                                db.UsersCourses.Add(userCourse);
                                db.SaveChanges();

                                //API - Sergiy
                                //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");

                                string lmsCourseId = GetLMSCourseId(course.EDXCourseLink);
                                if (lmsCourseId != null)
                                {
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
                                    try
                                    {
                                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                        {
                                            //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

                                            //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
                                            string json = "{\"email\":\"" + item.EmailId + "\"," +

                                                          "\"course_id\":\"" + lmsCourseId + "\"}";
                                            streamWriter.Write(json);
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                        }
                                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                        {
                                            var result = streamReader.ReadToEnd();
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }
                            }
                        }
                    }
                    

                    //Send Email and SMS
                    if (item.EmailId != null)
                    {
                        MillionLightsEmails mEmail = new MillionLightsEmails();
                        mEmail.SendNewImportUserRegistrationCompleteEmail(
                            ConfigurationManager.AppSettings["SenderName"],
                            ConfigurationManager.AppSettings["SenderEmail"],
                             ConfigurationManager.AppSettings["Telephone"],
                              ConfigurationManager.AppSettings["EmailId"],
                            "Welcome to Millionlights",
                            regTemplate,
                            userName,
                            item.EmailId,
                            decryptedPass,
                            partnerName,
                            courseCount,
                            //courseIdsArray.Count().ToString(),
                            courseNameForEmail
                            );
                    }
                    Int64? phNumber = null;
                    if (item.PhoneNumber != null)
                    {
                        phNumber = Int64.Parse(item.PhoneNumber);
                    }
                    if (phNumber != null && phNumber != 0)
                    {
                        //Send SMS
                        try
                        {
                            Int64? numberToSend = phNumber;
                            string msg = String.Format(Millionlights.Models.Constants.UserRegistrationSuccessMsg, item.EmailId, decryptedPass);
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
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                //New Referral Code for User Registrations
                CouponGen cc = new CouponGen();
                ReferralCodes refC = new ReferralCodes();
                //refC.ReferralCode = cc.GenerateVouchers();
                refC.ReferralCode = cc.GenerateReferralCodes();
                refC.Referrer = item.UserId;
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
            }
        }


        //[Route("api/SendEmailAsynch")]
        //[AcceptVerbs("GET", "POST")]
        //public void SendEmailAsynch(List<BatchExecuteUserDataResult> finalUserList, string partnerName)
        //{
        //    UserRegisterController userRegisterController = new UserRegisterController();
        //    foreach (var item in finalUserList)
        //    {
        //        //Send Email
        //        try
        //        {
        //            string userName = item.FirstName + " " + item.LastName;
        //            string regTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/ImportUsersRegistrationEmail.htm");
        //            string decryptedPass = userRegisterController.passwordDecrypt(item.Password);
        //            int? NotificationId = null;
        //            if (item.EmailId != null)
        //            {
        //                MillionLightsEmails mEmail = new MillionLightsEmails();
        //                mEmail.SendImportUserRegistrationCompleteEmail(
        //                    ConfigurationManager.AppSettings["SenderName"],
        //                    ConfigurationManager.AppSettings["SenderEmail"],
        //                     ConfigurationManager.AppSettings["Telephone"],
        //                      ConfigurationManager.AppSettings["EmailId"],
        //                    "Welcome to Millionlights",
        //                    regTemplate,
        //                    userName,
        //                    item.EmailId,
        //                    decryptedPass,
        //                    partnerName
        //                    );
        //                //email notification

        //                //UserNotitification userNotification = new UserNotitification();
        //                //userNotification.Receiver = item.UserId;
        //                //userNotification.Sender = "System";
        //                //userNotification.Subject = Millionlights.Models.Constants.UserRegSubNotification;
        //                //userNotification.Message = String.Format(Millionlights.Models.Constants.UserRegMsgNotification, item.EmailId, decryptedPass);
        //                //userNotification.IsAlert = false;
        //                //userNotification.DateSent = DateTime.Now;
        //                //userNotification.ReadDate = null;
        //                //userNotification.SMSDate = null;
        //                //userNotification.MailDate = DateTime.Now;
        //                //userNotification.NotificationStatusId = 2;
        //                //db.UserNotitifications.Add(userNotification);
        //                //db.SaveChanges();
        //                //NotificationId = userNotification.Id;
        //            }
        //            //if (item.ud.PhoneNumber != null && item.ud.PhoneNumber != "null" && item.ud.PhoneNumber != "NULL" && item.ud.PhoneNumber != "" && item.ud.PhoneNumber != "Null")
        //            Int64? phNumber = null;
        //            if (item.PhoneNumber != null)
        //            {
        //                phNumber = Int64.Parse(item.PhoneNumber);
        //            }
        //            if (phNumber != null && phNumber != 0)
        //            {
        //                //Send SMS
        //                try
        //                {
        //                    Int64? numberToSend = phNumber;
        //                    //string msg = "Your Millionlights registration is successful. Loginid is " + user.EmailId + " and password is " + userReg.Password;
        //                    string msg = String.Format(Millionlights.Models.Constants.UserRegistrationSuccessMsg, item.EmailId, decryptedPass);
        //                    string url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
        //                    string smsRequestUrl = string.Format(url, numberToSend, msg);
        //                    HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
        //                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //                    {
        //                        if (response.StatusCode.Equals(HttpStatusCode.OK))
        //                        {
        //                            StreamReader responseStream = new StreamReader(response.GetResponseStream());
        //                            string resp = responseStream.ReadLine();
        //                            // messageID = resp.Substring(33, 20);
        //                        }
        //                    }

        //                    //  sms notification
        //                    //if (NotificationId != null)
        //                    //{
        //                    //    UserNotitification userNotificationForSms = db.UserNotitifications.Find(NotificationId);
        //                    //    userNotificationForSms.SMSDate = DateTime.Now;
        //                    //    userNotificationForSms.NotificationStatusId = 2;
        //                    //    db.Entry(userNotificationForSms).State = EntityState.Modified;
        //                    //    db.SaveChanges();
        //                    //}
        //                }
        //                catch (Exception)
        //                {
        //                    continue;
        //                }
        //            }
        //            //New Requirement - 
        //            //Registered the imported user directly &
        //            //Enrolled them to a few courses
        //            //var list = new List<Tuple<int, int>>();
        //            //list.Add(new Tuple<int, int>(17, 32));
        //            //Azure
        //            string courseIdsList = "17;32";
        //            //Test
        //            //string courseIdsList = "129;131";
        //            string[] courseIdsArray = courseIdsList.Split(';');
        //            for (int i = 0; i < courseIdsArray.Count(); i++)
        //            {
        //                int cId = int.Parse(courseIdsArray[i]);
        //                var course = db.Courses.Where(x => x.Id == cId).FirstOrDefault();
        //                if (course != null)
        //                {
        //                    UsersCourses userCourse = new UsersCourses();
        //                    userCourse.UserId = item.UserId;
        //                    userCourse.CourseID = course.Id;
        //                    userCourse.CreatedOn = DateTime.Now;
        //                    userCourse.Status = true;
        //                    userCourse.VoucherAssignedOn = DateTime.Now;
        //                    userCourse.CanAccessLMS = true;
        //                    userCourse.CanPay = true;
        //                    userCourse.CanAccessCertification = true;
        //                    userCourse.IsActive = true;
        //                    db.UsersCourses.Add(userCourse);
        //                    db.SaveChanges();

        //                    //API - Sergiy
        //                    //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");

        //                    string lmsCourseId = GetLMSCourseId(course.EDXCourseLink);
        //                    if (lmsCourseId != null)
        //                    {
        //                        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
        //                        httpWebRequest.ContentType = "application/json";
        //                        httpWebRequest.Method = "POST";
        //                        httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
        //                        try
        //                        {
        //                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //                            {
        //                                //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

        //                                //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
        //                                string json = "{\"email\":\"" + item.EmailId + "\"," +

        //                                              "\"course_id\":\"" + lmsCourseId + "\"}";
        //                                streamWriter.Write(json);
        //                                streamWriter.Flush();
        //                                streamWriter.Close();
        //                            }
        //                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //                            {
        //                                var result = streamReader.ReadToEnd();
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            continue;
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            continue;
        //        }
        //    }
        //}

        //New Changes as per the meeting held on 26feb
        [Route("api/GetCoursesByCourseName")]
        [AcceptVerbs("GET", "POST")]
        //PublicCoursesModel
        public List<PublicCoursesDetailsModel> GetCoursesByCourseName(string courseName)
        {
            if (courseName != null && courseName != "")
            {
                List<PublicCoursesDetailsModel> userCourse = new List<PublicCoursesDetailsModel>();

                List<Course> courseList = db.Courses.Where(x => x.IsActive == true && x.CourseName == courseName).ToList();
                if (courseList != null)
                {
                    foreach (var course in courseList)
                    {
                        //string imgUrl = "https://www.millionlights.org/Images/CourseImg/{0}/{1}";
                        string imgUrl = HttpContext.Current.Request.Url.Host + "/Images/CourseImg/{0}/{1}";
                        PublicCoursesDetailsModel courseViewModel = new PublicCoursesDetailsModel();
                        courseViewModel.Id = course.Id;
                        courseViewModel.CourseCode = course.CourseCode;
                        courseViewModel.CourseName = course.CourseName;
                        courseViewModel.ShortDescription = course.ShortDescription;
                        courseViewModel.EDXCourseLink = course.EDXCourseLink;
                        courseViewModel.LongDescription = course.LongDescription;
                        courseViewModel.Objective = course.Objective;
                        courseViewModel.ExamObjective = course.ExamObjective;
                        courseViewModel.StartDate = course.StartDate;
                        courseViewModel.EndDate = course.EndDate;
                        courseViewModel.CreatedOn = course.CreatedOn;
                        courseViewModel.BasePrice = course.BasePrice;
                        //courseViewModel.LMSCourseId = course.LMSCourseId;changed to below as per Srini
                        courseViewModel.LMSCourseId = GetLMSCourseId(course.EDXCourseLink);
                        courseViewModel.Instructor = course.Instructor;
                        courseViewModel.CourseImageURL = string.Format(imgUrl, course.Id, course.CourseImageLink);
                        userCourse.Add(courseViewModel);
                    }
                    return userCourse;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "Sorry! No Courses are found"));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                 "Please provide course name"));
            }
        }

        [Route("api/IsUserEnrolledToCourse")]
        [AcceptVerbs("GET", "POST")]
        public string IsUserEnrolledToCourse(string courseId)
        {
            string returnMsg = string.Empty;
            if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                string userEmail = User.Identity.Name;
                User userInfo = db.Users.Where(u => u.EmailId == userEmail).FirstOrDefault();
                if (userInfo != null)
                {
                    int cId = int.Parse(courseId);
                    var userCourses = db.UsersCourses.Where(X => X.CourseID == cId && X.UserId == userInfo.UserId).FirstOrDefault();
                    if (userCourses != null)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "The course is already enrolled by you. Please check your dashboard."));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "The course is not enrolled by you."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                      "User account does not exist."));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                     "You are not authenticated to access the details. Please login.."));
            }
        }

        [Route("api/SendResetPasswordLink")]
        [AcceptVerbs("GET", "POST")]
        public string SendResetPasswordLink(string emailId)
        {
            string returnMsg = string.Empty;
            SendResetPasswordLinkModel model = new SendResetPasswordLinkModel();
            if (emailId != null && emailId != "")
            {
                var userDetails = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(x => x.a.EmailId == emailId).FirstOrDefault();
                if (userDetails != null)
                {
                    if (userDetails.a.Password != null)
                    {
                        string recipientsEmail = userDetails.a.EmailId;
                        string userPwd = userDetails.a.Password;
                        string fName = userDetails.b.FirstName;
                        string lName = userDetails.b.LastName;
                        var userName = fName + " " + lName;

                        int? NotificationId = null;

                        if (recipientsEmail != null)
                        {
                            try
                            {
                                PasswordResetRequest resetReq = new PasswordResetRequest();
                                resetReq.UserID = userDetails.a.UserId;
                                resetReq.VerificationId = Guid.NewGuid().ToString();
                                resetReq.IsPasswordReset = false;
                                resetReq.PasswordResetRequestDateTime = DateTime.Now;
                                resetReq.PasswordResetDateTime = null;
                                db.PasswordResetRequest.Add(resetReq);
                                db.SaveChanges();

                                string regTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/ResetPassword.html");

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
                                userNotification.Receiver = userDetails.a.UserId;
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

                                returnMsg = "Reset password link is sent to your email address  " + recipientsEmail + " . Please check your email account.";
                                model.Message = returnMsg;
                                model.Success = true;
                            }
                            catch (Exception)
                            {
                                returnMsg = "An exception is occured while sending reset password email to user.";
                                model.Message = returnMsg;
                                model.Success = false;
                            }
                        }
                        else
                        {
                            returnMsg = "We dont find users email address to send the reset password link.";
                            model.Message = returnMsg;
                            model.Success = false;
                        }
                        if (userDetails.b.PhoneNumber != null)
                        {
                            //Send SMS
                            try
                            {
                                Int64? numberToSend = userDetails.b.PhoneNumber;
                                //string msg = "Your password has been changed successfully. Your LoginId is " + userDetails[0].a.EmailId + " and password is " + userDetails[0].a.Password;
                                string msg = String.Format(Millionlights.Models.Constants.ForgotPasswordTextMsg, userDetails.a.EmailId);
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
                                returnMsg = "An exception is occured while sending reset password text to user.";
                                model.Message = returnMsg;
                                model.Success = false;
                            }
                        }
                        else
                        {
                            returnMsg = "We dont find users phone number to send the reset password link.";
                            model.Message = returnMsg;
                            model.Success = false;
                        }
                    }
                    else
                    {
                        returnMsg = "This is a social account, we can't send you reset password link.";
                        model.Message = returnMsg;
                        model.Success = false;
                    }
                }
                else
                {
                    returnMsg = "User account does not exist. Please submit valid email address";
                    model.Message = returnMsg;
                    model.Success = false;
                }
            }
            else
            {
                returnMsg = "Please provide valid email address";
                model.Message = returnMsg;
                model.Success = false;
            }
            //throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK,
            //                        returnMsg));

            // Archana 27.12.2016
            throw new HttpResponseException(Request.CreateResponse<SendResetPasswordLinkModel>(HttpStatusCode.OK, model));
           
        }

        [Route("api/VerifyUserAndResetPassword")]
        [AcceptVerbs("GET", "POST")]
        public string VerifyUserAndResetPassword(string verificationId, string emailId, string newPassword, string newConfirmPassword)
        {
            string returnMsg = string.Empty;
            if (verificationId != null && emailId != null && newPassword != null && newConfirmPassword != null)
            {
                if (newPassword == newConfirmPassword)
                {
                    User user = db.Users.Where(x => x.EmailId == emailId).FirstOrDefault();
                    if (user != null)
                    {
                        PasswordResetRequest IsUser = db.PasswordResetRequest.Where(x => x.VerificationId == verificationId && x.UserID == user.UserId).FirstOrDefault();
                        if (IsUser != null)
                        {
                            UserRegisterController userRegController = new UserRegisterController();
                            try
                            {
                                user.Password = userRegController.passwordEncrypt(newPassword);
                                db.Entry(user).State = EntityState.Modified;
                                db.SaveChanges();
                                IsUser.IsPasswordReset = true;
                                IsUser.PasswordResetDateTime = DateTime.Now;
                                db.Entry(IsUser).State = EntityState.Modified;
                                db.SaveChanges();

                                UserDetails userDetails = db.UsersDetails.Where(u => u.UserId == user.UserId).FirstOrDefault();
                                string cerTemplate = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/PasswordChangedEmail.htm");
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

                                returnMsg = "Your password is reset successfully.";
                            }
                            catch (Exception)
                            {
                                returnMsg = "An error has occured. Please check the parameters or try again";
                            }
                        }
                        else
                        {
                            returnMsg = "We dont find user with this email in our system. Please check and try again..";
                        }
                    }
                    else
                    {
                        returnMsg = "We dont find user with this email in our system. Please check and try again..";
                    }
                }
                else
                {
                    returnMsg = "New password and Confirm Password does not match. Please check and try again.";
                }
            }
            else
            {
                returnMsg = "All parameters are required. Please check and try again..";
            }
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                    returnMsg));
        }

        [Route("api/GetCoursesByCoupon")]
        public List<CouponCoursesModel> GetCoursesByCoupon(string couponCode)
        {
            if (couponCode != null && couponCode != "")
            {
                List<CouponCoursesModel> couponCoursesModel = new List<CouponCoursesModel>();
                if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    string userEmail = User.Identity.Name;
                    //string userEmail = "harsha@ncorpuscle.com";
                    var userInfo = db.Users.Join(db.UsersDetails, u => u.UserId, ud => ud.UserId, (u, ud) => new { u, ud }).Where(x => x.u.EmailId == userEmail).FirstOrDefault();
                    string DiscountPrice = string.Empty;
                    string Partner = string.Empty;
                    string BenefitDetails = string.Empty;
                    //List<Course> CourseList = null;
                    string AllowdCourses = string.Empty;
                    string ActCourses = string.Empty;

                    if (userInfo != null)
                    {
                        var couponCourses = db.Coupons.Join(db.CouponCourses, c => c.Id, cc => cc.CouponId, (c, cc) => new { c, cc }).Where(x => x.c.CouponCode == couponCode).ToList();
                        if (couponCourses.Count > 0)
                        {
                            Coupon coupon = db.Coupons.Where(f => f.CouponCode == couponCode && f.IsActive == true).FirstOrDefault();

                            if (coupon != null)
                            {
                                // Check if the coupon is used by the user
                                var usedCouponCount = db.UserCoupons.Where(uc => (uc.CouponId == coupon.Id && uc.ActivateBy == userInfo.u.UserId && uc.RedeemStatus == "Complete")).FirstOrDefault();
                                bool isUsedByUser = (usedCouponCount != null) ? true : false;
                                bool notActiveYet = coupon.ValidFrom >= DateTime.Now;
                                bool isExpired = coupon.ValidTo.Date < DateTime.Now.Date;

                                bool coupApplicable = false;
                                if (coupon.MultiRedeem)
                                {
                                    coupApplicable = true;
                                }
                                else
                                {
                                    if (usedCouponCount != null)
                                    {
                                        if (usedCouponCount.CouponId == coupon.Id && usedCouponCount.ActivateBy == userInfo.u.UserId && usedCouponCount.RedeemStatus == "Partial")
                                        {
                                            if (coupon.UserId == null && coupon.ProsUserId == null)
                                            {
                                                coupApplicable = true;
                                            }
                                            else
                                            {
                                                if (coupon.UserId == userInfo.u.UserId || coupon.ProsUserId == userInfo.u.UserId)
                                                {
                                                    coupApplicable = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            isUsedByUser = true;
                                        }
                                    }
                                    else
                                    {
                                        var userCoupons = db.UserCoupons.Where(x => x.CouponId == coupon.Id).FirstOrDefault();
                                        if (userCoupons != null)
                                        {
                                            if (userCoupons.ActivateBy == userInfo.u.UserId)
                                            {
                                                if (userCoupons.RedeemStatus == "Partial")
                                                {
                                                    isUsedByUser = false;
                                                    coupApplicable = true;
                                                }
                                                else
                                                {
                                                    isUsedByUser = true;

                                                }
                                            }
                                            else
                                            {
                                                isUsedByUser = true;
                                            }
                                        }
                                        else if (coupon.UserId == null && coupon.ProsUserId == null)
                                        {
                                            coupApplicable = true;
                                        }
                                        else
                                        {
                                            if (coupon.UserId == userInfo.u.UserId)
                                            {
                                                coupApplicable = true;
                                            }
                                            else if (coupon.ProsUserId == userInfo.u.UserId)
                                            {
                                                coupApplicable = true;
                                            }
                                            else
                                            {
                                                var usr = db.Users.Find(userInfo.u.UserId);
                                                var tmpUsr = db.TmpUsers.Where(x => x.EmailId == usr.EmailId).FirstOrDefault();
                                                if (tmpUsr != null)
                                                {
                                                    coupApplicable = true;
                                                }
                                                else
                                                {
                                                    coupApplicable = false;
                                                }
                                            }

                                        }
                                    }
                                }
                                if (coupApplicable == false)
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "Please check the coupon code (Format / Expiry / Already Used / Assigned to other"));
                                }
                                else if (isUsedByUser == true)
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "The coupon you have entered has been already used.Contact Support"));
                                }
                                else if (notActiveYet == true)
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "The coupon is not active yet for the activation. Please try again or try new coupon code.."));
                                }
                                else if (isExpired == true)
                                {
                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                         "The coupon you entered has expired.Contact Support"));
                                }
                                else
                                {
                                    DiscountPrice = coupon.DiscountPrice.ToString();
                                    Partner = coupon.PartnerName;

                                    var disc = coupon.DiscountPrice;
                                    string emailComp = (userInfo.u.EmailId == null ? null : userInfo.u.EmailId);
                                    string mobComp = (userInfo.ud.PhoneNumber == null ? null : userInfo.ud.PhoneNumber.ToString());
                                    if (coupon.EmailId == emailComp || coupon.MobileNo == Convert.ToInt64(mobComp)
                                        || coupon.EmailId == null || coupon.MobileNo == 0)
                                    {
                                        var benefit = db.BenifitTypes.Where(b => b.BenifitId == coupon.BenifitId).FirstOrDefault();
                                        BenefitDetails = benefit.ToString();

                                        //Course Details
                                        var coupCourses = db.CouponCourses.Where(cc => cc.CouponId == coupon.Id).ToList();

                                        //Ignore if the couse is already taken with the Coupon
                                        var redeemedCourses = db.UsersCourses.Where(uc => uc.CouponApplied == couponCode && uc.UserId == userInfo.u.UserId);

                                        //List<Course> courseList = new List<Course>();

                                        //string imgUrl = "https://www.millionlights.org/Images/CourseImg/{0}/{1}";
                                        string imgUrl = HttpContext.Current.Request.Url.Host + "/Images/CourseImg/{0}/{1}";
                                        foreach (var coupcourse in coupCourses)
                                        {
                                            //Ignore if the course is already redeemed
                                            if (redeemedCourses.FirstOrDefault(rc => rc.CourseID == coupcourse.CourseId) == null)
                                            {
                                                CouponCoursesModel courseViewModel = new CouponCoursesModel();
                                                Course crs = db.Courses.Where(c => c.Id == coupcourse.CourseId && c.IsActive == true).FirstOrDefault();

                                                if (crs != null)
                                                {
                                                    //courseList.Add(crs);
                                                    courseViewModel.CourseId = crs.Id;
                                                    courseViewModel.CourseCode = crs.CourseCode;
                                                    courseViewModel.CourseName = crs.CourseName;
                                                    courseViewModel.CourseImageURL = string.Format(imgUrl, crs.Id, crs.CourseImageLink);
                                                    courseViewModel.ShortDescription = crs.ShortDescription;
                                                    courseViewModel.LongDescription = crs.LongDescription;
                                                    courseViewModel.EDXCourseLink = crs.EDXCourseLink;
                                                    courseViewModel.StartDate = crs.StartDate;
                                                    courseViewModel.EndDate = crs.EndDate;
                                                    courseViewModel.CreatedOn = crs.CreatedOn;
                                                    courseViewModel.BasePrice = decimal.Round(crs.BasePrice, 2, MidpointRounding.AwayFromZero);

                                                    if (coupon.BenefitName != null && coupon.BenefitName != "")
                                                    {
                                                        if (coupon.BenefitName == "Discount")
                                                        {
                                                            courseViewModel.Discount = coupon.DiscountPrice.ToString() + " %";
                                                            courseViewModel.FinalPriceAfterDiscount = decimal.Round((crs.BasePrice) - ((coupon.DiscountPrice * crs.BasePrice) / 100), 2, MidpointRounding.AwayFromZero);
                                                        }
                                                        else
                                                        {
                                                            courseViewModel.Discount = coupon.DiscountPrice.ToString();
                                                            courseViewModel.FinalPriceAfterDiscount = decimal.Round(crs.BasePrice - coupon.DiscountPrice, 2, MidpointRounding.AwayFromZero);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        courseViewModel.Discount = coupon.DiscountPrice.ToString();
                                                        courseViewModel.FinalPriceAfterDiscount = decimal.Round(crs.BasePrice, 2, MidpointRounding.AwayFromZero);
                                                    }

                                                    //courseViewModel.LMSCourseId = crs.LMSCourseId;changed to below as per Srini
                                                    courseViewModel.LMSCourseId = GetLMSCourseId(crs.EDXCourseLink);
                                                    courseViewModel.Instructor = crs.Instructor;
                                                    couponCoursesModel.Add(courseViewModel);
                                                }
                                            }
                                        }

                                        //CourseList = courseList;
                                        AllowdCourses = coupon.AllowedCourses.ToString();
                                        ActCourses = redeemedCourses.Count().ToString();
                                    }
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "Please check the coupon code.."));
                            }

                            return couponCoursesModel;
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "No courses are found. Please check the coupon code.."));
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                          "User account does not exist."));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                         "You are not authenticated to access the details. Please login.."));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.OK,
                                         "Please enter the valid coupon code and try again.."));
            }
        }

        [Route("api/LoginExternalUser")]
        [AcceptVerbs("GET", "POST")]
        public ExternalUserModel LoginExternalUser(string appId, string secretKey, string email, string provider, string userName, string firstName, string lastName)
        {
            ExternalUserModel exUser = new ExternalUserModel();
            User user;
            string clientAppId = WebConfigurationManager.AppSettings["Client4AppId"];
            string clientSecret = WebConfigurationManager.AppSettings["Client4Secret"];
            if (clientAppId == appId && clientSecret == secretKey)
            {
                if (email != null && email != "")
                {
                    user = db.Users.FirstOrDefault(p => p.EmailId == email);
                    if (user == null)
                    {
                        //Create new user in the DB
                        User users = new User();
                        users.UserType = provider;
                        users.ProviderKey = null;
                        users.UserName = userName;
                        users.EmailId = email;
                        users.IsActive = true;
                        db.Users.Add(users);
                        db.SaveChanges();
                        int userId = users.UserId;

                        UserInRole usrInRole = new UserInRole();
                        usrInRole.UserId = userId;
                        usrInRole.RoleId = 2;
                        db.UserInRoles.Add(usrInRole);
                        db.SaveChanges();

                        UserDetails userDetails = new UserDetails();
                        userDetails.UserId = userId;
                        userDetails.FirstName = firstName;
                        userDetails.LastName = lastName;
                        userDetails.IsActive = true;
                        userDetails.RegisteredDatetime = DateTime.Now;
                        string partId = ConfigurationManager.AppSettings["MLPatrnerID"];
                        userDetails.PartnerId = int.Parse(partId);
                        db.UsersDetails.Add(userDetails);
                        db.SaveChanges();

                        //Send Email
                        if (email != null && email != "" && email != "null" && email != "undefined")
                        {
                            string regTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "ExternalUserRegistration.html");
                            MillionLightsEmails mEmail = new MillionLightsEmails();
                            mEmail.SendExternalRegistrationCompleteEmail(
                                ConfigurationManager.AppSettings["SenderName"],
                                ConfigurationManager.AppSettings["SenderEmail"],
                                 ConfigurationManager.AppSettings["Telephone"],
                                  ConfigurationManager.AppSettings["EmailId"],
                                "Your Millionlights Registration Is Successful",
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
                            db.UserNotitifications.Add(userNotification);
                            db.SaveChanges();

                            //User Created Successfully. Call the Login function and Get the Token.
                            string token = Login(appId, email, email, secretKey);
                            exUser.Token = token;

                            //New Referral Code for User Registrations
                            CouponGen cc = new CouponGen();
                            ReferralCodes refC = new ReferralCodes();
                            //refC.ReferralCode = cc.GenerateVouchers();
                            refC.ReferralCode = cc.GenerateReferralCodes();
                            refC.Referrer = userId;
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

                            exUser.Message = "New user created successfully. Redirect user to the respective dashboard page.";
                        }
                        else
                        {
                            exUser.Message = "Please provide the email address";
                        }
                    }
                    else if (user.IsActive == false)
                    {
                        exUser.Message = "Sorry, your account is deactivated. Please contact the MillionLights support team.";
                    }
                    else if (user.UserType != provider)
                    {
                        exUser.Message = "Sorry, the credentials you entered is already registered with your " + user.UserType + " account";
                    }
                    else
                    {
                        //User exists with the same provider
                        //Code to allow user to login
                        string token = Login(appId, email, "", secretKey);
                        exUser.Token = token;
                        exUser.Message = "Valid Existing User. Redirect user to the respective dashboard page.";
                    }
                }
                else
                {
                    exUser.Message = "Please provide the email address";
                }
            }
            else
            {
                exUser.Message = "Invalid appId and secret key.";
            }
            return exUser;
        }

        string Login(string appId, string email, string password, string secretKey)
        {
            string baseAddress = GetBaseUrl();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>    
                    {    
                        {"grant_type", "password"},    
                        {"username", email},
                        {"password", password},
                        {"client_id", appId},
                        {"client_secret", secretKey},
                    };
                var tokenResponse = client.PostAsync(baseAddress + "/oauth2/token", new FormUrlEncodedContent(form)).Result;
                var token = tokenResponse.Content.ReadAsStringAsync().Result;
                return token;
            }
        }

        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var baseUrl = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            return baseUrl;
        }
        public void OrderSuccessEmail(string cName, long orderNo, DateTime orderDate, decimal totalPrice, decimal unitPrice, string cNameForEmail,string orderbyUserId)
        {
            string recipientsEmail = null;
            string firstName = "";
            string lastName = "";
            var userId = Convert.ToInt32(orderbyUserId);
            var userReg = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == userId).FirstOrDefault();
            if (userReg != null)
            {
                recipientsEmail = userReg.a.EmailId;
                firstName = userReg.b.FirstName;
                lastName = userReg.b.LastName;
                var UserName = firstName + " " + lastName;
                string regTemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates"), "PaymentSuccess.htm");

                //Insert entry to User notification table
                MillionLightsEmails mEmail = new MillionLightsEmails();
                if (recipientsEmail != null)
                {
                    mEmail.SendPaymentSuccessEmail(
                    ConfigurationManager.AppSettings["SenderName"],
                    ConfigurationManager.AppSettings["SenderEmail"],
                     ConfigurationManager.AppSettings["Telephone"],
                    ConfigurationManager.AppSettings["EmailId"],
                    "MillionLights Course order Payment details",
                    new List<string> { recipientsEmail },
                    regTemplate,
                    UserName,
                    Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Content/assets/img/slider/Logo.png")), orderNo, orderDate, totalPrice, cName, unitPrice);


                    //email notification
                    UserNotitification userNotification = new UserNotitification();
                    userNotification.Receiver = userId;
                    userNotification.Sender = "System";
                    userNotification.Subject = String.Format(Constants.CourseCouponEnrollSubNotification, orderNo);
                    userNotification.Message = String.Format(Constants.CourseCouponEnrollMsgNotification, orderNo, cName);
                    userNotification.IsAlert = false;
                    userNotification.DateSent = DateTime.Now;
                    userNotification.ReadDate = null;
                    userNotification.SMSDate = null;
                    userNotification.MailDate = DateTime.Now;
                    userNotification.NotificationStatusId = 2;
                    db.UserNotitifications.Add(userNotification);
                    db.SaveChanges();
                }

                //Send SMS
                if (userReg.b.PhoneNumber != null)
                {
                    //int uId = int.Parse(Session["UserID"].ToString());
                    //var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
                    //var userName = userReg.b.FirstName + " " + userRegister.b.LastName;
                    Int64? numberToSend = userReg.b.PhoneNumber;
                    string msg = String.Format(Millionlights.Models.Constants.OrderSuccessSMSMsg, orderNo);
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
            }
        }
    }

    public class UserViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public string UserImgUrl { get; set; }
        public string UserName { get; set; }
    }

    public class UserCoursesModel
    {
        public string CourseName { get; set; }
        public string CourseLMSId { get; set; }
        public int MLCourseId { get; set; }
        public string EDXCourseLink { get; set; }
    }
    public class AllCoursesModel
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Objective { get; set; }
        public string ExamObjective { get; set; }
        public string EDXCourseLink { get; set; }
        public DateTime? StartDate { get; set; }
        public string stDateString
        {
            get
            {
                if (StartDate != null)
                {
                    return ((DateTime)StartDate).ToString(@"dd/MM/yyyy");
                }
                return string.Empty;
            }
        }

        public DateTime? EndDate { get; set; }
        public string endDateString
        {
            get
            {
                if (EndDate != null)
                {
                    return ((DateTime)EndDate).ToString(@"dd/MM/yyyy");
                }
                return string.Empty;
            }
        }
        public DateTime CreatedOn { get; set; }
        public decimal BasePrice { get; set; }
        public string LMSCourseId { get; set; }
        public string Instructor { get; set; }
    }
    public class ChangePasswordMessage
    {
        public string Message { get; set; }
    }
    public class CourseEnrollment
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int UserID { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderedDatetime { get; set; }
        public string couponCode { get; set; }
        public string enrolledCourses { get; set; }
    }
    public class MyCoupon
    {
        public int CoupanId { get; set; }
        public int? CCGId { get; set; }
        public string CouponCode { get; set; }
        public string PartnerName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string BenefitName { get; set; }
        public decimal DiscountPrice { get; set; }
        public bool IsPrepaid { get; set; }
        public int AllowedCourses { get; set; }
        public string ApplicableCourseIds { get; set; }
        public string ApplicableCourseCodes { get; set; }
        public string ApplicableCourseNames { get; set; }
        public string CouponRedeemStatus { get; set; }
        public string CouponExpired { get; set; }
        public string ActivatedByUserId { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime? BlockedOn { get; set; }
        public string BlockedReason { get; set; }
    }
    public class PublicCoursesModel
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string ShortDescription { get; set; }
        public string EDXCourseLink { get; set; }
        public string CourseImageURL { get; set; }
    }
    public class PublicCoursesDetailsModel
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseImageURL { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Objective { get; set; }
        public string ExamObjective { get; set; }
        public string EDXCourseLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal BasePrice { get; set; }
        public string LMSCourseId { get; set; }
        public string Instructor { get; set; }
        public string CourseStructure { get; set; }
    }
    public class CourseCategoriesList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CourseListByCategoryId
    {
        public int Id { get; set; }
        public string LMSCourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string ShortDescription { get; set; }
        public string EDXCourseLink { get; set; }
        public string CourseImageURL { get; set; }
    }
    public class CouponCoursesModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseImageURL { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string EDXCourseLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal BasePrice { get; set; }
        public string Discount { get; set; }
        public decimal FinalPriceAfterDiscount { get; set; }
        public string LMSCourseId { get; set; }
        public string Instructor { get; set; }
        public string CourseStructure { get; set; }
    }
    public class ExternalUserModel
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }

    public class SendResetPasswordLinkModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}