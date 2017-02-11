using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Millionlights.Models;
using System.Text;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;
using System.Configuration;
using Millionlights.EmailService;
using System.Net.Mime;

namespace Millionlights.Controllers
{
    public class CouponCodeController : Controller
    {

        private MillionlightsContext db = new MillionlightsContext();
        public string tempCouponCode = string.Empty;
        // GET: /CouponCode/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            var coupons = db.Coupons.Include(c => c.BenifitType).Include(c => c.CouponStatus).Include(c => c.createdByUser).Include(c => c.partner);
            return View(coupons.ToList());
        }

        [HttpPost]
        public JsonResult GetCouponDetails(string userType, int? partId, string coupState, string dateRange, string city, string couponTag, string couponCodeStr)
        {
            if (coupState == "null")
            {
                coupState = "";
            }
            var existingUser = "";
            if (userType == "exitUser")
            {
                existingUser = "exitUser";
            }
            else if (userType == "tempUser")
            {
                existingUser = "tempUser";
            }
            else {
                existingUser = "unAssigned";
            }
            DateTime? dtfrom = null;
            DateTime? dtTo = null;
            if (dateRange != "")
            {
                string[] dateR = dateRange.Split('-');
                dtfrom = DateTime.Parse(dateR[0]);
                dtTo = DateTime.Parse(dateR[1]).AddDays(1).AddSeconds(-1);
            }

      
            List<dynamic> couponDetails = new List<dynamic>();
            if (existingUser == "exitUser")
                {
                    //var couponInfo = db.Coupons.Join(db.UsersDetails, coup => coup.UserId, usd => usd.UserId, (coup, usd) => new { coup, usd })
                    //                  .Join(db.Users, usrd => usrd.coup.UserId, us => us.UserId, (usrd, us) => new { usrd, us })
                    //                  .Where(x => (String.IsNullOrEmpty(existingUser) || x.usrd.coup.usersid != null)
                    //                  && (String.IsNullOrEmpty(partId.ToString()) || x.usrd.coup.PartnerID == partId)
                    //                  && (String.IsNullOrEmpty(coupState) || coupState.Contains(x.usrd.coup.StatusId.ToString()))
                    //                  && (String.IsNullOrEmpty(dateRange) || (x.usrd.coup.CreatedOn >= dtfrom && x.usrd.coup.CreatedOn <= dtTo))
                    //                  && (String.IsNullOrEmpty(city) || (x.usrd.usd.City == city))
                    //                  && (String.IsNullOrEmpty(couponTag) || (x.usrd.coup.CouponTag == couponTag)));
                    var couponInfo = db.Coupons.Join(db.UsersDetails, coup => coup.UserId, usd => usd.UserId, (coup, usd) => new { coup, usd })
                                         .Join(db.Users, usrd => usrd.coup.UserId, us => us.UserId, (usrd, us) => new { usrd, us })
                                         .Where(x => (String.IsNullOrEmpty(existingUser) || x.usrd.coup.usersid != null)
                                         && (String.IsNullOrEmpty(partId.ToString()) || x.usrd.coup.PartnerID == partId)
                                         && (String.IsNullOrEmpty(coupState) || coupState.Contains(x.usrd.coup.StatusId.ToString()))
                                         && (String.IsNullOrEmpty(dateRange) || (x.usrd.coup.CreatedOn >= dtfrom && x.usrd.coup.CreatedOn <= dtTo))
                                         && (String.IsNullOrEmpty(city) || (x.usrd.usd.City == city))
                                         && (String.IsNullOrEmpty(couponTag) || (x.usrd.coup.CouponTag == couponTag))
                                         && (String.IsNullOrEmpty(couponCodeStr) || (x.usrd.coup.CouponCode == couponCodeStr)));

                    foreach (var item in couponInfo) {
                        try
                        {
                            dynamic p = new ExpandoObject();
                            p.Id = item.usrd.coup.Id;
                            p.CouponCode = item.usrd.coup.CouponCode;
                            p.PartnerName = item.usrd.coup.PartnerName;
                            p.AllowedCourses = item.usrd.coup.AllowedCourses;
                            p.DiscountPrice = item.usrd.coup.DiscountPrice;
                            p.CreatedOn = item.usrd.coup.CreatedOn;
                            p.CreatedOnString = item.usrd.coup.CreatedOnString;
                            p.ValidFromString = item.usrd.coup.ValidFromString;
                            p.ValidToString = item.usrd.coup.ValidToString;
                            p.CouponValidity = item.usrd.coup.CouponValidity;
                            p.StatusType = item.usrd.coup.StatusType;
                            p.AssignedUser = item.us.EmailId;
                            p.CouponTag = item.usrd.coup.CouponTag;
                            p.IsActive = item.usrd.coup.IsActive;
                            p = JsonConvert.SerializeObject(p);
                            couponDetails.Add(p);
                        }
                        catch (Exception) {
                        
                        }
                    }

                    //return Json(couponDetails, JsonRequestBehavior.AllowGet);
                  
                }
                else if (existingUser == "tempUser")
                {
                    var couponInfo = db.Coupons.Join(db.TmpUsers, a => a.ProsUserId, b => b.TmpId, (a, b) => new { a, b }).Where(x =>
                         (String.IsNullOrEmpty(existingUser) || x.a.ProsUserId != null)
                         && (String.IsNullOrEmpty(partId.ToString()) || x.a.PartnerID == partId)
                         && (String.IsNullOrEmpty(coupState) || coupState.Contains(x.a.StatusId.ToString()))
                         && (String.IsNullOrEmpty(dateRange) || (x.a.CreatedOn >= dtfrom && x.a.CreatedOn <= dtTo))
                          && (String.IsNullOrEmpty(city) || (x.b.City == city))
                          && (String.IsNullOrEmpty(couponTag) || (x.a.CouponTag == couponTag))
                          && (String.IsNullOrEmpty(couponCodeStr) || (x.a.CouponCode == couponCodeStr))).ToList();
                    foreach (var item in couponInfo)
                    {
                        try {
                            dynamic p = new ExpandoObject();
                            p.Id = item.a.Id;
                            p.CouponCode = item.a.CouponCode;
                            p.PartnerName = item.a.PartnerName;
                            p.AllowedCourses = item.a.AllowedCourses;
                            p.DiscountPrice = item.a.DiscountPrice;
                            p.CreatedOn = item.a.CreatedOn;
                            p.CreatedOnString = item.a.CreatedOnString;
                            p.ValidFromString = item.a.ValidFromString;
                            p.ValidToString = item.a.ValidToString;
                            p.CouponValidity = item.a.CouponValidity;
                            p.StatusType = item.a.StatusType;
                            p.AssignedUser = item.b.EmailId;
                            p.CouponTag = item.a.CouponTag;
                            p.IsActive = item.a.IsActive;
                            p = JsonConvert.SerializeObject(p);
                            couponDetails.Add(p);
                        
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                  var couponInfo = db.Coupons.Where(a => (String.IsNullOrEmpty(existingUser) || (a.ProsUserId == null && a.usersid == null))
                            && (String.IsNullOrEmpty(partId.ToString()) || a.PartnerID == partId)
                            && (String.IsNullOrEmpty(coupState) || coupState.Contains(a.StatusId.ToString()))
                            && (String.IsNullOrEmpty(dateRange) || (a.CreatedOn >= dtfrom && a.CreatedOn <= dtTo))
                            && (String.IsNullOrEmpty(couponTag) || (a.CouponTag == couponTag))
                            && (String.IsNullOrEmpty(couponCodeStr) || (a.CouponCode == couponCodeStr))).ToList();
                  foreach (var item in couponInfo)
                  {
                      try
                      {
                          dynamic p = new ExpandoObject();
                          p.Id = item.Id;
                          p.CouponCode = item.CouponCode;
                          p.PartnerName = item.PartnerName;
                          p.AllowedCourses = item.AllowedCourses;
                          p.DiscountPrice = item.DiscountPrice;
                          p.CreatedOn = item.CreatedOn;
                          p.CreatedOnString = item.CreatedOnString;
                          p.ValidFromString = item.ValidFromString;
                          p.ValidToString = item.ValidToString;
                          p.CouponValidity = item.CouponValidity;
                          p.StatusType = item.StatusType;
                          p.AssignedUser = item.EmailId;
                          p.CouponTag = item.CouponTag;
                          p.IsActive = item.IsActive;
                          p = JsonConvert.SerializeObject(p);
                          couponDetails.Add(p);
                      }
                      catch (Exception) { }
                  }
                  //return Json(couponDetails, JsonRequestBehavior.AllowGet);
                }
            var resultSet = Json(couponDetails, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }
        [HttpPost]
        public ActionResult AllCoupons(Coupon model)
        {
            var cd = new ContentDisposition
            {
                FileName = "CouponExportData.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        public ActionResult AllCoupons()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partnerInfo = db.Partners.Where(x => x.IsActive == true).ToList();
            if (partnerInfo != null)
            {
                foreach (var item in partnerInfo)
                {
                    partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
                }
                ViewBag.partnerList = partnerList;
            }

            List<SelectListItem> couponTypeList = new List<SelectListItem>();
            IEnumerable<CouponStatus> couponStateInfo = db.CouponStatus.Where(x => x.IsActive == true).ToList();
            if (couponStateInfo != null)
            {
                foreach (var item in couponStateInfo)
                {
                    couponTypeList.Add(new SelectListItem() { Text = item.StatusName, Value = item.StatusId.ToString() });
                }
                ViewBag.CouponStatusList = couponTypeList;
            }
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
         
            return View();
        }

        public ActionResult Disabled(int? id, bool status)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Coupon coupanstatus = db.Coupons.Find(id);
            if (status == true)
            {
                coupanstatus.IsActive = false;
            }
            else if (status == false)
            {
                coupanstatus.IsActive = true;
            }
            db.Entry(coupanstatus).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AllCoupons");
        }

        // GET: /CouponCode/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        public ActionResult CreateCoupon()
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

            List<SelectListItem> courseList = new List<SelectListItem>();
            IEnumerable<Course> courses = db.Courses.Where(X => X.IsActive == true && X.CourseAvailability!=2).ToList();

            foreach (var item in courses)
            {
                courseList.Add(new SelectListItem() { Text = item.CourseName, Value = item.Id.ToString() });
            }
            ViewBag.CourseList = courseList;
            List<SelectListItem> benifitList = new List<SelectListItem>();
            IEnumerable<BenifitType> benifitType = db.BenifitTypes.Where(X => X.IsActive == true).ToList();

            foreach (var item in benifitType)
            {
                benifitList.Add(new SelectListItem() { Text = item.BenifitName, Value = item.BenifitId.ToString() });
            }
            ViewBag.BenifitId = benifitList;
            List<SelectListItem> userEmailList = new List<SelectListItem>();
            IEnumerable<User> emaillist = db.Users.Where(X => X.IsActive == true).ToList();

            foreach (var item in emaillist)
            {
                userEmailList.Add(new SelectListItem() { Text = item.EmailId, Value = item.UserId.ToString() });
            }
            ViewBag.UserEmailId = userEmailList;

            List<SelectListItem> tempUserList = new List<SelectListItem>();
            IEnumerable<TmpUser> tempUser = db.TmpUsers.Where(x => x.EmailId != null).Take(100).ToList();
            foreach (var item in tempUser)
            {
                tempUserList.Add(new SelectListItem() { Text = item.EmailId, Value = item.TmpId.ToString() });
            }
            ViewBag.TmpUsersList = tempUserList;

            return View();
        }
        //public ActionResult CreateCoupon2()
        //{
        //    List<SelectListItem> partnerList = new List<SelectListItem>();
        //    IEnumerable<Partner> partnerType = db.Partners.Where(X => X.IsActive == true).ToList();

        //    foreach (var item in partnerType)
        //    {
        //        partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
        //    }
        //    ViewBag.PartnerID = partnerList;

        //    List<SelectListItem> courseList = new List<SelectListItem>();
        //    IEnumerable<Course> courses = db.Courses.Where(X => X.IsActive == true).ToList();

        //    foreach (var item in courses)
        //    {
        //        courseList.Add(new SelectListItem() { Text = item.CourseName, Value = item.Id.ToString() });
        //    }
        //    ViewBag.CourseList = courseList;
        //    List<SelectListItem> benifitList = new List<SelectListItem>();
        //    IEnumerable<BenifitType> benifitType = db.BenifitTypes.Where(X => X.IsActive == true).ToList();

        //    foreach (var item in benifitType)
        //    {
        //        benifitList.Add(new SelectListItem() { Text = item.BenifitName, Value = item.BenifitId.ToString() });
        //    }
        //    ViewBag.BenifitId = benifitList;
        //    List<SelectListItem> userEmailList = new List<SelectListItem>();
        //    IEnumerable<User> emaillist = db.Users.Where(X => X.IsActive == true).ToList();

        //    foreach (var item in emaillist)
        //    {
        //        userEmailList.Add(new SelectListItem() { Text = item.EmailId, Value = item.UserId.ToString() });
        //    }
        //    ViewBag.UserEmailId = userEmailList;

        //    List<SelectListItem> tempUserList = new List<SelectListItem>();
        //    IEnumerable<TmpUser> tempUser = db.TmpUsers.Where(x => x.EmailId != null).Take(100).ToList();
        //    foreach (var item in tempUser)
        //    {
        //        tempUserList.Add(new SelectListItem() { Text = item.EmailId, Value = item.TmpId.ToString() });
        //    }
        //    ViewBag.TmpUsersList = tempUserList;

        //    return View();
        //}

        private void ImportUsers(Coupon couponCourse,string csvFile, string partnerType, out List<TmpUser> successList, out List<TmpUser> erroList)
        {

            // Read CSV file. skip header
            var lines = System.IO.File.ReadLines(csvFile).Select(line => line.Split(',')).Skip(1);
            int tmpuserId = 1;
            if (db.TmpUsers.Any())
            {
                tmpuserId = db.TmpUsers.OrderByDescending(x => x.TmpId).FirstOrDefault().TmpId + 1;
            }

            List<TmpUser> tusers = lines
                .Select(tokens => new TmpUser
                {
                    TmpId = tmpuserId++,
                    FirsrName = tokens[0],
                    LastName = tokens[1],
                    EmailId = tokens[2],
                    MobileNumber = tokens[3],
                    AddressLine1 = tokens[5],
                    AddressLine2 = tokens[6],
                    City = tokens[7],
                    State = tokens[8],
                    Country = tokens[9],
                    ZipCode = tokens[10],
                    UserType = tokens[11],
                    PartnerId= couponCourse.PartnerID,
                }).ToList();
            List<TmpUser> SavedData = db.TmpUsers.ToList();
             //successList = tusers.Where(p => !SavedData.Any(p2 => p2.EmailId == p.EmailId)).ToList();
           successList = new List<TmpUser>();
            erroList = new List<TmpUser>();
            var distinctRecords = tusers.Where(p => !SavedData.Any(p2 => p2.EmailId == p.EmailId && p2.EmailId != string.Empty));

            if (partnerType.ToLowerInvariant().Contains("telecom"))
            {
                erroList.AddRange(tusers.FindAll(tu => tu.MobileNumber.Trim() == string.Empty));
               successList.AddRange(tusers.FindAll(tu => tu.MobileNumber.Trim() != string.Empty));
               // successList.AddRange(tusers.Where(p => !SavedData.Any(p2 => p2.EmailId == p.EmailId && p2.EmailId != string.Empty)));
            }
            else
            {
                erroList.AddRange(tusers.FindAll(tu => tu.EmailId.Trim() == string.Empty));
                successList.AddRange(tusers.FindAll(tu => tu.EmailId.Trim() != string.Empty)); 
                //successList.AddRange(tusers.Where(p => !SavedData.Any(p2 => p2.EmailId == p.EmailId && p2.EmailId!=string.Empty)));
            }

            using (var ctx = new MillionlightsContext())
            {
                using (var transactionScope = new TransactionScope())
                {
                    // some stuff in dbcontext


                    if (distinctRecords!=null)
                    {
                        ctx.BulkInsert(distinctRecords, SqlBulkCopyOptions.KeepIdentity);
                        ctx.SaveChanges();
                    }
                    transactionScope.Complete();
                }
            }
            return;
        }
        [HttpPost]
        public JsonResult GenerateCoupon(Coupon couponCourse, string courseidList, string assignUserList, string CSVFilePath, int NoOfCoupons, string assignTempUserList)
        {
            List<Coupon> coupons = null;
           
            if (CSVFilePath != "")
            {
                List<TmpUser> successList = null;
                List<TmpUser> erroList = null;

                //Import Users in db
                ImportUsers(couponCourse,CSVFilePath, couponCourse.PartnerName, out successList, out erroList);
                coupons = GenerateBulkCoupons(couponCourse, null, successList, NoOfCoupons, courseidList);
            }
            else
            {
                string[] userIdList = assignUserList.Split(',');
                if (userIdList[0] == "")
                {
                    userIdList = assignTempUserList.Split(',');
                    if (userIdList[0] == "")
                    {
                        userIdList = null;
                    }
                }

                coupons = GenerateBulkCoupons(couponCourse, userIdList, null, NoOfCoupons, courseidList);
            }

            AssignCouponCourses(courseidList.Split(','), coupons);
            string couponTag = TempData["couponTag"].ToString();
            return Json(couponTag);
        }

        private void AssignCouponCourses(string[] courseList, List<Coupon> couponList)
        {
            int couponCoursesId = 1;
            if (db.CouponCourses.Any())
            {
                couponCoursesId = db.CouponCourses.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }

            foreach (string item in courseList)
            {
                List<CouponCourses> cclist = couponList.
                    Select(coopon => new CouponCourses
                    {
                        Id = couponCoursesId++,
                        CouponId = coopon.Id,
                        CourseId = Convert.ToInt32(item)
                    }).ToList();

                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 20, 0)))
                {
                    db.BulkInsert(cclist, SqlBulkCopyOptions.KeepIdentity);
                    db.SaveChanges();
                    transactionScope.Complete();
                }
            }
        }
        private List<Coupon> GenerateBulkCoupons(Coupon couponCourse, string[] users, List<TmpUser> tmpUsers, int NoOfCoupons,string courseidList)
        {

            int couponId = 1;
            int userIdSession = int.Parse(Session["UserID"].ToString());
            if (db.Coupons.Any())
            {
                couponId = db.Coupons.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }

            List<Coupon> couponList = new List<Coupon>();
            Coupon coupon = null;
            CouponGen couponGen = null;
            var copCode="";
            var random = new Random();
            MillionLightsEmails mEmail = new MillionLightsEmails();
            var courseWithPrice = "";
            var partnerName = "";
            var PartnerTypeName = "";
            string couponCodeList = null;
            for (int i = 0; i < NoOfCoupons; i++)
            {
                couponGen = new CouponGen();
                coupon = new Coupon();
                coupon.Id = couponId++;
                coupon.StatusId = 4;
                copCode = couponGen.GenerateVouchers();
                coupon.CouponCode = copCode;
                coupon.BenifitId = couponCourse.BenifitId;
                coupon.PartnerID = couponCourse.PartnerID;
                coupon.ValidFrom = couponCourse.ValidFrom;
                coupon.ValidTo = couponCourse.ValidTo.AddDays(1).AddSeconds(-1);
                coupon.CreatedBy = couponCourse.CreatedBy;
                coupon.IsPrepaid = couponCourse.IsPrepaid;
                coupon.AllowedCourses = couponCourse.AllowedCourses;
                coupon.CreatedOn = DateTime.Now;
                coupon.IsActive = true;
                coupon.DiscountPrice = couponCourse.DiscountPrice;
                coupon.MultiRedeem = couponCourse.MultiRedeem;
               coupon.CouponTag =null;

                //Create comma separated coupon Code list to send it in an email
               if (couponCodeList != null)
               {
                   couponCodeList = copCode + ", " + couponCodeList;
               }
               else
               {
                   couponCodeList = copCode;
               }


                //Assign users if present
               if (users != null)
                {
                    if (i < users.Length)
                    {
                        coupon.UserId = int.Parse(users[i]);
                    }
                }

                //Assign tmp users if present
                if (tmpUsers != null)
                {
                    if (i < tmpUsers.Count)
                    {
                        string emailId =tmpUsers[i].EmailId;
                        var duplicateTmpUsers = db.TmpUsers.Where(x => x.EmailId == emailId).FirstOrDefault();
                        if (duplicateTmpUsers != null)
                        {
                            coupon.ProsUserId = duplicateTmpUsers.TmpId;
                        }
                        else { coupon.ProsUserId = tmpUsers[i].TmpId; }
                     
                    }
                }
                couponList.Add(coupon);
               coupon = null;
               
                //New Modification to fix pritis issue
                   var tableCintents = "";
                   var smsContents = "";
                   string[] courseIdList = courseidList.Split(',');

                   for (int c = 0; c < courseIdList.Count(); c++)
                   {
                       var curseId = int.Parse(courseIdList[c]);
                       var corseDetails = db.Courses.Where(x => x.Id == curseId).FirstOrDefault();
                       tableCintents = tableCintents + ("<tr><td>" + (1 + c) + ")" + " Course : " + corseDetails.CourseName + " - " + " Base Price : " + corseDetails.BasePrice + "</td></tr>");
                       smsContents = smsContents + ((1 + c) + "." + corseDetails.CourseName + ": " + " " + "Rs." + corseDetails.BasePrice + " ");

                   }
                   courseWithPrice = "<span style='font-size:10.0pt;font-family:'Georgia','serif';color:#565656'><table>" + tableCintents + "</table></span>";
                 //End Modification - If doesnt work the revert is to place this code just after the below 2 lines    
                if (users != null || (tmpUsers != null && i < 5))
                {
                   string recipientsEmail =null;
                    string firstName="";
                    string lastName="";
                    Int64? mobileNo=null;
                    if(users != null){
                        if (i < users.Length) {
                    var userId = int.Parse(users[i]);

                    var userReg = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == userId).FirstOrDefault();
                    if (userReg.a.EmailId != null)
                    {
                        recipientsEmail = userReg.a.EmailId;
                    }
                        firstName=userReg.b.FirstName;
                        lastName = userReg.b.LastName;
                        if (userReg.b.PhoneNumber != null)
                        {
                            mobileNo = userReg.b.PhoneNumber;
                        }
                        }
                        }
                    else{
                        if (i < tmpUsers.Count)
                        {
                            if ((tmpUsers)[i].EmailId != null)
                            {
                                recipientsEmail = (tmpUsers)[i].EmailId;
                            }
                            firstName = (tmpUsers)[i].FirsrName;
                            lastName = (tmpUsers)[i].LastName;
                            if ((tmpUsers)[i].MobileNumber != null)
                            {
                                mobileNo = long.Parse((tmpUsers)[i].MobileNumber);
                            }
                        }
                    }

                   
                    var UserName = firstName + " " + lastName;
                    var partnerDetails = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(x => x.a.Id == couponCourse.PartnerID).FirstOrDefault();
                    string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "CouponAssigment.htm");
                    
                    partnerName = partnerDetails.a.Name;
                   PartnerTypeName=partnerDetails.b.PartnerTypeName;
                    //if (partnerName == "Self Registered")
                    //{ partnerName = "Million Lights"; }
                    
                    //UserNotitification userNotif = new UserNotitification();
                    //try
                    //{
                   if (recipientsEmail != null)
                   {
                       mEmail.SendCouponAssignEmail(
                                               ConfigurationManager.AppSettings["SenderName"],
                                               ConfigurationManager.AppSettings["SenderEmail"],
                                               ConfigurationManager.AppSettings["Telephone"],
                                               ConfigurationManager.AppSettings["EmailId"],
                                               "Millionlights Coupon Assigned to you",
                                               new List<string> { recipientsEmail },
                                               regTemplate,
                                               UserName,
                                               Path.Combine(Server.MapPath("~/Content/assets/img/slider/Logo.png")),
                                               recipientsEmail, copCode, couponCourse.DiscountPrice, courseWithPrice, couponCourse.AllowedCourses, PartnerTypeName, partnerName, couponCourse.ValidFrom, couponCourse.ValidTo);
                   }
                       
                      
                    
                    //update user notification table
                    //        userNotif.MailDate = DateTime.Now;
                    //        userNotif.NotificationStatusId = 2;
                    //        db.Entry(userNotif).State = EntityState.Modified;
                    //        db.SaveChanges();
                    //}
                    //catch (Exception ex)
                    //{
                    //    //update user notification table
                    //    userNotif.MailDate = DateTime.Now;
                    //    userNotif.NotificationStatusId = 3;
                    //    db.Entry(userNotif).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                    
                        //email notification
                       
                       
                    //Send SMS
                   if (mobileNo != null)
                   {
                       var usename = firstName + " " + lastName;
                       Int64? numberToSend = mobileNo;
                       //var msg = "Dear" + " " + usename + "," + " " + "Congratulations, you can enroll" + " " + couponCourse.AllowedCourses + " " + "courses from" + " " + smsContents + " " + "use coupon Code :" + " " + copCode + " " + "to get discount :" + " " + couponCourse.DiscountPrice + "%" + " " + "On above courses.";
                       string msg = String.Format(Millionlights.Models.Constants.AssignedCouponSMSMsg, usename, couponCourse.AllowedCourses, smsContents, copCode, couponCourse.DiscountPrice);
                       var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
                       string smsRequestUrl = string.Format(url, numberToSend, msg);
                       HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
                       using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                       {
                           if (response.StatusCode.Equals(HttpStatusCode.OK))
                           {
                               StreamReader responseStream = new StreamReader(response.GetResponseStream());
                               string resp = responseStream.ReadLine();
                               // messageID = resp.Substring(33, 20);
                               //userNotif.SMSDate = DateTime.Now;
                               //userNotif.NotificationStatusId = 2;
                               //db.Entry(userNotif).State = EntityState.Modified;
                               //db.SaveChanges();
                           }
                           //  sms notification
                           //int NotificationId = userNotification.Id;
                           //UserNotitification userNotificationForSms = db.UserNotitifications.Find(NotificationId);
                           //userNotificationForSms.SMSDate = DateTime.Now;
                           //userNotificationForSms.NotificationStatusId = 2;
                           //db.Entry(userNotificationForSms).State = EntityState.Modified;
                           //db.SaveChanges();

                           UserNotitification userNotification = new UserNotitification();
                           userNotification.Receiver = userIdSession;
                           userNotification.Sender = "System";
                           userNotification.Subject = Constants.CouponSubNotification;
                           userNotification.Message = Constants.CouponMsgNotification;
                           userNotification.IsAlert = false;
                           userNotification.DateSent = DateTime.Now;
                           userNotification.ReadDate = null;
                           userNotification.SMSDate = DateTime.Now;
                           userNotification.MailDate = DateTime.Now;
                           userNotification.NotificationStatusId = 2;
                           db.UserNotitifications.Add(userNotification);
                           db.SaveChanges();

                           //else
                           //{
                           //    userNotif.SMSDate = DateTime.Now;
                           //    userNotif.NotificationStatusId = 3;
                           //    db.Entry(userNotif).State = EntityState.Modified;
                           //    db.SaveChanges();
                           //}
                       }
                   }
                    
                }

            }
            var couponTag = "";
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 20, 0)))
            {
                // Bulk Insert
                db.BulkInsert(couponList, SqlBulkCopyOptions.KeepIdentity);
                db.SaveChanges();
                //To update the coupon tag
                int lastCouponId = couponList[0].Id;
                couponTag = "CouponSet_" + lastCouponId;
                TempData["couponTag"] = couponTag;
                ViewBag.CouponTag = couponTag;
                var coupounTemp = db.Coupons.OrderByDescending(x => x.CouponTag==null).Take(couponList.Count()).ToList();
                foreach (var c in coupounTemp)
                {
                    coupon= db.Coupons.Where(x => x.Id == c.Id).FirstOrDefault();
                    coupon.CouponTag = couponTag;
                    
                    db.Entry(coupon).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                transactionScope.Complete();
            }
            //Mail send to admin with coupon details
                string assignedToAdminTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "CouponAssignmentMailToAdmin.html");
               
                var userName = Session["UserName"].ToString();
                string userEmail = Session["UserEmailId"].ToString();
                //Replace copCode with couponCodeList
                mEmail.SendCouponAssignToAdmin(
                                 ConfigurationManager.AppSettings["SenderName"],
                                 ConfigurationManager.AppSettings["SenderEmail"],
                                  ConfigurationManager.AppSettings["Telephone"],
                                  ConfigurationManager.AppSettings["EmailId"],
                                 "Details of Coupons Assigned to students",
                                  new List<string> { userEmail },
                                 assignedToAdminTemplate,
                                 userName, NoOfCoupons, couponTag,
                                 Path.Combine(Server.MapPath("~/Content/assets/img/slider/Logo.png")), couponCodeList, couponCourse.DiscountPrice, courseWithPrice, couponCourse.AllowedCourses, PartnerTypeName, partnerName, couponCourse.ValidFrom, couponCourse.ValidTo);
            
            return couponList;
        }


        [HttpPost]
        public JsonResult UploadCSVFile()
        {
            string path = "";
            int RecordCount = 0;
            var file = Request.Files[0];
            if (Request.Files.Count > 0)
            {

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/Content/CSVFiles/"), fileName);
                    file.SaveAs(path);
                    var lines = System.IO.File.ReadLines(path).Select(line => line.Split(',')).Skip(1);
                    RecordCount = lines.Count();
                }
            }
            List<dynamic> csvInfo = new List<dynamic>();
            dynamic dp = new ExpandoObject();
            dp.path = path;
            dp.count = RecordCount;
            dp = JsonConvert.SerializeObject(dp);
            csvInfo.Add(dp);
            return Json(csvInfo);
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
        //Upload CSV file Using Bank Campaign
        //public JsonResult UploadBankFile()
        //{
        //    string path = "";
        //    int RecordCount = 0;
        //    var file = Request.Files[0];
        //    if (Request.Files.Count > 0)
        //    {
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
                    var lines = System.IO.File.ReadLines(path).Select(line => line.Split(',')).Skip(1);
                    RecordCount = lines.Count();
                }
            }
            ImportBankUsers(path);
            return Json(path);
        }
        private void ImportBankUsers(string csvFile)
        {

            try
            {// Read CSV file. skip header
                var lines = System.IO.File.ReadLines(csvFile).Select(line => line.Split(',')).Skip(1);
                int tmpuserId = 1;
                if (db.TmpUsers.Any())
                {
                    tmpuserId = db.TmpUsers.OrderByDescending(x => x.TmpId).FirstOrDefault().TmpId + 1;
                }

                List<TmpUser> tusers = lines
                    .Select(tokens => new TmpUser
                    {
                        TmpId = tmpuserId++,
                        FirsrName = tokens[0],
                        LastName = tokens[1],
                        EmailId = tokens[2],
                        MobileNumber = tokens[3],
                        AddressLine1 = tokens[5],
                        AddressLine2 = tokens[6],
                        City = tokens[7],
                        State = tokens[8],
                        Country = tokens[9],
                        ZipCode = tokens[10],
                        UserType = tokens[11],
                    }).ToList();

                List<DuplicateRecords> duplicateUser = lines
                    .Select(tokens => new DuplicateRecords
                    {
                        TmpId = tmpuserId++,
                        FirsrName = tokens[0],
                        LastName = tokens[1],
                        EmailId = tokens[2],
                        MobileNumber = ReturnPhoneNumber(tokens[3]),
                        AddressLine1 = tokens[5],
                        AddressLine2 = tokens[6],
                        City = tokens[7],
                        State = tokens[8],
                        Country = tokens[9],
                        ZipCode = tokens[10],
                        UserType = tokens[11],
                    }).ToList();

                List<TmpUser> SavedData = db.TmpUsers.ToList();
                var distinct = tusers.Where(p => !SavedData.Any(p2 => p2.EmailId == p.EmailId));
                var duplicate = duplicateUser.Where(p => SavedData.Any(p2 => p2.EmailId == p.EmailId));

                TmpUser tmpUser = new TmpUser();
                var NullCells = db.TmpUsers.
                    Where(x => x.FirsrName == null || x.LastName == null || x.MobileNumber == null || x.CCGId == null || x.AddressLine1 == null || x.AddressLine2 == null || x.City == null || x.State == null || x.Country == null || x.ZipCode == null || x.UserType == null || x.PartnerId == null);
                foreach (var cell in NullCells)
                {
                    foreach (var data in duplicate)
                    {
                        if (cell.EmailId == data.EmailId)
                        {
                            tmpUser = (from p in db.TmpUsers
                                          where p.EmailId == cell.EmailId
                                          select p).SingleOrDefault();

                            if (cell.FirsrName == null && data.FirsrName != null)
                            {
                                    tmpUser.FirsrName = data.FirsrName;

                            }
                            if (cell.LastName == null && data.LastName != null)
                            {
                                tmpUser.LastName = data.LastName;
                            }

                            if (cell.MobileNumber == null && data.MobileNumber != null)
                            {
                                tmpUser.MobileNumber = data.MobileNumber.ToString();
                            }

                            if (cell.CCGId == null && data.CCGId != null)
                            {
                                tmpUser.CCGId = data.CCGId;
                            }

                            if (cell.EmailId == null && data.EmailId != null)
                            {
                                tmpUser.AddressLine1 = data.AddressLine1;
                            }

                            if (cell.AddressLine2 == null && data.AddressLine2 != null)
                            {
                                tmpUser.AddressLine2 = data.AddressLine2;
                            }
                            if (cell.City == null && data.City != null)
                                {
                                    tmpUser.City = data.City;
                            }
                            if (cell.State == null && data.State != null)
                                {
                                    tmpUser.State = data.State;
                            }

                            if (cell.Country == null && data.Country != null)
                                {
                                    tmpUser.Country = data.Country;
                            }

                            if (cell.ZipCode == null && data.ZipCode != null)
                                {
                                    tmpUser.ZipCode = data.ZipCode;
                            }

                            if (cell.UserType == null && data.UserType != null)
                                {
                                    tmpUser.UserType = data.UserType;
                            }
                            if (cell.PartnerId == null && data.PartnerId != null)
                                {
                                    tmpUser.PartnerId = data.PartnerId;
                            }
                            db.SaveChanges();
                        }
                        return;
                    }
                }

                using (var ctx = new MillionlightsContext())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        // some stuff in dbcontext
                        ctx.BulkInsert(distinct, SqlBulkCopyOptions.KeepIdentity);
                        ctx.BulkInsert(duplicate, SqlBulkCopyOptions.KeepIdentity);
                        ctx.SaveChanges();
                        transactionScope.Complete();
                    }
                }


            }
            catch (Exception)
            {

            }

        }
        // POST: /CouponCode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CCGId,CouponCode,PartnerID,AllowedCourses,BenifitId,ValidFrom,ValidTo,StatusId,ActivatedBy,ActivatedOn,BlockedOn,BlockedReason,CreatedBy,CreatedOn,IsPrepaid,IsActive")] Coupon coupon)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (ModelState.IsValid)
            {
                db.Coupons.Add(coupon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ActivatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.ActivatedBy);
            ViewBag.BenifitId = new SelectList(db.BenifitTypes, "BenifitId", "BenifitName", coupon.BenifitId);
            ViewBag.StatusId = new SelectList(db.CouponStatus, "StatusId", "StatusName", coupon.StatusId);
            ViewBag.CreatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.CreatedBy);
            ViewBag.PartnerID = new SelectList(db.Partners, "Id", "Name", coupon.PartnerID);
            return View(coupon);
        }

        // GET: /CouponCode/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ActivatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.ActivatedBy);
            ViewBag.BenifitId = new SelectList(db.BenifitTypes, "BenifitId", "BenifitName", coupon.BenifitId);
            ViewBag.StatusId = new SelectList(db.CouponStatus, "StatusId", "StatusName", coupon.StatusId);
            ViewBag.CreatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.CreatedBy);
            ViewBag.PartnerID = new SelectList(db.Partners, "Id", "Name", coupon.PartnerID);
            return View(coupon);
        }

        // POST: /CouponCode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CCGId,CouponCode,PartnerID,AllowedCourses,BenifitId,ValidFrom,ValidTo,StatusId,ActivatedBy,ActivatedOn,BlockedOn,BlockedReason,CreatedBy,CreatedOn,IsPrepaid,IsActive")] Coupon coupon)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (ModelState.IsValid)
            {
                db.Entry(coupon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ActivatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.ActivatedBy);
            ViewBag.BenifitId = new SelectList(db.BenifitTypes, "BenifitId", "BenifitName", coupon.BenifitId);
            ViewBag.StatusId = new SelectList(db.CouponStatus, "StatusId", "StatusName", coupon.StatusId);
            ViewBag.CreatedBy = new SelectList(db.Users, "UserId", "UserName", coupon.CreatedBy);
            ViewBag.PartnerID = new SelectList(db.Partners, "Id", "Name", coupon.PartnerID);
            return View(coupon);
        }

        public ActionResult ActivateCouponCode(string vcc, int userId, string isExt)
        {
            TempData["CouponCode"] = vcc;
            if (!string.IsNullOrEmpty(vcc))
            {
                HttpCookie cookie = new HttpCookie("CouponCode");
                cookie.Value = vcc;
                HttpContext.Response.SetCookie(cookie);
            }

            // Done by Archana 06.12.2016
            if (vcc.Length == 8)    //For Referral Code
            {
                if (isExt == "true")
                {
                    TempData["isExternalReferralCode"] = true;
                    return RedirectToAction("ActivateCoupon", "CouponCode");
                }
            }

            Coupon coupon = db.Coupons.Where(f => f.CouponCode == vcc && f.IsActive == true).FirstOrDefault();

            if (coupon != null)
            {
                TempData["ValidCoupon"] = true;
                // Check if the coupon is used by the user
                var usedCouponCount = db.UserCoupons.Where(uc => (uc.CouponId == coupon.Id && uc.ActivateBy == userId && uc.RedeemStatus == "Complete")).FirstOrDefault();
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
                        if (usedCouponCount.CouponId == coupon.Id && usedCouponCount.ActivateBy == userId && usedCouponCount.RedeemStatus == "Partial")
                        {
                            if (coupon.UserId == null && coupon.ProsUserId == null)
                            {
                                coupApplicable = true;
                            }
                            else
                            {
                                if (coupon.UserId == userId || coupon.ProsUserId == userId)
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

                        var userCoupons = db.UserCoupons.Where(x =>x.CouponId == coupon.Id).FirstOrDefault();
                        if (userCoupons !=null)
                        {
                            if (userCoupons.ActivateBy == userId)
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
                            
                                if (coupon.UserId == userId)
                                {
                                    coupApplicable = true;
                                }
                                else if (coupon.ProsUserId == userId)
                                {
                                    coupApplicable = true;
                                }
                                else
                                {
                                    var usr = db.Users.Find(userId);
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

                if ((!coupApplicable) || isUsedByUser || notActiveYet || isExpired)
                {
                    TempData["IsApplicableToRedeem"] = coupApplicable;
                    TempData["IsUsedByUser"] = isUsedByUser;
                    TempData["NotActiveYet"] = notActiveYet;
                    TempData["IsExpired"] = isExpired;
                }
                else
                {
                    TempData["IsApplicableToRedeem"] = true;
                    TempData["IsUsedByUser"] = false;
                    TempData["NotActiveYet"] = false;
                    TempData["IsExpired"] = false;
                    TempData["DiscountPrice"] = coupon.DiscountPrice;
                    TempData["Partner"] = coupon.PartnerName;
                    var disc = coupon.DiscountPrice;
                    string emailComp=(Session["UserEmailId"]==null?null:Session["UserEmailId"].ToString());
                    string mobComp = (Session["UserMobile"] == null ? null : Session["UserMobile"].ToString());
                    if (coupon.EmailId == emailComp || coupon.MobileNo == Convert.ToInt64(mobComp) 
                        || coupon.EmailId == null || coupon.MobileNo == 0)
                    {
                        TempData["EmailMatched"] = true;
                        TempData["MobileMatched"] = true;

                        var benefit = db.BenifitTypes.Where(b => b.BenifitId == coupon.BenifitId).FirstOrDefault();
                        TempData["BenefitDetails"] = benefit;

                        //Course Details
                        var coupCourses = db.CouponCourses.Where(cc => cc.CouponId == coupon.Id).ToList();

                        //Ignore if the couse is already taken with the Coupon
                        var redeemedCourses = db.UsersCourses.Where(uc => uc.CouponApplied == vcc && uc.UserId == userId);

                        List<Course> courseList = new List<Course>();
                        foreach (var coupcourse in coupCourses)
                        {
                            //Ignore if the course is already redeemed
                            if (redeemedCourses.FirstOrDefault(rc => rc.CourseID == coupcourse.CourseId) == null)
                            {
                                Course crs = db.Courses.Where(c => c.Id == coupcourse.CourseId && c.IsActive==true).FirstOrDefault();

                                if (crs != null)
                                {
                                    courseList.Add(crs);
                                }
                            }
                        }

                        TempData["CourseList"] = courseList;
                        TempData["AllowdCourses"] = coupon.AllowedCourses;
                        TempData["ActCourses"] = redeemedCourses.Count();
                    }
                }
            }
            else
            {
                TempData["ValidCoupon"] = false;
            }
            if (isExt == "true")
            {
                //ClearCoupon(vcc);
                return RedirectToAction("ActivateCoupon", "CouponCode");
            }
            else
            {
               // ClearCoupon(vcc);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
            //return Json("", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("ActivateCoupon", "CouponCode");
        }
        public void ClearCoupon(string vcc)
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
        public ActionResult ActivateCoupon()
        {
            //if (Session["UserID"] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            ViewBag.isExternalReferralCode = TempData["isExternalReferralCode"];
            ViewBag.CourseList = TempData["CourseList"];
            ViewBag.CouponCode = TempData["CouponCode"];
            ViewBag.ValidCoupon = TempData["ValidCoupon"];
            ViewBag.IsExpired = TempData["IsExpired"];
            ViewBag.NotActiveYet = TempData["NotActiveYet"];
            ViewBag.IsUsedByUser = TempData["IsUsedByUser"];
            ViewBag.EmailMatched = TempData["EmailMatched"];
            ViewBag.MobileMatched = TempData["MobileMatched"];
            ViewBag.CouponDetails = TempData["CouponDetails"];
            ViewBag.BenefitDetails = TempData["BenefitDetails"];
            ViewBag.AllowedCourses = TempData["AllowdCourses"];
            ViewBag.ActCourses = TempData["ActCourses"];
            ViewBag.IsApplicableToRedeem = TempData["IsApplicableToRedeem"];
            ViewBag.DiscountPrice = TempData["DiscountPrice"];
            ViewBag.Partner = TempData["Partner"];
            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            return View();
        }

        // GET: /CouponCode/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // POST: /CouponCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Coupon coupon = db.Coupons.Find(id);
            db.Coupons.Remove(coupon);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult UserListOnPartnerSelect(int partnerId)
        {

            List<SelectListItem> userList = new List<SelectListItem>().ToList();
            //IEnumerable<UserDetails> userDetails = db.UsersDetails.Join(db.UserInRoles, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(x => x.a.PartnerId == partnerId && x.b.RoleId == 2).Select(X => X.a).ToList();
            
            //List<string> user = new List<string>();
            //foreach (var ud in result)
            //{
            //    var usr = db.Users.Where(m => m.UserId == ud.UserId).FirstOrDefault();
            //    if (usr != null && usr.EmailId != null)
            //    {
            //        userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.UserId.ToString() });
            //    }
            //}
            MillionlightsDataContext md = new MillionlightsDataContext();
            List<GetUserDetailsResult> result = md.GetUserDetails(2, null, null, null, null, null, partnerId).ToList();
            List<string> user = new List<string>();
            foreach (var ud in result)
            {
                if (ud != null && ud.EmailId != null && ud.EmailId!="")
                {
                    userList.Add(new SelectListItem() { Text = ud.EmailId, Value = ud.UserId.ToString() });
                }
            }
            ViewBag.UserEmailId = userList;
            return Json(userList);
        }
        [HttpPost]
        public JsonResult BindSortedUsers(string usersDdl, string usersInfoDdl, string operatorDdl, string userInfoTxt)
        {
            List<SelectListItem> userList = new List<SelectListItem>();
            if (usersDdl != null)
            {
                if (usersDdl == "extUser")
                {
                    IEnumerable<UserDetails> userDetails = null;
                    if (usersInfoDdl == "fName")
                    {
                        if (operatorDdl == "equal")
                        {
                            userDetails = db.UsersDetails.Where(X => X.FirstName == userInfoTxt).ToList();
                        }
                        else
                        {
                            userDetails = db.UsersDetails.Where(X => X.FirstName != userInfoTxt).ToList();
                        }
                        foreach (var ud in userDetails)
                        {
                            var usr = db.Users.Where(m => m.UserId == ud.UserId).FirstOrDefault();
                            if (usr != null)
                            {

                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.UserId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "city")
                    {
                        if (operatorDdl == "equal")
                        { userDetails = db.UsersDetails.Where(X => X.City == userInfoTxt).ToList(); }
                        else { userDetails = db.UsersDetails.Where(X => X.City != userInfoTxt).ToList(); }

                        foreach (var ud in userDetails)
                        {
                            var usr = db.Users.Where(m => m.UserId == ud.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.UserId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "email")
                    {
                        IEnumerable<User> user = null;
                        if (operatorDdl == "equal")
                        { user = db.Users.Where(X => X.EmailId == userInfoTxt).ToList(); }
                        else { user = db.Users.Where(X => X.EmailId != userInfoTxt).ToList(); }

                        foreach (var ud in user)
                        {
                            var usr = db.Users.Where(m => m.UserId == ud.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.UserId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "utype")
                    {
                        IEnumerable<User> user = null;
                        if (operatorDdl == "equal")
                        { user = db.Users.Where(X => X.UserType == userInfoTxt).ToList(); }
                        else { user = db.Users.Where(X => X.UserType != userInfoTxt).ToList(); }

                        foreach (var ud in user)
                        {
                            var usr = db.Users.Where(m => m.UserId == ud.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.UserId.ToString() });
                            }
                        }
                    }


                }
                else
                {
                    IEnumerable<TmpUser> userDetails = null;
                    if (usersInfoDdl == "fName")
                    {
                        if (operatorDdl == "equal")
                        { userDetails = db.TmpUsers.Where(X => X.FirsrName == userInfoTxt).ToList(); }
                        else { userDetails = db.TmpUsers.Where(X => X.FirsrName != userInfoTxt).ToList(); }

                        foreach (var ud in userDetails)
                        {
                            var usr = db.TmpUsers.Where(m => m.TmpId == ud.TmpId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.FirsrName, Value = usr.TmpId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "city")
                    {
                        if (operatorDdl == "equal")
                        { userDetails = db.TmpUsers.Where(X => X.City == userInfoTxt).ToList(); }
                        else { userDetails = db.TmpUsers.Where(X => X.City != userInfoTxt).ToList(); }

                        foreach (var ud in userDetails)
                        {
                            var usr = db.TmpUsers.Where(m => m.TmpId == ud.TmpId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.TmpId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "emilId")
                    {
                        if (operatorDdl == "equal")
                        { userDetails = db.TmpUsers.Where(X => X.EmailId == userInfoTxt).ToList(); }
                        else { userDetails = db.TmpUsers.Where(X => X.EmailId != userInfoTxt).ToList(); }

                        foreach (var ud in userDetails)
                        {
                            var usr = db.TmpUsers.Where(m => m.TmpId == ud.TmpId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.TmpId.ToString() });
                            }
                        }
                    }
                    else if (usersInfoDdl == "utype")
                    {
                        if (operatorDdl == "equal")
                        { userDetails = db.TmpUsers.Where(X => X.UserType == userInfoTxt).ToList(); }
                        else { userDetails = db.TmpUsers.Where(X => X.UserType != userInfoTxt).ToList(); }

                        foreach (var ud in userDetails)
                        {
                            var usr = db.TmpUsers.Where(m => m.TmpId == ud.TmpId).FirstOrDefault();
                            if (usr != null)
                            {
                                userList.Add(new SelectListItem() { Text = usr.EmailId, Value = usr.TmpId.ToString() });
                            }
                        }
                    }

                }
            }
            ViewBag.UserEmailId = userList;
            return Json(userList);
        }
        public ActionResult Bank()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class CouponGen
    {
        private const int IN_BYTE_SIZE = 8;
        private const int OUT_BYTE_SIZE = 5;
        private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        private static Random random;
        private static object syncObj = new object();

        private string Base32Encode(byte[] data)
        {
            int i = 0, index = 0, digit = 0;
            int current_byte, next_byte;
            StringBuilder result = new StringBuilder((data.Length + 7) * IN_BYTE_SIZE / OUT_BYTE_SIZE);

            while (i < data.Length)
            {
                current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256); // Unsign

                /* Is the current digit going to span a byte boundary? */
                if (index > (IN_BYTE_SIZE - OUT_BYTE_SIZE))
                {
                    if ((i + 1) < data.Length)
                        next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                    else
                        next_byte = 0;

                    digit = current_byte & (0xFF >> index);
                    index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                    digit <<= index;
                    digit |= next_byte >> (IN_BYTE_SIZE - index);
                    i++;
                }
                else
                {
                    digit = (current_byte >> (IN_BYTE_SIZE - (index + OUT_BYTE_SIZE))) & 0x1F;
                    index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                    if (index == 0)
                        i++;
                }
                result.Append(alphabet[digit]);
            }

            return result.ToString();
        }


        private DateTime RandomDay()
        {
            DateTime start = new DateTime(1500, 12, 3);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        private static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }

        public string GenerateVouchers()
        {

            DateTime timeValue = RandomDay();
            List<string> voucherCodes = new List<string>();
            int rand = 0;
            //for (int i = 0; i < count; ++i)
            //{
            lock (syncObj)
            {
                if (random == null)
                    random = new Random();
                rand = random.Next(3600);
            }
            rand = random.Next(3600);
            timeValue = timeValue.AddMinutes(rand);

            byte[] b = System.BitConverter.GetBytes(timeValue.Ticks);

            string base64Code = Base32Encode(b);

            string code = string.Format("{0}-{1}-{2}", base64Code.Substring(0, 4), base64Code.Substring(4, 4), base64Code.Substring(8, 5)).ToUpperInvariant();
            // voucherCodes.Add(code);
            //}
            //return voucherCodes.ToArray();
            return code;
        }
        public string GenerateReferralCodes()
        {

            DateTime timeValue = RandomDay();
            List<string> voucherCodes = new List<string>();
            int rand = 0;
            //for (int i = 0; i < count; ++i)
            //{
            lock (syncObj)
            {
                if (random == null)
                    random = new Random();
                rand = random.Next(3600);
            }
            rand = random.Next(3600);
            timeValue = timeValue.AddMinutes(rand);

            byte[] b = System.BitConverter.GetBytes(timeValue.Ticks);

            string base64Code = Base32Encode(b);

            string code = string.Format("{0}{1}", base64Code.Substring(0, 4), base64Code.Substring(4, 4)).ToUpperInvariant();
            // voucherCodes.Add(code);
            //}
            //return voucherCodes.ToArray();
            return code;
        }

    }

}
