using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Millionlights.Models;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Millionlights.Controllers
{
    public class CertificationController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();

        // GET: /Certification/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View(db.Certifications.ToList());
        }

        public ActionResult ViewCertificate()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            return View();
        }
        [HttpPost]
        public ActionResult Index(Certification model)
        {
            var cd = new ContentDisposition
            {
                FileName = "Certification.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        // GET: /Certification/Details/5
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
            Certification certification = db.Certifications.Find(id);
            if (certification == null)
            {
                return HttpNotFound();
            }
            return View(certification);
        }
        [HttpPost]
        public JsonResult GetCertificationsDetails()
        {
            IEnumerable<Certification> CertificationDetails = db.Certifications.OrderBy(x => x.Id).ToList();
            List<dynamic> miniCertificationDetails = new List<dynamic>();
            foreach (var item in CertificationDetails)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.Name = item.Name;
                p.Objective = item.Objective;
                p.ShortDescription = item.ShortDescription;
                p.Price = item.Price;
                p.AvailableAttempts = item.AvailableAttempts;
                p.Benefits = item.Benifits;
                p.IsActive = item.IsActive;
                p = JsonConvert.SerializeObject(p);
                miniCertificationDetails.Add(p);
            }
            return Json(miniCertificationDetails);
        }
        // GET: /Certification/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partners = db.Partners.Where(X => X.IsActive == true).ToList();

            foreach (var item in partners)
            {
                partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.PartnerList = partnerList;

            List<SelectListItem> courseList = new List<SelectListItem>();
            IEnumerable<Course> courses = db.Courses.Where(X => X.IsActive == true).ToList();

            foreach (var item in courses)
            {
                courseList.Add(new SelectListItem() { Text = item.CourseName, Value = item.Id.ToString() });
            }
            ViewBag.CourseList = courseList;

            List<SelectListItem> catList = new List<SelectListItem>();
            IEnumerable<CourseCategory> category = db.CourseCategories.Where(X => X.IsActive == true).ToList();

            foreach (var item in category)
            {
                catList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CatList = catList;

            return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Certification certification,int? id,string userid)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (id == null)
            {
                certification.Name = Request["Name"];
                certification.ShortDescription = Request["ShortDescription"];
                certification.LongDescription = Request["LongDescription"];
                certification.Objective = Request["Objective"];
                certification.AvailableAttempts = Convert.ToInt32(Request["AvailableAttempts"]);
                certification.Price = Convert.ToDecimal(Request["Price"]);
                certification.CourseCategoryId = Convert.ToInt32(Request["CourseCategoryId"]);
                certification.Benifits = Request["Benifits"];
                certification.CreatedBy = Convert.ToInt32(userid);
                certification.ModifiedBy = Convert.ToInt32(userid);
                certification.CreatedOn = DateTime.Today;
                certification.ModifiedOn = DateTime.Today;
                certification.IsActive = true;
            }

                db.Certifications.Add(certification);
                db.SaveChanges();
                return RedirectToAction("Index");
        }

        // GET: /Certification/Edit/5
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
            Certification certification = db.Certifications.Find(id);
            if (certification == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> catList = new List<SelectListItem>();
            IEnumerable<CourseCategory> category = db.CourseCategories.Where(X => X.IsActive == true).ToList();

            foreach (var item in category)
            {
                catList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CatList = catList;
            return View(certification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Certification certificationDetails, int? id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var certificationDetails1 = db.Certifications.Find(certificationDetails.Id);

            certificationDetails1.Name = Request["Name"];
            certificationDetails1.ShortDescription = Request["ShortDescription"];
            certificationDetails1.LongDescription = Request["LongDescription"];

            certificationDetails1.AvailableAttempts = int.Parse(Request["AvailableAttempts"]);
            certificationDetails1.CourseCategoryId = int.Parse(Request["CourseCategoryId"]);
            certificationDetails1.Objective = Request["Objective"];
            certificationDetails1.Price = decimal.Parse(Request["Price"]);
            certificationDetails1.Benifits = Request["Benifits"];
            certificationDetails1.ModifiedOn = DateTime.Today;
            certificationDetails1.ModifiedBy = int.Parse(Session["UserID"].ToString());
            
            db.Entry(certificationDetails1).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Disabled(int? id, bool status)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Certification certificationDetails = db.Certifications.Find(id);
            if (status == true)
            {
                certificationDetails.IsActive = false;
            }
            else
            {
                certificationDetails.IsActive = true;
            }


            db.Entry(certificationDetails).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index");

        }
        public ActionResult DeleteMultipleCertification(List<SearchCertificationModel> model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Certification certificationModel = null;

            foreach (var cert in model)
            {
                certificationModel = db.Certifications.Where(x => x.Id == cert.Id).FirstOrDefault();
                certificationModel.IsActive = false;
                db.Entry(certificationModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("index", "Certification");
        }
        public class SearchCertificationModel
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

        public JsonResult GetStudentCertification()
        {
            var studentList = db.UsersCertificate.ToList();
            return Json(studentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEvidenceDetails(string certificateId, string courseId)
        {
            var certId=int.Parse(certificateId);
            var couId=int.Parse(courseId);
            var details = db.UserCertificateEvidenceDetails.Where(x => x.IsActive == true && x.IsUploaded == true && x.CourseId == couId && x.UsersCertificateId == certId).FirstOrDefault();
            dynamic evdDetails = new ExpandoObject();
            evdDetails.FirstName = details.FirstName;
            evdDetails.LastName = details.LastName;
            evdDetails.Address = details.Address;
            evdDetails.EvidenceType = db.CertificateEvidenceLkp.Where(x => x.Id == details.EvidenceId).FirstOrDefault().EvidenceName;
            evdDetails.EvidenceNo = details.EvidenceNumber;
            evdDetails.ProfileUrl = details.ImageUrl;
            evdDetails.EvidenceUrl = details.EvidenceUploadedPath;
            evdDetails.IssueDate = details.IssuedDateString;
            evdDetails.ExpiryDate = details.EvidenceExpiryString;
            evdDetails = JsonConvert.SerializeObject(evdDetails);
            return Json(evdDetails);
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
                    evidenceFound = false;
                }
            }
            return Json(evidenceFound);
        }
    }
}
