using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Millionlights.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Dynamic;
using System.Security.AccessControl;
using System.Security.Principal;
using PagedList;
using System.Net.Mime;
using System.Drawing;
using System.Data.Linq.SqlClient;

namespace Millionlights.Controllers
{
    public class CourseController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        // GET: /Course/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View();
        }
        [HttpPost]
        public ActionResult Index(Course model)
        {
            var cd = new ContentDisposition
            {
                FileName = "CourseExportData.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        [HttpPost]
        public JsonResult GetCourseDetails()
        {
            //IEnumerable<Course> courseDetails = db.Courses.Where(X => X.IsActive == true).OrderBy(x => x.Id).ToList();
            //return Json(courseDetails, JsonRequestBehavior.AllowGet);
            IEnumerable<Course> courseDetails = db.Courses.OrderBy(x => x.Id).ToList();
            List<dynamic> miniCourses = new List<dynamic>();
            foreach (var item in courseDetails)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.CourseName = item.CourseName;
                p.stDateString = item.stDateString;
                p.endDateString = item.endDateString;
                p.BasePrice = item.BasePrice;
                p.Duration = item.Duration;
                p.NoOfSessions = item.NoOfSessions;
                p.CourseCode = item.CourseCode;
                p.EnableForCertification = item.EnableForCertification ? "Yes" : "No"; ;
                p.DisplayOnHomePage = item.DisplayOnHomePage?"Yes":"No";
                p.IsActive = item.IsActive;
                p = JsonConvert.SerializeObject(p);
                miniCourses.Add(p);
            }
            return Json(miniCourses);
        }

        public ActionResult CreateCourse(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CoursesDetails course = new CoursesDetails();
            course.EnableForPayment = true;
            course.EnableForCertification = true;
            course.EnableForLMS = true;
            List<SelectListItem> courseAvlList = new List<SelectListItem>();
            IEnumerable<CourseAvailability> courseAvailabilities = db.CourseAvailability.ToList();

            foreach (var item in courseAvailabilities)
            {
                courseAvlList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseAvailability = courseAvlList;

            List<SelectListItem> courseCatList = new List<SelectListItem>();
            IEnumerable<CourseCategory> courseCategories = db.CourseCategories.ToList();

            foreach (var item in courseCategories)
            {
                courseCatList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseCategory = courseCatList;

            List<SelectListItem> courseDeliveryList = new List<SelectListItem>();
            IEnumerable<CourseDelivery> courseDelivery = db.CourseDeliveris.ToList();
            foreach (var item in courseDelivery)
            {
                courseDeliveryList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseDelivery = courseDeliveryList;

            List<SelectListItem> courseLevelsList = new List<SelectListItem>();
            IEnumerable<CourseLevel> courseLevels = db.CourseLevels.ToList();
            foreach (var item in courseLevels)
            {
                courseLevelsList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLevels = courseLevelsList;


            List<SelectListItem> courseLanguagesList = new List<SelectListItem>();
            IEnumerable<CourseLanguage> courseLanguages = db.CourseLanguages.ToList();
            foreach (var item in courseLanguages)
            {
                courseLanguagesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLanguages = courseLanguagesList;

            List<SelectListItem> courseTypesList = new List<SelectListItem>();
            IEnumerable<CourseType> courseTypes = db.CourseTypes.ToList();
            foreach (var item in courseTypes)
            {
                courseTypesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseTypes = courseTypesList;

            List<SelectListItem> certificationList = new List<SelectListItem>();
            IEnumerable<Certification> certifications = db.Certifications.ToList().Where(x=>x.IsActive);
            foreach (var item in certifications)
            {
                certificationList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.Certification = certificationList;

            //Added by harsha
            List<SelectListItem> courseCertProvList = new List<SelectListItem>();
            var parterlistCertProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Certification Provider").ToList();

            foreach (var item in parterlistCertProvider)
            {
                courseCertProvList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseCertiProvider = courseCertProvList;

            //Added by harsha
            List<SelectListItem> courseProviderList = new List<SelectListItem>();
            var parterlistCourseProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Course Provider").ToList();
            foreach (var item in parterlistCourseProvider)
            {
                courseProviderList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseProvider = courseProviderList;

            List<SelectListItem> eaxmManagerList = new List<SelectListItem>();
            var parterlistExamManager = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Exam Manager").ToList();
            foreach (var item in parterlistExamManager)
            {
                eaxmManagerList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.ExamManager = eaxmManagerList;
            return View(course);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]

        public ActionResult About(int? Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var courseList = db.Courses.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Course = courseList;

            List<Course> courseListByCat = db.Courses.Where(x => x.CourseCategory == courseList.CourseCategory && x.Id != Id).ToList();
            ViewBag.CourseByCategories = courseListByCat;

            return View();
        }
        public ActionResult AboutCourse(string id)
        {
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            if (TempData["CourseContents"] != null && TempData["Courses"] != null && TempData["Pe"] != null)
            {
                ViewBag.CourseContents = TempData["CourseContents"];
                ViewBag.Courses = TempData["Courses"];
                ViewBag.Pe = TempData["Pe"];

                //For Google+ Sharing
                ViewBag.PageTitle = TempData["PageTitle"];
                ViewBag.PageDesc = TempData["PageDesc"];
                ViewBag.PageImg = TempData["PageImg"];
            }
            else
            {
                var Courses = db.Courses.Where(x => x.CourseName == id).FirstOrDefault();
                if (Courses == null)
                {
                    var crId = int.Parse(id);
                    Courses = db.Courses.Where(x => x.Id == crId).FirstOrDefault();
                }
                ViewBag.Courses = Courses;
                ViewBag.CourseContents = db.CourseContent.Where(x => x.CourseId == Courses.Id).ToList();
                string pe = null;
                if (Courses.CourseAvailability == 2)
                {
                    pe = "1";
                }
                else
                {
                    pe = "0";
                }
                ViewBag.Pe = pe;
                ViewBag.Messages = Millionlights.Models.Constants.Messages();

                //For Google+ Sharing
                ViewBag.PageTitle = Courses.CourseName + "-" + Courses.ShortDescription;
                ViewBag.PageDesc = Courses.ShortDescription;
                if (Courses.CourseImageLink != null)
                {
                    if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                    {
                        ViewBag.PageImg = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + Courses.Id + "/" + Courses.CourseImageLink;
                    }
                    else
                    {
                        ViewBag.PageImg = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + Courses.Id + "/" + Courses.CourseImageLink;
                    }
                }
                else
                {
                    if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                    {
                        ViewBag.PageImg = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                    }
                    else
                    {
                        ViewBag.PageImg = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public JsonResult AboutCourseData(int? Id, string pe)
        {
            var courseList = db.Courses.Where(x => x.Id == Id).FirstOrDefault();
            //ViewBag.Courses = courseList;
            TempData["Courses"] = courseList;
            List<CourseContent> courseContents = db.CourseContent.Where(x => x.CourseId == Id).ToList();
            //ViewBag.CourseContents = courseContents;
            TempData["CourseContents"] = courseContents;
            TempData["Pe"] = pe;
            //ViewBag.Messages = Millionlights.Models.Constants.Messages();
            //return View();

            //Share on Google Plus
            TempData["PageTitle"] = courseList.CourseName + "-" + courseList.ShortDescription; ;
            TempData["PageDesc"] = courseList.ShortDescription;
            if (courseList.CourseImageLink != null)
            {
                if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                {
                    TempData["PageImg"] = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + courseList.Id + "/" + courseList.CourseImageLink;

                }
                else
                {
                    TempData["PageImg"] = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + courseList.Id + "/" + courseList.CourseImageLink;
                }
            }
            else
            {
                if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                {
                    TempData["PageImg"] = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                }
                else
                {
                    TempData["PageImg"] = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                }

            }
            return Json("");
        }
        public JsonResult GetCourses()
        {
            var courseCat = db.Courses.Join(db.CourseCategories, c => c.CourseCategory, cc => cc.Id.ToString(), (c, cc) => new { c, cc }).ToList();
            List<dynamic> minicourscat = new List<dynamic>();
            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.CourseName = item.c.CourseName + " in category " + item.cc.Name;
                p.CourseId = item.c.Id;
                minicourscat.Add(p);
            }
            return Json(minicourscat);
        }
        //Need to remove this code once the feature is freeze
        public ActionResult _AllCoursesPartial(int? Id, int? MinPrice, int? MaxPrice, string type, string courseName, int? page)
        {
            string currentURL = Server.UrlDecode(Request.Url.AbsoluteUri);
            string[] splittedCurrentUrl = currentURL.Split(new string[] { "&type=" }, StringSplitOptions.None);
            string[] typeIdSplittedUrl = currentURL.Split(new string[] { "id=", "&type=" }, StringSplitOptions.None);
            if (typeIdSplittedUrl.Length > 1 && type == null)
            {
                Id = int.Parse(typeIdSplittedUrl[1].ToString());
            }
            if (splittedCurrentUrl.Length > 1 && type == null)
            {
                type = splittedCurrentUrl[1].ToString();
            }

            int pageSize = 9;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<Course> courseList = null;
            if (courseName == null)
            {
                //courseList = db.Courses.ToList();
                courseList = db.Courses.Where(x => x.IsActive == true).ToList();
                if (type == "Category")
                {
                    courseList = courseList.Where(x => x.CourseCategory.Contains(Id.ToString())).ToList();
                }
                else if (type == "CourseAvailability")
                {
                    courseList = courseList.Where(x => x.CourseAvailability == Id).ToList();
                }
                else if (type == "CourseLevel")
                {
                    courseList = courseList.Where(x => x.CourseLevels == Id).ToList();
                }
                else if (type == "CourseLanguage")
                {
                    courseList = courseList.Where(x => x.CourseLanguage == Id).ToList();
                }
                else if (type == "CourseType")
                {
                    courseList = courseList.Where(x => x.CourseTypes == Id).ToList();
                }
                else if (type == "FreeCourse")
                {
                    courseList = courseList.Where(x => x.BasePrice < 1).ToList();
                }
                else if (type == "FilterCourse")
                {
                    courseList = courseList.Where(x => x.BasePrice >= MinPrice && x.BasePrice <= MaxPrice).ToList();
                }

                ViewBag.Course = courseList;

            }
            else
            {
                if (courseName != "")
                {
                    courseList = db.Courses.Where(x => x.IsActive == true).ToList();
                    courseList = courseList.Where(x => x.CourseName.ToLower().Contains(courseName.ToLower())).ToList();
                    ViewBag.Course = courseList;
                }
            }
            if (!string.IsNullOrEmpty(type))
            {
                ViewBag.CourseType = type;
                ViewBag.CourseId = Id;
            }
            else
            {
                ViewBag.CourseType = null;
                ViewBag.CourseId = null;
            }

            IPagedList<Course> CourseInfo = courseList.ToPagedList(pageIndex, pageSize);
            ViewBag.Course = CourseInfo;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            //Free and Paid Courses
            var freeCourses = courseList.Where(x => x.BasePrice == decimal.Parse("0.00")).ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.FreeCourses = freeCourses;
            var paidCourses = courseList.Where(x => x.BasePrice != decimal.Parse("0.00")).ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.PaidCourses = paidCourses;

            return PartialView("_AllCoursesPartial", CourseInfo);
        }

        //[ActionName("e-Learning-Certificate-Programs-India")]
        public ActionResult AllCourses(int? Id, int? MinPrice, int? MaxPrice, string type, string courseName, int? page)
        {
            //If paging, check last query 
            if (page != null && Session["LastQuery"] != null)
            {
                System.Collections.Specialized.NameValueCollection qscoll = HttpUtility.ParseQueryString(Session["LastQuery"].ToString());

                if (qscoll["id"] != null)
                {
                    Id = Convert.ToInt32(qscoll["id"]);
                }
                if (qscoll["MinPrice"] != null)
                {
                    MinPrice = Convert.ToInt32(qscoll["MinPrice"]);
                }
                if (qscoll["MaxPrice"] != null)
                {
                    MaxPrice = Convert.ToInt32(qscoll["MaxPrice"]);
                }
                if (qscoll["type"] != null)
                {
                    type = qscoll["type"];
                }
                if (qscoll["courseName"] != null)
                {
                    courseName = qscoll["courseName"];
                }
            }

            int? uId = null;
            if (Session["UserID"] != null)
            {
                uId = int.Parse(Session["UserID"].ToString());
            }

            int pageSize = 6;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            //Get all Course Catagories with Course Count
            if (Session["CourseCategories"] == null)
            {
                Session["CourseCategories"] = GetCourseCategories();
            }

            ViewBag.CourseCategory = Session["CourseCategories"];

            if (Session["AllCourses"] == null)
            {
                Session["AllCourses"] = GetAllActiveCourses();
            }

            List<ShortCourse> courseList = FilterCourses(courseName, type, Id, MinPrice, MaxPrice);
            //if (uId != null)
            //{
            //    foreach (var c in courseList)
            //    {
            //        if (db.UsersCourses.Where(x => x.CourseID == c.Id && x.UserId == uId).ToList().Count() > 0)
            //        {
            //            c.IsCourseEnrolled = true;
            //            c.EmailCourseName = "Hello, I have enrolled for the online course " + c.CourseName.Trim() + " , from Millionlights. This course might interest you too. Please have a look on Millionlights.org";
            //        }
            //    }
            //}

            PagedList.IPagedList<ShortCourse> freeCourses = (courseList.Where(x => x.BasePrice == decimal.Parse("0.00")).ToList()).ToPagedList(pageIndex, pageSize);
            ViewBag.FreeCourses = freeCourses;
            ViewBag.FreeCoursesCounter = freeCourses.Count;

            PagedList.IPagedList<ShortCourse> paidCourses = courseList.Where(x => x.BasePrice > decimal.Parse("0.00")).ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.PaidCourses = paidCourses;
            ViewBag.PaidCoursesCounter = paidCourses.Count;

            ViewBag.CourseType = type;
            ViewBag.CourseId = Id;
            ViewBag.Course = courseList.ToPagedList(pageIndex, pageSize);
            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            //This is to handle paging on filtered course list
            if (!string.IsNullOrEmpty(Request.Url.Query.Trim()) && !Request.Url.Query.Contains("page"))
            {
                Session["LastQuery"] = string.IsNullOrEmpty(Request.Url.Query.Trim()) ? null : Request.Url.Query.Replace("?", "");
            }

            return View(freeCourses.TotalItemCount >= paidCourses.TotalItemCount ? freeCourses : paidCourses);
        }

        private List<ShortCourse> GetAllActiveCourses()
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
        private List<ShortCourse> FilterCourses(string courseName, string type, int? id, int? MinPrice, int? MaxPrice)
        {
            List<ShortCourse> filteredCourses = new List<ShortCourse>();

            List<ShortCourse> allCourses = (List<ShortCourse>)Session["AllCourses"];

            if (!string.IsNullOrEmpty(courseName))
            {
                filteredCourses = allCourses.Where(x => x.CourseName.ToLower().Equals(courseName.ToLower())).ToList();
            }
            else
            {
                //Filter based on Search
                switch (type)
                {
                    case "Category":

                        // Done by Archana 25.01.2017
                        var list = allCourses.Where(delegate(ShortCourse cor)
                        {

                            List<int> listTemp = cor.Category.Split(',').Select(Int32.Parse).ToList();

                            return listTemp.Contains(Convert.ToInt32(id));
                        }).ToList();

                        filteredCourses = list;

                        // Commented by archana . This takes time to load a list 25.01.2017

                        //filteredCourses = allCourses.Where(x => x.Category == id.ToString()).ToList();
                        
                        //List<ShortCourse> courseList = new List<ShortCourse>();
                        //foreach (var item in allCourses)
                        //{

                        //    string[] tempId;
                        //    tempId = item.Category.ToString().Split(',');
                        //    for (int i = 0; i < tempId.Length; i++)
                        //    {
                        //        if (tempId[i] == id.ToString())
                        //        {
                        //            courseList.Add(item);
                        //        }
                        //    }
                        //}
                        //filteredCourses = courseList;                                          
                        
                        break;
                    case "CourseAvailability":
                        filteredCourses = allCourses.Where(x => x.AvailabilityId == id).ToList();
                        break;
                    case "CourseLevel":
                        filteredCourses = allCourses.Where(x => x.CourseLevel == id).ToList();
                        break;
                    case "CourseLanguage":
                        filteredCourses = allCourses.Where(x => x.CourseLanguage == id).ToList();
                        break;
                    case "CourseType":
                        filteredCourses = allCourses.Where(x => x.CourseType == id).ToList();
                        break;
                    case "FreeCourse":
                        filteredCourses = allCourses.Where(x => x.BasePrice < 1).ToList();
                        break;
                    case "FilterCourse":
                        filteredCourses = allCourses.Where(x => x.BasePrice >= MinPrice && x.BasePrice <= MaxPrice).ToList();
                        break;
                    default:
                        filteredCourses = allCourses;
                        break;
                }
            }
            return filteredCourses;
        }

        private List<CourseDetailsCategory> GetCourseCategories()
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
            return courseCatList;
        }

        [HttpPost]
        public JsonResult CourseExist(string courseCode)
        {
            var isExist = db.Courses.Where(x => x.CourseCode == courseCode).FirstOrDefault();
            var courseCodeExist = false;
            if (isExist != null)
            {
                courseCodeExist = true;
            }
            else
            {
                courseCodeExist = false;
            }
            return Json(courseCodeExist);
        }
        [HttpPost]
        public JsonResult CreateCourse(FormCollection form, HttpPostedFileBase tempImage)
        {
            Course course = new Course();

            if (form["CourseCode"] != "")
            {
                course.CourseCode = form["CourseCode"];
            }
            if (form["CourseName"] != "")
            {
                course.CourseName = form["CourseName"];
            }
            if (form["ShortDescription"] != "")
            {
                course.ShortDescription = form["ShortDescription"];
            }
            if (form["LongDescHidden"] != "")
            {
                course.LongDescription = HttpUtility.UrlDecode(form["LongDescHidden"]);
            }
            if (form["ObjectiveHidden"] != "")
            {
                course.Objective = HttpUtility.UrlDecode(form["ObjectiveHidden"]);
            }
            if (form["ExamObjectiveHidden"] != "")
            {
                course.ExamObjective = HttpUtility.UrlDecode(form["ExamObjectiveHidden"]);
            }
            if (form["StartDate"] != "")
            {
                course.StartDate = Convert.ToDateTime(form["StartDate"]);
            }
            else
            {
                course.StartDate = null;
            }
            if (form["EndDate"] != "")
            {
                course.EndDate = Convert.ToDateTime(form["EndDate"]).AddDays(1).AddSeconds(-1);
            }
            else
            {
                course.EndDate = null;
            }
            if (form["ImageNameHidden"] != "")
            {
                course.CourseImageLink = form["ImageNameHidden"];
            }
            if (form["Duration"] != "")
            {
                course.Duration = form["Duration"];
            }
            if (form["Hours"] != "")
            {
                course.Hours = Convert.ToInt32(form["Hours"]);
            }
            if (form["NoOfSessions"] != "")
            {
                course.NoOfSessions = Convert.ToInt32(form["NoOfSessions"]);
            }
            if (form["CourseAvailability"] != "")
            {
                course.CourseAvailability = Convert.ToInt32(form["CourseAvailability"]);
            }
            if (form["CreditPoints"] != "")
            {
                course.CreditPoints = Convert.ToInt32(form["CreditPoints"]);
            }
            if (form["Rating"] != "")
            {
                course.Rating = form["Rating"];
            }
            if (form["Instructor"] != "")
            {
                course.Instructor = form["Instructor"];
            }
            if (form["CourseCategory"] != "")
            {
                course.CourseCategory = form["CourseCategory"].ToString();
                //course.CourseCategory = Convert.ToInt32(form["CourseCategory"]);
            }
            if (form["CourseLevels"] != "")
            {
                course.CourseLevels = Convert.ToInt32(form["CourseLevels"]);
            }
            if (form["CourseLanguage"] != "")
            {
                course.CourseLanguage = Convert.ToInt32(form["CourseLanguage"]);
            }
            if (form["CourseTypes"] != "")
            {
                course.CourseTypes = Convert.ToInt32(form["CourseTypes"]);
            }
            if (form["CertificationProvider"] != "")
            {
                course.CertificationProvider = Convert.ToInt32(form["CertificationProvider"]);
            }
            if (form["ExamManager"] != "")
            {
                course.ExamManager = Convert.ToInt32(form["ExamManager"]);
            }
            if (form["CourseProvider"] != "")
            {
                course.CourseProvider = Convert.ToInt32(form["CourseProvider"]);
            }
            if (form["DeliveryID"] != "")
            {
                course.DeliveryID = Convert.ToInt32(form["DeliveryID"]);
            }
            string hasVideoLink = Request.Form.GetValues("HasVideoLink")[0];
            //if (form["HasVideoLink"] != "")
            //{
            //    hasVideoLink = form["HasVideoLink"];
            //}
            if (hasVideoLink == "true")
            {
                course.HasVideoLink = Convert.ToBoolean(1);
            }
            else
            {
                course.HasVideoLink = Convert.ToBoolean(0);
            }
            string scheduleApp = Request.Form.GetValues("ScheduleApplicable")[0];
            //if (form["ScheduleApplicable"] != "")
            //{
            //    scheduleApp = form["ScheduleApplicable"];
            //}

            if (scheduleApp == "true")
            {
                course.ScheduleApplicable = Convert.ToBoolean(1);
            }
            else
            {
                course.ScheduleApplicable = Convert.ToBoolean(0);
            }
            string hassamplecont = Request.Form.GetValues("HasSampleContents")[0];
            //if (form["HasSampleContents"] != "")
            //{
            //    hassamplecont = form["HasSampleContents"];
            //}

            if (hassamplecont == "true")
            {
                course.HasSampleContents = Convert.ToBoolean(1);
            }
            else
            {
                course.HasSampleContents = Convert.ToBoolean(0);
            }
            if (form["SampleContentsLink"] != "")
            {
                course.SampleContentsLink = form["SampleContentsLink"];
            }
            if (form["EDXCourseLink"] != "")
            {
                course.EDXCourseLink = form["EDXCourseLink"];
            }
            string enableCertification = Request.Form.GetValues("EnableForCertification1")[0];
            if (enableCertification == "true")
            {
                course.EnableForCertification = Convert.ToBoolean(1);

                string useDefaultCertificateContents = Request.Form.GetValues("UseDefaultCertificateContents")[0];

                if (useDefaultCertificateContents == "true")
                {
                    course.UseDefaultCertificateContents = Convert.ToBoolean(1);
                    course.CertificateTemplate = "";
                    course.CertificateLogo = "";
                    course.CertificateSignature = "";
                    course.CertificateTemplateHtmFile = "";
                }
                else
                {
                    course.UseDefaultCertificateContents = Convert.ToBoolean(0);
                    if (!string.IsNullOrEmpty(form["CertTemplateHidden"]))
                    {
                        course.CertificateTemplate = form["CertTemplateHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertLogoHidden"]))
                    {
                        course.CertificateLogo = form["CertLogoHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertSignHidden"]))
                    {
                        course.CertificateSignature = form["CertSignHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertHtmFileHidden"]))
                    {
                        course.CertificateTemplateHtmFile = form["CertHtmFileHidden"];
                    }   
                }
            }
            else
            {
                course.EnableForCertification = Convert.ToBoolean(0);
            }
            if (form["Certification"] != "")
            {
                course.CertificationId = Convert.ToInt32(form["Certification"]);
            }
            else
            {
                course.CertificationId = null;
            }
            if (form["BasePrice"] != "")
            {
                course.BasePrice = Convert.ToDecimal(form["BasePrice"]);
            }
            if (form["LMSCourseId"] != "")
            {
                course.LMSCourseId = form["LMSCourseId"];
            }
            string displayonhome = Request.Form.GetValues("DisplayOnHomePage")[0];
            if (displayonhome == "true")
            {
                course.DisplayOnHomePage = Convert.ToBoolean(1);
            }
            else
            {
                course.DisplayOnHomePage = Convert.ToBoolean(0);
            }
            string evidenceReq = Request.Form.GetValues("EvidenceRequired")[0];
            if (evidenceReq == "true")
            {
                course.EvidenceRequired = Convert.ToBoolean(1);
            }
            else
            {
                course.EvidenceRequired = Convert.ToBoolean(0);
            }
            course.CreatedBy = int.Parse(form["UserIdHidden"]);
            course.ModifiedBy = Convert.ToInt32(form["UserIdHidden"]);
            course.CreatedOn = DateTime.Today;
            course.ModifiedOn = DateTime.Today;
            course.IsActive = true;
            course.CoursePriority = int.Parse(form["CoursePriority"]);
            //// file upload
            //var file = Request.Files["image"];
            //if (Request.Files.Count > 0)
            //{

            //    if (file != null && file.ContentLength > 0)
            //    {
            //        var fileName = Path.GetFileName(file.FileName);
            //        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
            //        file.SaveAs(path);
            //        course.CourseImageLink = fileName;
            //    }
            //}

            // //video upload
            // var httpPostedFile = Request.Files[1];
            // if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            // {

            //     // Validate the uploaded file if you want like content length(optional)

            //     // Get the complete file path
            //     var uploadFilesDir = System.Web.HttpContext.Current.Server.MapPath("~/Videos/");
            //     if (!Directory.Exists(uploadFilesDir))
            //     {
            //         Directory.CreateDirectory(uploadFilesDir);
            //     }
            //     var fileSavePath = Path.Combine(uploadFilesDir, httpPostedFile.FileName);

            //     // Save the uploaded file to "UploadedFiles" folder
            //     httpPostedFile.SaveAs(fileSavePath);
            //     course.VideoLink = httpPostedFile.FileName;

            // }
            db.Courses.Add(course);
            db.SaveChanges();
            int courseId = course.Id;
            CourseContent coursecontent = new CourseContent();
            var stageCount = Convert.ToInt32(form["StageCountHidden"]);
            for (var i = 1; i <= stageCount; i++)
            {
                coursecontent.CourseId = courseId;
                if (i == 1)
                {
                    coursecontent.ChapterNumber = Convert.ToInt32(form["ChapterNumber"]);
                    coursecontent.ChapterName = form["ChapterName"];
                    coursecontent.ChapterDescription = form["ChapterDescription"];
                }
                else
                {

                    var chapNumber = "StageNo" + i + "_input";
                    var chapName = "StageName" + i + "_input";
                    var chapDesc = "StageDesc" + i + "_input";
                    coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
                    coursecontent.ChapterName = form[chapName];
                    coursecontent.ChapterDescription = form[chapDesc];
                }

                db.CourseContent.Add(coursecontent);
                db.SaveChanges();
            }
            coursecontent.CourseId = courseId;
            return Json(coursecontent);
        }

        public void UploadCoursePic(int FolderID)
        {
            // int userId = int.Parse(Session["UserID"].ToString());
            //string path = "";
            if (Request.Files.Count > 0)
            {


                var file = Request.Files[0];
                //string file = string.Empty;
                int i = 0;
                string fileName = string.Empty;
                string path = string.Empty;
                foreach (var item in Request.Files)
                {

                    string data = item.ToString();
                    string[] tempData = data.Split(':');
                    if (!string.IsNullOrEmpty(tempData[1]))
                    {
                        string image = tempData[0];
                        string id = tempData[1];

                        file = Request.Files[i];
                        fileName = Path.GetFileName(file.FileName);

                        if (id.Contains("Certificate"))
                        {
                            if (id == "uploadFileCertificateTemplate")
                            {
                                path = Server.MapPath("~/Images/Certificate/Template/" + FolderID);
                            }
                            else if (id == "uploadFileCertificateLogo")
                            {
                                path = Server.MapPath("~/Images/Certificate/Logo/" + FolderID);
                            }
                            else if (id == "uploadFileCertificateSign")
                            {
                                path = Server.MapPath("~/Images/Certificate/Signature/" + FolderID);
                            }
                            else if (id == "uploadFileCertificateHtmTemplateFile")
                            {
                                path = Server.MapPath("~/Images/Certificate/HtmlTemplate/" + FolderID);
                            }
                            else
                            {
                                path = Server.MapPath("~/Images/Certificate/" + FolderID);
                            }
                        }
                        else
                        {
                            path = Server.MapPath("~/Images/CourseImg/" + FolderID);
                        }

                        if (Directory.Exists(path))
                        {
                            file.SaveAs(path + "\\" + fileName);
                        }
                        else
                        {
                            DirectoryInfo createDirectory = Directory.CreateDirectory(path);
                            DirectorySecurity dirSecurity = createDirectory.GetAccessControl();
                            dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                            createDirectory.SetAccessControl(dirSecurity);
                            file.SaveAs(path + "\\" + fileName);
                        }
                    }

                    i++;
                }
            }
            //if (Request.Files.Count > 0)
            //{
            //    fileName = Path.GetFileName(file.FileName);
            //    path = Server.MapPath("~/Images/CourseImg/"+FolderID);

            //   if (Directory.Exists(path))
            //   {
            //      file.SaveAs(path + "\\" + fileName);
            //   }
            //   else
            //   {
            //    DirectoryInfo createDirectory = Directory.CreateDirectory(path);
            //    DirectorySecurity dirSecurity = createDirectory.GetAccessControl();
            //    dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            //    createDirectory.SetAccessControl(dirSecurity);
            //    file.SaveAs(path + "\\" + fileName);
            //    }
            //  }
            //return Json(path);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateCourse(FormCollection form)
        {

            int id = Convert.ToInt32(form["courseIdHidden"]);
            Course course = new Course();
            course = db.Courses.Find(id);

            //if (form["CourseCode"] != "")
            //{
            //    course.CourseCode = form["CourseCode"];
            //}
            if (form["CourseName"] != "")
            {
                course.CourseName = form["CourseName"];
            }
            if (form["ShortDescription"] != "")
            {
                course.ShortDescription = form["ShortDescription"];
            }
            if (form["LongDescHidden"] != "")
            {
                course.LongDescription = HttpUtility.UrlDecode(form["LongDescHidden"]);
            }
            if (form["ObjectiveHidden"] != "")
            {
                course.Objective = HttpUtility.UrlDecode(form["ObjectiveHidden"]);
            }

            if (form["ImageNameHidden"] != "")
            {
                course.CourseImageLink = form["ImageNameHidden"];
            }
            else
            {
                course.CourseImageLink = course.CourseImageLink;
            }
            if (form["ExamObjectiveHidden"] != "")
            {
                course.ExamObjective = HttpUtility.UrlDecode(form["ExamObjectiveHidden"]);
            }
            if (form["StartDate"] != "")
            {
                course.StartDate = Convert.ToDateTime(form["StartDate"]);
            }
            else
            {
                course.StartDate = null;
            }
            if (form["EndDate"] != "")
            {
                course.EndDate = Convert.ToDateTime(form["EndDate"]);
            }
            else
            {
                course.EndDate = null;
            }
            if (form["Duration"] != "")
            {
                course.Duration = form["Duration"];
            }
            if (form["Hours"] != "")
            {
                course.Hours = Convert.ToInt32(form["Hours"]);
            }
            if (form["NoOfSessions"] != "")
            {
                course.NoOfSessions = Convert.ToInt32(form["NoOfSessions"]);
            }
            if (form["CourseAvailability"] != "")
            {
                course.CourseAvailability = Convert.ToInt32(form["CourseAvailability"]);
            }
            if (form["CreditPoints"] != "")
            {
                course.CreditPoints = Convert.ToInt32(form["CreditPoints"]);
            }
            if (form["Rating"] != "")
            {
                course.Rating = form["Rating"];
            }
            if (form["Instructor"] != "")
            {
                course.Instructor = form["Instructor"];
            }
            if (form["CourseCategory"] != "")
            {
                course.CourseCategory = form["CourseCategory"].ToString();
                //course.CourseCategory = Convert.ToInt32(form["CourseCategory"]);
            }
            if (form["CourseLevels"] != "")
            {
                course.CourseLevels = Convert.ToInt32(form["CourseLevels"]);
            }
            if (form["CourseLanguage"] != "")
            {
                course.CourseLanguage = Convert.ToInt32(form["CourseLanguage"]);
            }
            if (form["CourseTypes"] != "")
            {
                course.CourseTypes = Convert.ToInt32(form["CourseTypes"]);
            }
            if (form["CertificationProvider"] != "")
            {
                course.CertificationProvider = Convert.ToInt32(form["CertificationProvider"]);
            }
            if (form["ExamManager"] != "")
            {
                course.ExamManager = Convert.ToInt32(form["ExamManager"]);
            }
            if (form["CourseProvider"] != "")
            {
                course.CourseProvider = Convert.ToInt32(form["CourseProvider"]);
            }
            if (form["DeliveryID"] != "")
            {
                course.DeliveryID = Convert.ToInt32(form["DeliveryID"]);
            }
            string hasVideoLink = Request.Form.GetValues("HasVideoLink")[0];
            //if (form["HasVideoLink"] != "")
            //{
            //    hasVideoLink = form["HasVideoLink"];
            //}
            if (hasVideoLink == "true")
            {
                course.HasVideoLink = Convert.ToBoolean(1);
            }
            else
            {
                course.HasVideoLink = Convert.ToBoolean(0);
            }
            string scheduleApp = Request.Form.GetValues("ScheduleApplicable")[0];
            //if (form["ScheduleApplicable"] != "")
            //{
            //    scheduleApp = form["ScheduleApplicable"];
            //}

            if (scheduleApp == "true")
            {
                course.ScheduleApplicable = Convert.ToBoolean(1);
            }
            else
            {
                course.ScheduleApplicable = Convert.ToBoolean(0);
            }
            string hassamplecont = Request.Form.GetValues("HasSampleContents")[0];
            //if (form["HasSampleContents"] != "")
            //{
            //    hassamplecont = form["HasSampleContents"];
            //}

            if (hassamplecont == "true")
            {
                course.HasSampleContents = Convert.ToBoolean(1);
            }
            else
            {
                course.HasSampleContents = Convert.ToBoolean(0);
            }
            if (form["SampleContentsLink"] != "")
            {
                course.SampleContentsLink = form["SampleContentsLink"];
            }
            if (form["EDXCourseLink"] != "")
            {
                course.EDXCourseLink = form["EDXCourseLink"];
            }
            string enableCertification = Request.Form.GetValues("EnableForCertification1")[0];

            if (enableCertification == "true")
            {
                course.EnableForCertification = Convert.ToBoolean(1);

                string useDefaultCertificateContents = Request.Form.GetValues("UseDefaultCertificateContents")[0];

                if (useDefaultCertificateContents == "true")
                {
                    course.UseDefaultCertificateContents = Convert.ToBoolean(1);
                    course.CertificateTemplate = "";
                    course.CertificateLogo = "";
                    course.CertificateSignature = "";
                    course.CertificateTemplateHtmFile = "";
                }
                else
                {
                    course.UseDefaultCertificateContents = Convert.ToBoolean(0);
                    if (!string.IsNullOrEmpty(form["CertTemplateHidden"]))
                    {
                        course.CertificateTemplate = form["CertTemplateHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertLogoHidden"]))
                    {
                        course.CertificateLogo = form["CertLogoHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertSignHidden"]))
                    {
                        course.CertificateSignature = form["CertSignHidden"];
                    }

                    if (!string.IsNullOrEmpty(form["CertHtmFileHidden"]))
                    {
                        course.CertificateTemplateHtmFile = form["CertHtmFileHidden"];
                    }
                }

               

            }
            else
            {
                course.EnableForCertification = Convert.ToBoolean(0);
                course.UseDefaultCertificateContents = Convert.ToBoolean(0);
            }
            if (form["Certification"] != "")
            {
                course.CertificationId = Convert.ToInt32(form["Certification"]);
            }
            else
            {
                course.CertificationId = null;
            }

            if (form["BasePrice"] != "")
            {
                course.BasePrice = Convert.ToDecimal(form["BasePrice"]);
            }

            if (form["LMSCourseId"] != "")
            {
                course.LMSCourseId = form["LMSCourseId"];
            }
            string displayonhome = Request.Form.GetValues("DisplayOnHomePage")[0];
            if (displayonhome == "true")
            {
                course.DisplayOnHomePage = Convert.ToBoolean(1);
            }
            else
            {
                course.DisplayOnHomePage = Convert.ToBoolean(0);
            }
            string evidenceReq = Request.Form.GetValues("EvidenceRequired")[0];
            if (evidenceReq == "true")
            {
                course.EvidenceRequired = Convert.ToBoolean(1);
            }
            else
            {
                course.EvidenceRequired = Convert.ToBoolean(0);
            }
            //course.CreatedBy = Convert.ToInt32(form["UserIdHidden"]);
            course.ModifiedBy = Convert.ToInt32(form["UserIdHidden"]);
            //course.CreatedOn = DateTime.Today;
            course.ModifiedOn = DateTime.Today;
            course.CoursePriority = int.Parse(form["CoursePriority"]);
            //course.IsActive = true;
            //// file upload
            //var file = Request.Files["image"];
            //if (Request.Files.Count > 0)
            //{

            //    if (file != null && file.ContentLength > 0)
            //    {
            //        var fileName = Path.GetFileName(file.FileName);
            //        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
            //        file.SaveAs(path);
            //        course.CourseImageLink = fileName;
            //    }
            //}
            // //video upload
            // var httpPostedFile = Request.Files[1];
            // if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            // {

            //     // Validate the uploaded file if you want like content length(optional)

            //     // Get the complete file path
            //     var uploadFilesDir = System.Web.HttpContext.Current.Server.MapPath("~/Videos/");
            //     if (!Directory.Exists(uploadFilesDir))
            //     {
            //         Directory.CreateDirectory(uploadFilesDir);
            //     }
            //     var fileSavePath = Path.Combine(uploadFilesDir, httpPostedFile.FileName);

            //     // Save the uploaded file to "UploadedFiles" folder
            //     httpPostedFile.SaveAs(fileSavePath);
            //     course.VideoLink = httpPostedFile.FileName;

            // }
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            //List<CourseContent> coursecontent= db.CourseContent.Where(m=>m.CourseId==id).ToList();
            //foreach (var content in coursecontent)
            //{

            //}
            //31,32,33,34,35
            string[] editedCourseIds = Session["EditedCourseIds"].ToString().Split(',');
            string[] Ids = form["contentLengthHidden"].Split(',');
            // var Ids = Idsr.Length - 1;
            int courseId = course.Id;
            CourseContent coursecontent = new CourseContent();
            var stageCount = Convert.ToInt32(form["StageCountHidden"]);
            for (int i = 0; i < stageCount; i++)
            {
                string courseContentId = Ids[i];
                int count = i + 1;
                if (!string.IsNullOrEmpty(courseContentId))
                {
                    bool IsRegularPresent = Array.Exists(editedCourseIds, element => element == courseContentId);
                    if (IsRegularPresent)
                    {
                        int contId = Convert.ToInt32(courseContentId);
                        coursecontent = db.CourseContent.Find(contId);
                        coursecontent.CourseId = id;

                        if (i == 0)
                        {
                            coursecontent.ChapterNumber = Convert.ToInt32(form["ChapterNumber_" + count]);
                            coursecontent.ChapterName = form["ChapterName_" + count];
                            coursecontent.ChapterDescription = form["ChapterDescription_" + count];
                        }
                        else
                        {
                            var chapNumber = "StageNo" + count + "_input";
                            var chapName = "StageName" + count + "_input";
                            var chapDesc = "StageDesc" + count + "_input";
                            coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
                            coursecontent.ChapterName = form[chapName];
                            coursecontent.ChapterDescription = form[chapDesc];
                        }
                        db.Entry(coursecontent).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var courseCont = new CourseContent { Id = int.Parse(courseContentId) };
                        db.CourseContent.Attach(courseCont);
                        db.CourseContent.Remove(courseCont);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var chapNumber = "StageNo" + count + "_input";
                    var chapName = "StageName" + count + "_input";
                    var chapDesc = "StageDesc" + count + "_input";
                    coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
                    coursecontent.ChapterName = form[chapName];
                    coursecontent.ChapterDescription = form[chapDesc];

                    db.CourseContent.Add(coursecontent);
                    db.SaveChanges();
                }
            }
            for (int i = 0; i < editedCourseIds.Length; i++)
            {
                bool IsPresent = Array.Exists(Ids, element => element == editedCourseIds[i]);
                if (!IsPresent)
                {
                    var courseCont = new CourseContent { Id = int.Parse(editedCourseIds[i]) };
                    db.CourseContent.Attach(courseCont);
                    db.CourseContent.Remove(courseCont);
                    db.SaveChanges();
                }
            }
            //foreach (var item in editedCourseIds)
            //{

            //    var target = item;

            //    bool IsRegularPresent = true;

            //    if (Ids.Count()>0)
            //    {

            //        IsRegularPresent = Array.Exists(Ids, element => element == target.ToString());
            //        if (!IsRegularPresent)
            //        {
            //            var courseCont = new CourseContent { Id = int.Parse(target) };
            //            db.CourseContent.Attach(courseCont);
            //            db.CourseContent.Remove(courseCont);
            //            db.SaveChanges();
            //        }
            //    }
            //}
            //var length = 0;
            //if (editedCourseIds.Length == 1)
            //{
            //    length = editedCourseIds.Length;
            //}
            //else
            //{
            //    length = editedCourseIds.Length - 1;
            //}
            ////var length = editedCourseIds.Length - 1;
            //for (var k = 0; k < length; k++)
            //{
            //    if (editedCourseIds[k] != "")
            //    {

            //        for (var i = 1; i <= stageCount; i++)
            //        {
            //            if ((i - 1) <= length)
            //            {

            //                if (i <= editedCourseIds.Length)
            //                {
            //                    int contId = Convert.ToInt32(editedCourseIds[i - 1]);
            //                    coursecontent = db.CourseContent.Find(contId);
            //                    coursecontent.CourseId = id;

            //                    if (i == 1)
            //                    {


            //                        coursecontent.ChapterNumber = Convert.ToInt32(form["ChapterNumber_" + i]);
            //                        coursecontent.ChapterName = form["ChapterName_" + i];
            //                        coursecontent.ChapterDescription = form["ChapterDescription_" + i];
            //                    }
            //                    else
            //                    {

            //                        var chapNumber = "StageNo" + i + "_input";
            //                        var chapName = "StageName" + i + "_input";
            //                        var chapDesc = "StageDesc" + i + "_input";
            //                        coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
            //                        coursecontent.ChapterName = form[chapName];
            //                        coursecontent.ChapterDescription = form[chapDesc];
            //                    }
            //                    db.Entry(coursecontent).State = EntityState.Modified;
            //                    db.SaveChanges();
            //                }



            //            }
            //            else
            //            {
            //                coursecontent.CourseId = id;
            //                if (i == 1)
            //                {
            //                    coursecontent.ChapterNumber = Convert.ToInt32(form["ChapterNumber_" + i]);
            //                    coursecontent.ChapterName = form["ChapterName_" + i];
            //                    coursecontent.ChapterDescription = form["ChapterDescription_" + i];
            //                }

            //                if (k == 1)
            //                {
            //                    var chapNumber = "StageNo" + i + "_input";
            //                    var chapName = "StageName" + i + "_input";
            //                    var chapDesc = "StageDesc" + i + "_input";
            //                    coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
            //                    coursecontent.ChapterName = form[chapName];
            //                    coursecontent.ChapterDescription = form[chapDesc];

            //                    db.CourseContent.Add(coursecontent);
            //                    db.SaveChanges();
            //                    newID = coursecontent.Id;
            //                }
            //                else if (i != editedCourseIds.Length)
            //                {
            //                    var chapNumber = "StageNo" + i + "_input";
            //                    var chapName = "StageName" + i + "_input";
            //                    var chapDesc = "StageDesc" + i + "_input";
            //                    coursecontent.ChapterNumber = Convert.ToInt32(form[chapNumber]);
            //                    coursecontent.ChapterName = form[chapName];
            //                    coursecontent.ChapterDescription = form[chapDesc];

            //                    db.CourseContent.Add(coursecontent);
            //                    db.SaveChanges();
            //                    newID = coursecontent.Id;
            //                }
            //                else
            //                {
            //                    var coursecontentTemp = db.CourseContent.Find(newID);
            //                    if (coursecontentTemp != null)
            //                    {
            //                        var chapNumber = "StageNo" + i + "_input";
            //                        var chapName = "StageName" + i + "_input";
            //                        var chapDesc = "StageDesc" + i + "_input";
            //                        coursecontentTemp.ChapterNumber = Convert.ToInt32(form[chapNumber]);
            //                        coursecontentTemp.ChapterName = form[chapName];
            //                        coursecontentTemp.ChapterDescription = form[chapDesc];
            //                        db.Entry(coursecontentTemp).State = EntityState.Modified;
            //                        db.SaveChanges();
            //                    }

            //                }

            //            }

            //        }
            //    }

            //for (int i = Ids.Length + 1; i <= editedCourseIds.Length; i++)
            //{
            //    //for (int j = 0; j < Ids.Length; j++)
            //    //{
            //    //if (editedCourseIds[i] != Ids[j])
            //    //{
            //    var courseCont = new CourseContent { Id = int.Parse(editedCourseIds[i]) };
            //    db.CourseContent.Attach(courseCont);
            //    db.CourseContent.Remove(courseCont);
            //    db.SaveChanges();
            //    //}
            //    //}
            //}
            return Json(coursecontent);
        }
        public static string UnescapeCodes(string src)
        {
            string result = string.Empty;
            if (Regex.Match(src, @".*\\u.*").Success)
            {
                Regex rx = new Regex(@"\\[uU]([0-9A-F]{4})");
                result = rx.Replace(result, delegate(Match match) { return ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString(); });
            }
            return result;

        }

        // POST: /Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(FormCollection form)
        //{
        //    Course course = new Course();

        //    //if (id == null)
        //    //{
        //    //    course.Id = courseDetails.CourseId;
        //    //    if (Request["Code"] != "")
        //    //    {
        //    //        course.CourseCode = Request["Code"];
        //    //    }
        //    //    if (Request["CourseName"] != "")
        //    //    {
        //    //        course.CourseName = Request["CourseName"];
        //    //    }
        //    //    if (Request["ShortDescription"] != "")
        //    //    {
        //    //        course.ShortDescription = Request["ShortDescription"];
        //    //    }
        //    //    course.LongDescription = courseDetails.LongDescription;
        //    //    course.Objective = courseDetails.Objective;
        //    //    course.ExamObjective = courseDetails.ExamObjective;
        //    //    if (Request["StartDate"] != "")
        //    //    {
        //    //        course.StartDate = Convert.ToDateTime(Request["StartDate"]);
        //    //    }
        //    //    if (Request["EndDate"] != "")
        //    //    {
        //    //        course.EndDate = Convert.ToDateTime(Request["EndDate"]);
        //    //    }
        //    //    course.CreatedBy = Convert.ToInt32(Userid);
        //    //    course.ModifiedBy = Convert.ToInt32(Userid);
        //    //    course.CreatedOn = DateTime.Today;
        //    //    course.ModifiedOn = DateTime.Today;
        //    //    if (Request["Certification"] != "")
        //    //    {
        //    //        course.CertificationId = Convert.ToInt32(Request["Certification"]);
        //    //    }
        //    //    if (Request["BasePrice"] != "")
        //    //    {
        //    //        course.BasePrice = Convert.ToDecimal(Request["BasePrice"]);
        //    //    }
        //    //    if (Request["LMSCourseId"] != "")
        //    //    {
        //    //        course.LMSCourseId = Convert.ToInt32(Request["LMSCourseId"]);
        //    //    }
        //    //    course.IsActive = true;
        //    //    string enableLMS = Request.Form.GetValues("EnableForLMS")[0];
        //    //    if (enableLMS == "true")
        //    //    {
        //    //        course.EnableForLMS = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.EnableForLMS = Convert.ToBoolean(0);
        //    //    }

        //    //    string enableCertification = Request.Form.GetValues("EnableForCertification")[0];
        //    //    if (enableCertification == "true")
        //    //    {
        //    //        course.EnableForCertification = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.EnableForCertification = Convert.ToBoolean(0);
        //    //    }

        //    //    string enablePayment = Request.Form.GetValues("EnableForPayment")[0];
        //    //    if (enablePayment == "true")
        //    //    {
        //    //        course.EnableForPayment = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.EnableForPayment = Convert.ToBoolean(0);
        //    //    }
        //    //    if (Request["Duration"] != "")
        //    //    {
        //    //        course.Duration = Request["Duration"];
        //    //    }
        //    //    if (Request["Hours"] != "")
        //    //    {
        //    //        course.Hours = Convert.ToInt32(Request["Hours"]);
        //    //    }
        //    //    if (Request["NoOfSessions"] != "")
        //    //    {
        //    //        course.NoOfSessions = Convert.ToInt32(Request["NoOfSessions"]);
        //    //    }
        //    //    if (Request["CourseLevels"] != "")
        //    //    {
        //    //        course.CourseLevels = Convert.ToInt32(Request["CourseLevels"]);
        //    //    }
        //    //    if (Request["CourseCategory"] != "")
        //    //    {
        //    //        course.CourseCategory = Convert.ToInt32(Request["CourseCategory"]);
        //    //    }
        //    //    if (Request["CourseAvailability"] != "")
        //    //    {
        //    //        course.CourseAvailability = Convert.ToInt32(Request["CourseAvailability"]);
        //    //    }

        //    //    string hassamplecont = Request.Form.GetValues("HasSampleContents")[0];
        //    //    if (hassamplecont == "true")
        //    //    {
        //    //        course.HasSampleContents = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.HasSampleContents = Convert.ToBoolean(0);
        //    //    }
        //    //    if (Request["SampleContentsLink"] != "")
        //    //    {
        //    //        course.SampleContentsLink = Request["SampleContentsLink"];
        //    //    }
        //    //    if (Request["CreditPoints"] != "")
        //    //    {
        //    //        course.CreditPoints = Convert.ToInt32(Request["CreditPoints"]);
        //    //    }
        //    //    if (Request["CertificationProvider"] != "")
        //    //    {
        //    //        course.CertificationProvider = Convert.ToInt32(Request["CertificationProvider"]);
        //    //    }
        //    //    if (Request["ExamManager"] != "")
        //    //    {
        //    //        course.ExamManager = Convert.ToInt32(Request["ExamManager"]);
        //    //    }
        //    //    if (Request["CourseProvider"] != "")
        //    //    {
        //    //        course.CourseProvider = Convert.ToInt32(Request["CourseProvider"]);
        //    //    }
        //    //    string hasVideoLink = Request.Form.GetValues("HasVideoLink")[0];
        //    //    if (hasVideoLink == "true")
        //    //    {
        //    //        course.HasVideoLink = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.HasVideoLink = Convert.ToBoolean(0);
        //    //    }
        //    //    if (Request["EDXCourseLink"] != "")
        //    //    {
        //    //        course.EDXCourseLink = Request["EDXCourseLink"];
        //    //    }
        //    //    string scheduleApp = Request.Form.GetValues("ScheduleApplicable")[0];
        //    //    if (scheduleApp == "true")
        //    //    {
        //    //        course.ScheduleApplicable = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.ScheduleApplicable = Convert.ToBoolean(0);
        //    //    }
        //    //    if (Request["DeliveryID"] != "")
        //    //    {
        //    //        course.DeliveryID = Convert.ToInt32(Request["DeliveryID"]);
        //    //    }
        //    //    if (Request["CourseLanguage"] != "")
        //    //    {
        //    //        course.CourseLanguage = Convert.ToInt32(Request["CourseLanguage"]);
        //    //    }
        //    //    if (Request["CourseTypes"] != "")
        //    //    {
        //    //        course.CourseTypes = Convert.ToInt32(Request["CourseTypes"]);
        //    //    }
        //    //    if (Request["Instructor"] != "")
        //    //    {
        //    //        course.Instructor = Request["Instructor"];
        //    //    }

        //    //    if (Request["Rating"] != "")
        //    //    {
        //    //        course.Rating = Request["Rating"];
        //    //    }
        //    //    string displayonhome = Request.Form.GetValues("DisplayOnHomePage")[0];
        //    //    if (displayonhome == "true")
        //    //    {
        //    //        course.DisplayOnHomePage = Convert.ToBoolean(1);
        //    //    }
        //    //    else
        //    //    {
        //    //        course.DisplayOnHomePage = Convert.ToBoolean(0);
        //    //    }
        //    //    //file upload
        //    //    var file = Request.Files[0];
        //    //    if (Request.Files.Count > 0)
        //    //    {

        //    //        if (file != null && file.ContentLength > 0)
        //    //        {
        //    //            var fileName = Path.GetFileName(file.FileName);
        //    //            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
        //    //            file.SaveAs(path);
        //    //            course.CourseImageLink = fileName;
        //    //        }
        //    //    }
        //    //    //video upload
        //    //    var httpPostedFile = Request.Files[1];
        //    //    if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
        //    //    {

        //    //        // Validate the uploaded file if you want like content length(optional)

        //    //        // Get the complete file path
        //    //        var uploadFilesDir = System.Web.HttpContext.Current.Server.MapPath("~/Videos/");
        //    //        if (!Directory.Exists(uploadFilesDir))
        //    //        {
        //    //            Directory.CreateDirectory(uploadFilesDir);
        //    //        }
        //    //        var fileSavePath = Path.Combine(uploadFilesDir, httpPostedFile.FileName);

        //    //        // Save the uploaded file to "UploadedFiles" folder
        //    //        httpPostedFile.SaveAs(fileSavePath);
        //    //        course.VideoLink = httpPostedFile.FileName;

        //    //    }

        //    //    db.Courses.Add(course);
        //    //    db.SaveChanges();

        //    //    int courseId = course.Id;
        //    //    CourseContent coursecontent = new CourseContent();
        //    //    coursecontent.CourseId = courseId;
        //    //    coursecontent.ChapterNumber = courseDetails.ChapterNumber;
        //    //    coursecontent.ChapterName = courseDetails.ChapterName;
        //    //    coursecontent.ChapterDescription = courseDetails.ChapterDescription;
        //    //    db.CourseContent.Add(coursecontent);
        //    //    db.SaveChanges();

        //        //TempData["Successmsg"] = "Record saved successfully";
        //   // }
        //    return RedirectToAction("Index");
        //}
        public ActionResult Disabled(int? id, bool status)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Course course = db.Courses.Find(id);
            if (status == true)
            {
                course.IsActive = false;
            }
            else
            {
                course.IsActive = true;
            }

            db.Entry(course).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditCourse(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> courseAvlList = new List<SelectListItem>();
            IEnumerable<CourseAvailability> courseAvailabilities = db.CourseAvailability.ToList();

            foreach (var item in courseAvailabilities)
            {
                courseAvlList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseAvailability = courseAvlList;

            List<SelectListItem> courseCatList = new List<SelectListItem>();
            IEnumerable<CourseCategory> courseCategories = db.CourseCategories.ToList();

            foreach (var item in courseCategories)
            {
                courseCatList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseCategory = courseCatList;
            List<SelectListItem> courseDeliveryList = new List<SelectListItem>();
            IEnumerable<CourseDelivery> courseDelivery = db.CourseDeliveris.ToList();
            foreach (var item in courseDelivery)
            {
                courseDeliveryList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseDelivery = courseDeliveryList;

            List<SelectListItem> courseLevelsList = new List<SelectListItem>();
            IEnumerable<CourseLevel> courseLevels = db.CourseLevels.ToList();
            foreach (var item in courseLevels)
            {
                courseLevelsList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLevels = courseLevelsList;


            List<SelectListItem> courseLanguagesList = new List<SelectListItem>();
            IEnumerable<CourseLanguage> courseLanguages = db.CourseLanguages.ToList();
            foreach (var item in courseLanguages)
            {
                courseLanguagesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLanguages = courseLanguagesList;
            //Added by harsha
            List<SelectListItem> courseCertProvList = new List<SelectListItem>();
            var parterlistCertProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Certification Provider").ToList();

            foreach (var item in parterlistCertProvider)
            {
                courseCertProvList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseCertiProvider = courseCertProvList;

            //Added by harsha
            List<SelectListItem> courseProviderList = new List<SelectListItem>();
            var parterlistCourseProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Course Provider").ToList();
            foreach (var item in parterlistCourseProvider)
            {
                courseProviderList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseProvider = courseProviderList;

            List<SelectListItem> eaxmManagerList = new List<SelectListItem>();
            var parterlistExamManager = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Exam Manager").ToList();
            foreach (var item in parterlistExamManager)
            {
                eaxmManagerList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.ExamManager = eaxmManagerList;

            List<SelectListItem> courseTypesList = new List<SelectListItem>();
            IEnumerable<CourseType> courseTypes = db.CourseTypes.ToList();
            foreach (var item in courseTypes)
            {
                courseTypesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseTypes = courseTypesList;

            List<SelectListItem> certificationList = new List<SelectListItem>();
            IEnumerable<Certification> certifications = db.Certifications.ToList();
            foreach (var item in certifications)
            {
                certificationList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.Certification = certificationList;
            //var courseContent = db.Courses.Join(db.CourseContent, a => a.Id, b => b.CourseId, (a, b) => new { a, b }).Where(X => X.a.Id == id).FirstOrDefault();
            var courseContent = db.Courses.Join(db.CourseContent, a => a.Id, b => b.CourseId, (a, b) => new { a, b }).Where(X => X.a.Id == id).FirstOrDefault();

            CoursesDetails courseDetails = new CoursesDetails();
            if (id != null)
            {
                courseDetails.CourseId = courseContent.a.Id;
                courseDetails.BasePrice = courseContent.a.BasePrice;
                courseDetails.CertificationId = courseContent.a.CertificationId;
                courseDetails.CertificationProvider = courseContent.a.CertificationProvider;
                courseDetails.CourseCode = courseContent.a.CourseCode;
                courseDetails.CourseAvailability = courseContent.a.CourseAvailability;
                courseDetails.CourseCategory = courseContent.a.CourseCategory;
                courseDetails.CourseDelivery = courseContent.a.CourseDelivery;
                courseDetails.CourseImageLink = courseContent.a.CourseImageLink;
                courseDetails.CourseLanguage = courseContent.a.CourseLanguage;
                courseDetails.CourseLevels = courseContent.a.CourseLevels;
                courseDetails.CourseName = courseContent.a.CourseName;
                courseDetails.CourseProvider = courseContent.a.CourseProvider;
                courseDetails.CourseTypes = courseContent.a.CourseTypes;
                courseDetails.CreditPoints = courseContent.a.CreditPoints;
                courseDetails.DeliveryID = courseContent.a.DeliveryID;
                courseDetails.DisplayOnHomePage = courseContent.a.DisplayOnHomePage;
                courseDetails.Duration = courseContent.a.Duration;
                courseDetails.EDXCourseLink = courseContent.a.EDXCourseLink;
                courseDetails.EnableForCertification = courseContent.a.EnableForCertification;
                courseDetails.EnableForLMS = courseContent.a.EnableForLMS;
                courseDetails.EnableForPayment = courseContent.a.EnableForPayment;
                courseDetails.EndDate = courseContent.a.EndDate;
                courseDetails.ExamManager = courseContent.a.ExamManager;
                courseDetails.ExamObjective = courseContent.a.ExamObjective;
                courseDetails.HasSampleContents = courseContent.a.HasSampleContents;
                courseDetails.HasVideoLink = courseContent.a.HasVideoLink;
                courseDetails.Hours = courseContent.a.Hours;
                courseDetails.Instructor = courseContent.a.Instructor;
                courseDetails.LMSCourseId = courseContent.a.LMSCourseId;
                courseDetails.LongDescription = courseContent.a.LongDescription;
                courseDetails.NoOfSessions = courseContent.a.NoOfSessions;
                courseDetails.Objective = courseContent.a.Objective;
                courseDetails.Rating = courseContent.a.Rating;
                courseDetails.SampleContentsLink = courseContent.a.SampleContentsLink;
                courseDetails.ScheduleApplicable = courseContent.a.ScheduleApplicable;
                courseDetails.ShortDescription = courseContent.a.ShortDescription;
                courseDetails.StartDate = courseContent.a.StartDate;
                courseDetails.VideoLink = courseContent.a.VideoLink;
                courseDetails.ChapterNumber = courseContent.b.ChapterNumber;
                courseDetails.ChapterName = courseContent.b.ChapterName;
                courseDetails.ChapterDescription = courseContent.b.ChapterDescription;
                courseDetails.EvidenceRequired = courseContent.a.EvidenceRequired;
                courseDetails.CoursePriority = courseContent.a.CoursePriority;

                courseDetails.UseDefaultCertificateContents = courseContent.a.UseDefaultCertificateContents;
               
                courseDetails.CertificateTemplate = courseContent.a.CertificateTemplate;
                courseDetails.CertificateLogo = courseContent.a.CertificateLogo;
                courseDetails.CertificateSignature = courseContent.a.CertificateSignature;
                courseDetails.CertificateTemplateHtmFile = courseContent.a.CertificateTemplateHtmFile;  
            }
            List<CourseContent> coursecontent = db.CourseContent.Where(m => m.CourseId == id).ToList();
            ViewBag.CourseContentCount = coursecontent.Count;
            ViewBag.CourseContents = coursecontent;
            string editedCourseIds = "";
            for (var i = 0; i <= coursecontent.Count - 1; i++)
            {
                string cId = coursecontent[i].Id.ToString();
                if (cId != "")
                {
                    editedCourseIds = editedCourseIds + "," + cId;
                }
            }

            Session["EditedCourseIds"] = editedCourseIds.TrimStart(',');
            return View(courseDetails);
        }
        // GET: /Course/Edit/5
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
            List<SelectListItem> courseAvlList = new List<SelectListItem>();
            IEnumerable<CourseAvailability> courseAvailabilities = db.CourseAvailability.ToList();

            foreach (var item in courseAvailabilities)
            {
                courseAvlList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseAvailability = courseAvlList;

            List<SelectListItem> courseCatList = new List<SelectListItem>();
            IEnumerable<CourseCategory> courseCategories = db.CourseCategories.ToList();

            foreach (var item in courseCategories)
            {
                courseCatList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseCategory = courseCatList;
            List<SelectListItem> courseDeliveryList = new List<SelectListItem>();
            IEnumerable<CourseDelivery> courseDelivery = db.CourseDeliveris.ToList();
            foreach (var item in courseDelivery)
            {
                courseDeliveryList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseDelivery = courseDeliveryList;

            List<SelectListItem> courseLevelsList = new List<SelectListItem>();
            IEnumerable<CourseLevel> courseLevels = db.CourseLevels.ToList();
            foreach (var item in courseLevels)
            {
                courseLevelsList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLevels = courseLevelsList;


            List<SelectListItem> courseLanguagesList = new List<SelectListItem>();
            IEnumerable<CourseLanguage> courseLanguages = db.CourseLanguages.ToList();
            foreach (var item in courseLanguages)
            {
                courseLanguagesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseLanguages = courseLanguagesList;
            //Added by harsha
            List<SelectListItem> courseCertProvList = new List<SelectListItem>();
            var parterlistCertProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Certification Provider").ToList();

            foreach (var item in parterlistCertProvider)
            {
                courseCertProvList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseCertiProvider = courseCertProvList;

            //Added by harsha
            List<SelectListItem> courseProviderList = new List<SelectListItem>();
            var parterlistCourseProvider = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Course Provider").ToList();
            foreach (var item in parterlistCourseProvider)
            {
                courseProviderList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.CourseProvider = courseProviderList;

            List<SelectListItem> eaxmManagerList = new List<SelectListItem>();
            var parterlistExamManager = db.Partners.Join(db.PartnerType, a => a.PartnerTypeId, b => b.Id, (a, b) => new { a, b }).Where(X => X.b.PartnerTypeName == "Exam Manager").ToList();
            foreach (var item in parterlistExamManager)
            {
                eaxmManagerList.Add(new SelectListItem() { Text = item.a.Name, Value = item.a.Id.ToString() });
            }
            ViewBag.ExamManager = eaxmManagerList;

            List<SelectListItem> courseTypesList = new List<SelectListItem>();
            IEnumerable<CourseType> courseTypes = db.CourseTypes.ToList();
            foreach (var item in courseTypes)
            {
                courseTypesList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CourseTypes = courseTypesList;

            List<SelectListItem> certificationList = new List<SelectListItem>();
            IEnumerable<Certification> certifications = db.Certifications.ToList();
            foreach (var item in certifications)
            {
                certificationList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.Certification = certificationList;
            var courseContent = db.Courses.Join(db.CourseContent, a => a.Id, b => b.CourseId, (a, b) => new { a, b }).Where(X => X.a.Id == id).FirstOrDefault();

            CoursesDetails courseDetails = new CoursesDetails();
            if (id != null)
            {
                courseDetails.CourseId = courseContent.a.Id;
                courseDetails.BasePrice = courseContent.a.BasePrice;
                courseDetails.CertificationId = courseContent.a.CertificationId;
                courseDetails.CertificationProvider = courseContent.a.CertificationProvider;
                courseDetails.CourseCode = courseContent.a.CourseCode;
                courseDetails.CourseAvailability = courseContent.a.CourseAvailability;
                courseDetails.CourseCategory = courseContent.a.CourseCategory;
                courseDetails.CourseDelivery = courseContent.a.CourseDelivery;
                courseDetails.CourseImageLink = courseContent.a.CourseImageLink;
                courseDetails.CourseLanguage = courseContent.a.CourseLanguage;
                courseDetails.CourseLevels = courseContent.a.CourseLevels;
                courseDetails.CourseName = courseContent.a.CourseName;
                courseDetails.CourseProvider = courseContent.a.CourseProvider;
                courseDetails.CourseTypes = courseContent.a.CourseTypes;
                courseDetails.CreditPoints = courseContent.a.CreditPoints;
                courseDetails.DeliveryID = courseContent.a.DeliveryID;
                courseDetails.DisplayOnHomePage = courseContent.a.DisplayOnHomePage;
                courseDetails.Duration = courseContent.a.Duration;
                courseDetails.EDXCourseLink = courseContent.a.EDXCourseLink;
                courseDetails.EnableForCertification = courseContent.a.EnableForCertification;
                courseDetails.EnableForLMS = courseContent.a.EnableForLMS;
                courseDetails.EnableForPayment = courseContent.a.EnableForPayment;
                courseDetails.EndDate = courseContent.a.EndDate;
                courseDetails.ExamManager = courseContent.a.ExamManager;
                courseDetails.ExamObjective = courseContent.a.ExamObjective;
                courseDetails.HasSampleContents = courseContent.a.HasSampleContents;
                courseDetails.HasVideoLink = courseContent.a.HasVideoLink;
                courseDetails.Hours = courseContent.a.Hours;
                courseDetails.Instructor = courseContent.a.Instructor;
                courseDetails.LMSCourseId = courseContent.a.LMSCourseId;
                courseDetails.LongDescription = courseContent.a.LongDescription;
                courseDetails.NoOfSessions = courseContent.a.NoOfSessions;
                courseDetails.Objective = courseContent.a.Objective;
                courseDetails.Rating = courseContent.a.Rating;
                courseDetails.SampleContentsLink = courseContent.a.SampleContentsLink;
                courseDetails.ScheduleApplicable = courseContent.a.ScheduleApplicable;
                courseDetails.ShortDescription = courseContent.a.ShortDescription;
                courseDetails.StartDate = courseContent.a.StartDate;
                courseDetails.VideoLink = courseContent.a.VideoLink;
                courseDetails.ChapterNumber = courseContent.b.ChapterNumber;
                courseDetails.ChapterName = courseContent.b.ChapterName;
                courseDetails.ChapterDescription = courseContent.b.ChapterDescription;
            }

            return View(courseDetails);
        }

        // POST: /Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOne(CoursesDetails courseDetails, int id, string Userid)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Course course = new Course();
            course = db.Courses.Find(id);
            if (Request["Code"] != "")
            {
                course.CourseCode = Request["Code"];
            }
            if (Request["CourseName"] != "")
            {
                course.CourseName = Request["CourseName"];
            }
            if (Request["ShortDescription"] != "")
            {
                course.ShortDescription = Request["ShortDescription"];
            }
            course.LongDescription = courseDetails.LongDescription;
            course.Objective = courseDetails.Objective;
            course.ExamObjective = courseDetails.ExamObjective;
            if (Request["StartDate"] != "")
            {
                course.StartDate = Convert.ToDateTime(Request["StartDate"]);
            }
            if (Request["EndDate"] != "")
            {
                course.EndDate = Convert.ToDateTime(Request["EndDate"]);
            }
            course.CreatedBy = Convert.ToInt32(Userid);
            course.ModifiedBy = Convert.ToInt32(Userid);
            course.CreatedOn = DateTime.Today;
            course.ModifiedOn = DateTime.Today;
            if (Request["Certification"] != "")
            {
                course.CertificationId = Convert.ToInt32(Request["Certification"]);
            }
            if (Request["BasePrice"] != "")
            {
                course.BasePrice = Convert.ToDecimal(Request["BasePrice"]);
            }
            if (Request["LMSCourseId"] != "")
            {
                course.LMSCourseId = Request["LMSCourseId"];
            }
            course.IsActive = true;
            string enableLMS = Request.Form.GetValues("EnableForLMS")[0];
            if (enableLMS == "true")
            {
                course.EnableForLMS = Convert.ToBoolean(1);
            }
            else
            {
                course.EnableForLMS = Convert.ToBoolean(0);
            }

            string enableCertification = Request.Form.GetValues("EnableForCertification")[0];
            if (enableCertification == "true")
            {
                course.EnableForCertification = Convert.ToBoolean(1);
            }
            else
            {
                course.EnableForCertification = Convert.ToBoolean(0);
            }

            string enablePayment = Request.Form.GetValues("EnableForPayment")[0];
            if (enablePayment == "true")
            {
                course.EnableForPayment = Convert.ToBoolean(1);
            }
            else
            {
                course.EnableForPayment = Convert.ToBoolean(0);
            }
            if (Request["Duration"] != "")
            {
                course.Duration = Request["Duration"];
            }
            if (Request["Hours"] != "")
            {
                course.Hours = Convert.ToInt32(Request["Hours"]);
            }
            if (Request["NoOfSessions"] != "")
            {
                course.NoOfSessions = Convert.ToInt32(Request["NoOfSessions"]);
            }
            if (Request["CourseLevels"] != "")
            {
                course.CourseLevels = Convert.ToInt32(Request["CourseLevels"]);
            }
            if (Request["CourseCategory"] != "")
            {
                course.CourseCategory = Request["CourseCategory"].ToString();
                //course.CourseCategory = Convert.ToInt32(Request["CourseCategory"]);
            }
            if (Request["CourseAvailability"] != "")
            {
                course.CourseAvailability = Convert.ToInt32(Request["CourseAvailability"]);
            }
            string hassamplecont = Request.Form.GetValues("HasSampleContents")[0];
            if (hassamplecont == "true")
            {
                course.HasSampleContents = Convert.ToBoolean(1);
            }
            else
            {
                course.HasSampleContents = Convert.ToBoolean(0);
            }
            if (Request["SampleContentsLink"] != "")
            {
                course.SampleContentsLink = Request["SampleContentsLink"];
            }
            if (Request["CreditPoints"] != "")
            {
                course.CreditPoints = Convert.ToInt32(Request["CreditPoints"]);
            }
            if (Request["CertificationProvider"] != "")
            {
                course.CertificationProvider = Convert.ToInt32(Request["CertificationProvider"]);
            }
            if (Request["ExamManager"] != "")
            {
                course.ExamManager = Convert.ToInt32(Request["ExamManager"]);
            }
            if (Request["CourseProvider"] != "")
            {
                course.CourseProvider = Convert.ToInt32(Request["CourseProvider"]);
            }
            string hasVideoLink = Request.Form.GetValues("HasVideoLink")[0];
            if (hasVideoLink == "true")
            {
                course.HasVideoLink = Convert.ToBoolean(1);
            }
            else
            {
                course.HasVideoLink = Convert.ToBoolean(0);
            }
            if (Request["EDXCourseLink"] != "")
            {
                course.EDXCourseLink = Request["EDXCourseLink"];
            }
            string scheduleApp = Request.Form.GetValues("ScheduleApplicable")[0];
            if (scheduleApp == "true")
            {
                course.ScheduleApplicable = Convert.ToBoolean(1);
            }
            else
            {
                course.ScheduleApplicable = Convert.ToBoolean(0);
            }
            if (Request["DeliveryID"] != "")
            {
                course.DeliveryID = Convert.ToInt32(Request["DeliveryID"]);
            }
            if (Request["CourseLanguage"] != "")
            {
                course.CourseLanguage = Convert.ToInt32(Request["CourseLanguage"]);
            }
            if (Request["CourseTypes"] != "")
            {
                course.CourseTypes = Convert.ToInt32(Request["CourseTypes"]);
            }
            if (Request["Instructor"] != "")
            {
                course.Instructor = Request["Instructor"];
            }
            if (Request["Rating"] != "")
            {
                course.Rating = Request["Rating"];
            }
            string displayonhome = Request.Form.GetValues("DisplayOnHomePage")[0];
            if (displayonhome == "true")
            {
                course.DisplayOnHomePage = Convert.ToBoolean(1);
            }
            else
            {
                course.DisplayOnHomePage = Convert.ToBoolean(0);
            }
            //file upload
            var file = Request.Files[0];
            if (Request.Files.Count > 0)
            {

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                    course.CourseImageLink = fileName;
                }
            }


            //video upload
            var httpPostedFile = Request.Files[1];
            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            {
                // Validate the uploaded file if you want like content length(optional)
                // Get the complete file path
                var uploadFilesDir = System.Web.HttpContext.Current.Server.MapPath("~/Videos/");
                if (!Directory.Exists(uploadFilesDir))
                {
                    Directory.CreateDirectory(uploadFilesDir);
                }
                var fileSavePath = Path.Combine(uploadFilesDir, httpPostedFile.FileName);

                // Save the uploaded file to "UploadedFiles" folder
                httpPostedFile.SaveAs(fileSavePath);
                course.VideoLink = httpPostedFile.FileName;

            }
            course.IsActive = true;
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            CourseContent coursecontent = new CourseContent();
            coursecontent.CourseId = id;
            coursecontent = db.CourseContent.Where(m => m.CourseId == id).FirstOrDefault();
            coursecontent.ChapterNumber = courseDetails.ChapterNumber;
            coursecontent.ChapterName = courseDetails.ChapterName;
            coursecontent.ChapterDescription = courseDetails.ChapterDescription;
            db.Entry(coursecontent).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }


        public ActionResult EditSelectedCourse(int? id)
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
            var UserInfo = db.Users.Where(X => X.IsActive == true).ToList();
            ViewBag.UserDetail = UserInfo;
            return View(userdetails);
        }




        // GET: /Course/Delete/5
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
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: /Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DeleteMultipleCourse(List<SearchCourseModel> model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Course courseModel = null;

            foreach (var course in model)
            {
                courseModel = db.Courses.Where(x => x.Id == course.Id).FirstOrDefault();
                courseModel.IsActive = false;
                db.Entry(courseModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Course");
        }
        public class SearchCourseModel
        {
            public int Id { get; set; }
        }
        //Knockout
        //[ActionName("e-Learning-Certificate-Programs-India")]
        public ActionResult PublicCourses()
        {
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View("PublicCourses");
        }
        public JsonResult GetCourseCategoryList()
        {
            List<dynamic> miniCategory = new List<dynamic>();
            var courseCat = db.CourseCategories.ToList();


            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                string name = item.Name;
                p.CategoryName = name;
                //int courseCount = db.Courses.Where(x => x.CourseCategory == item.Id.ToString() && x.IsActive == true).Count();
                int courseCount = db.Courses.Where(x => x.CourseCategory.Contains(item.Id.ToString()) && x.IsActive == true).Count();
                p.CourseCount = courseCount;
                p.CategoryCourseCount = String.Format("{0} ({1})", name, courseCount);
                p.Type = "Category";
                p = JsonConvert.SerializeObject(p);
                miniCategory.Add(p);
            }
            return Json(miniCategory);
        }
        public JsonResult GetCourseAvailability()
        {
            List<dynamic> miniCategory = new List<dynamic>();
            var courseCat = db.CourseAvailability.ToList();


            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                string name = item.Name;
                p.CategoryName = name;
                int courseCount = db.Courses.Where(x => x.CourseAvailability == item.Id && x.IsActive == true).Count();
                p.CourseCount = courseCount;
                p.CourseAvailabilityCount = String.Format("{0} ({1})", name, courseCount);
                p.Type = "CourseAvailability";
                p = JsonConvert.SerializeObject(p);
                miniCategory.Add(p);
            }
            return Json(miniCategory);
        }
        public JsonResult GetCourseLevel()
        {
            List<dynamic> miniCategory = new List<dynamic>();
            var courseCat = db.CourseLevels.ToList();


            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                string name = item.Name;
                p.CategoryName = name;
                int courseCount = db.Courses.Where(x => x.CourseLevels == item.Id && x.IsActive == true).Count();
                p.CourseCount = courseCount;
                p.CourseLevelCount = String.Format("{0} ({1})", name, courseCount);
                p.Type = "CourseLevel";
                p = JsonConvert.SerializeObject(p);
                miniCategory.Add(p);
            }
            return Json(miniCategory);
        }
        public JsonResult GetCourseLanguage()
        {
            List<dynamic> miniCategory = new List<dynamic>();
            var courseCat = db.CourseLanguages.ToList();


            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                string name = item.Name;
                p.CategoryName = name;
                int courseCount = db.Courses.Where(x => x.CourseLanguage == item.Id && x.IsActive == true).Count();
                p.CourseCount = courseCount;
                p.CourseLanguageCount = String.Format("{0} ({1})", name, courseCount);
                p.Type = "CourseLanguage";
                p = JsonConvert.SerializeObject(p);
                miniCategory.Add(p);
            }
            return Json(miniCategory);
        }
        public JsonResult GetCourseType()
        {
            List<dynamic> miniCategory = new List<dynamic>();
            var courseCat = db.CourseTypes.ToList();


            foreach (var item in courseCat)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                string name = item.Name;
                p.CategoryName = name;
                int courseCount = db.Courses.Where(x => x.CourseTypes == item.Id && x.IsActive == true).Count();
                p.CourseCount = courseCount;
                p.CourseTypeCount = String.Format("{0} ({1})", name, courseCount);
                p.Type = "CourseType";
                p = JsonConvert.SerializeObject(p);
                miniCategory.Add(p);
            }
            return Json(miniCategory);
        }
        public ActionResult GetCoursesByCategoryId(string clickedId, string type, string MinPrice, string MaxPrice)
        {
            int? uId = null;
            if (Session["UserID"] != null)
            {
                uId = int.Parse(Session["UserID"].ToString());
            }
            List<dynamic> miniCourses = new List<dynamic>();
            var courseList = db.Courses.Where(x => x.IsActive == true).ToList();
            if (clickedId != null)
            {
                int Id = int.Parse(clickedId);
                if (type == "Category")
                {
                    //courseList = courseList.Where(x => x.CourseCategory == Id.ToString()).ToList();
                    //courseList = courseList.Where(x => x.CourseCategory == Id.ToString()).ToList();
                    courseList = courseList.Where(x => x.CourseCategory.Contains(Id.ToString())).ToList();
                }
                if (type == "CourseAvailability")
                {
                    courseList = courseList.Where(x => x.CourseAvailability == Id).ToList();
                }
                if (type == "CourseLevel")
                {
                    courseList = courseList.Where(x => x.CourseLevels == Id).ToList();
                }
                if (type == "CourseLanguage")
                {
                    courseList = courseList.Where(x => x.CourseLanguage == Id).ToList();
                }
                if (type == "CourseType")
                {
                    courseList = courseList.Where(x => x.CourseTypes == Id).ToList();
                }

            }
            else if (type == "FreeCourse")
            {
                courseList = courseList.Where(x => x.BasePrice < 1).ToList();
            }
            else if (type == "FilterCourse" && MinPrice != null && MaxPrice != null)
            {
                decimal mnPrice = decimal.Parse(MinPrice);
                decimal mxPrice = decimal.Parse(MaxPrice);
                courseList = courseList.Where(x => x.BasePrice >= mnPrice && x.BasePrice <= mxPrice).ToList();
            }

            foreach (var item in courseList)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.CourseName = item.CourseName.Trim();
                p.ShortDescription = item.ShortDescription;
                p.CourseDuration = item.Duration + " (Days)";
                //p.hrefUrl = "/Course/AboutCourse?id=" + item.Id;
                //p.hrefUrl = "/Course/AboutCourse/" + System.Uri.EscapeDataString(item.CourseName.Trim());
                if (string.IsNullOrEmpty(item.CourseImageLink))
                {
                    p.ImageLink = "/Images/noimage378X225.png";
                    p.ImageName = "noimage378X225.png";
                }
                else
                {
                    var physicalPath = Request.MapPath("/Images/CourseImg/" + item.Id + "/" + item.CourseImageLink + "");
                    if (System.IO.File.Exists(physicalPath))
                    {
                        p.ImageLink = String.Format("/Images/CourseImg/{0}/{1}", item.Id, item.CourseImageLink);
                        p.ImageName = item.CourseImageLink;
                    }
                    else
                    {
                        p.ImageLink = "/Images/noimage378X225.png";
                        p.ImageName = "noimage378X225.png";
                    }

                }

                string tempCourseCatName = null;
                int CategoryId = 0;
                string courseCatName = item.CategoryName;
                if (clickedId != null)
                {
                    CategoryId = int.Parse(clickedId);
                }
                else
                {

                    string[] catIds = item.CourseCategory.Split(',');
                    CategoryId = int.Parse(catIds[0]);
                }
                var catName = db.CourseCategories.Where(x => x.Id == CategoryId).FirstOrDefault();
                tempCourseCatName = catName.Name;
                item.CourseCategory = tempCourseCatName;
                //for (int i = 0; i < catIds.Count(); i++)
                //{
                //    int CategoryId = int.Parse(catIds[i]);
                //    var catName = db.CourseCategories.Where(x => x.Id == CategoryId).FirstOrDefault();
                //    if (catName != null)
                //    {
                //        if (tempCourseCatName != null)
                //        {
                //            tempCourseCatName = catName.Name + "," + tempCourseCatName;
                //        }
                //        else
                //        {
                //            tempCourseCatName = catName.Name;
                //        }
                //    }
                //}
                //item.CourseCategory = tempCourseCatName;
                courseCatName = item.CourseCategory;
                if (courseCatName.Length > 21)
                {
                    tempCourseCatName = courseCatName.Substring(0, 20);
                }
                else
                {
                    tempCourseCatName = courseCatName;
                }
                if (courseCatName.Length > 21)
                {
                    p.TempCourseCatName = tempCourseCatName + "....";
                }
                else
                {
                    p.TempCourseCatName = tempCourseCatName;
                }
                //CourseDetails
                string courseName = item.CourseName.Trim();
                string tempCourseName = string.Empty;
                if (courseName.Length > 32)
                {
                    tempCourseName = courseName.Substring(0, 31);
                }
                else
                {
                    tempCourseName = courseName;
                }
                if (courseName.Length > 32)
                {
                    p.tempCourseName = tempCourseName + "....";
                }
                else
                {
                    p.tempCourseName = tempCourseName;
                }
                if (item.BasePrice == 0)
                {
                    p.CoursePrice = "Free";
                }
                else
                {
                    p.CoursePrice = "Rs." + item.BasePrice;
                }
                if (item.Availability == "Starting Soon")
                {
                    p.CourseAvailability = false;
                }
                else
                {
                    p.CourseAvailability = true;
                }
                var isCourseEnrolled = db.UsersCourses.Where(x => x.CourseID == item.Id && x.UserId == uId).ToList();
                if (isCourseEnrolled.Count() > 0)
                {
                    p.IsCourseEnrolled = true;
                    p.GoogleCourseDescription = System.Uri.EscapeDataString(p.ShortDescription);
                    p.EmailCourseName = "Hello, I have enrolled for the online course " + p.CourseName.Trim() + " , from Edunetworks. This course might interest you too. Please have a look on Edunetworks.com";
                    p.hrefUrl = item.EDXCourseLink;
                }
                else
                {
                    p.hrefUrl = "/Course/AboutCourse/" + System.Uri.EscapeDataString(item.CourseName.Trim());
                    p.IsCourseEnrolled = false;
                    p.GoogleCourseDescription = System.Uri.EscapeDataString(p.ShortDescription);
                    p.EmailCourseName = "Hello, I came across the online course " + p.CourseName.Trim() + " , from Edunetworks. This course might interest you. Please have a look on Edunetworks.com";

                }

                //Course Ratings Calculations
                var filledRatings = 0;
                var courseRatings = db.UsersCourseRatings.Where(x => x.CourseId == item.Id).ToList();
                if (courseRatings.Count() > 0)
                {
                    var courseCount = courseRatings.Count;
                    var rating = courseRatings.GroupBy(x => x.CourseId).Select(y => y.Sum(x => x.CourseRatings)).FirstOrDefault();
                    if (rating != null)
                    {
                        var finalCourseRating = rating / courseCount;
                        var finalRatings = (int)Math.Ceiling(decimal.Parse(finalCourseRating.ToString()));
                        p.CourseRatings = finalRatings;
                        filledRatings = finalRatings;
                    }
                }
                else
                {
                    //p.CourseRatings = 3;
                    //filledRatings = 2;
                    p.CourseRatings = 0;
                    filledRatings = 0;

                }
                p.FilledRatings = filledRatings;
                p.EmptyRatings = 5 - filledRatings;

                string cc = System.Web.HttpContext.Current.Request.Url.Host + "/Course/AboutCourses/" + System.Uri.EscapeDataString(item.Id.ToString().Trim()) + "/";
                string coursess = cc;
                if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                {
                    cc = "http://" + cc;
                }
                else
                {
                    cc = "https://" + cc;
                }
                p.GoogleShareURL = cc;
                //p.GoogleCourseDescription = System.Uri.EscapeDataString(item.ShortDescription + " .You can view this course on: " + cc);
                p = JsonConvert.SerializeObject(p);
                miniCourses.Add(p);
            }

            return Json(miniCourses);
        }

        //For Google Share
        public ActionResult AboutCourses(int? Id)
        {
            var courseList = db.Courses.Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.Courses = courseList;
            List<CourseContent> courseContents = db.CourseContent.Where(x => x.CourseId == Id).ToList();
            ViewBag.CourseContents = courseContents;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();

            //For Google+ Sharing
            ViewBag.PageTitle = courseList.CourseName + "-" + courseList.ShortDescription;
            ViewBag.PageDesc = courseList.ShortDescription;
            if (courseList.CourseImageLink != null)
            {
                if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                {
                    ViewBag.PageImg = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + courseList.Id + "/" + courseList.CourseImageLink;
                }
                else
                {
                    ViewBag.PageImg = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/CourseImg/" + courseList.Id + "/" + courseList.CourseImageLink;
                }
            }
            else
            {
                if (System.Web.HttpContext.Current.Request.IsSecureConnection == false)
                {
                    ViewBag.PageImg = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                }
                else
                {
                    ViewBag.PageImg = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/Images/noimage378X225.png";
                }
            }

            return View();
        }
    }

    public class CourseDetailsCategory
    {
        public int ID { get; set; }

        public string CategoryName { get; set; }
        public string CourseLevel { get; set; }
        public string CourseLanguage { get; set; }
        public string CourseType { get; set; }
        public string CourseAvailability { get; set; }

        public int CourseCount { get; set; }

        public string MainCourse { get; set; }
    }
}
