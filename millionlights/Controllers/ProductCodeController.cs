using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Controllers
{
    public class ProductCodeController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /ProductCode/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<ProductCode> prodCodeList = db.ProductCode.ToList();
            ViewBag.ProductCodeList = prodCodeList;
            return View();
        }
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partners = db.Partners.ToList();

            foreach (var item in partners)
            {
                partnerList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.PartnerList = partnerList;

            List<SelectListItem> courseList = new List<SelectListItem>();
            IEnumerable<Course> courses = db.Courses.ToList();

            foreach (var item in courses)
            {
                courseList.Add(new SelectListItem() { Text = item.CourseName, Value = item.Id.ToString() });
            }
            ViewBag.CourseList = courseList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            ProductCode productCode = new ProductCode();
            if (id == null)
            {
                productCode.ProdCode = "A5432";
                string partner = Request["PartnerID"];
                productCode.PartnerID = Convert.ToInt32(partner);

                //productCode.PartnerID = Convert.ToInt32(Request["PartnerID"]);
                string course = Request["CourseID"];
                productCode.CourseID = Convert.ToInt32(course);

                //productCode.CourseID = Convert.ToInt32(Request["CourseID"]);
                productCode.Fees = Convert.ToDecimal(Request["Fees"]);
                productCode.Discount = Convert.ToDecimal(Request["Discount"]);
                productCode.NOfAllowedCourses = Convert.ToInt32(Request["NOfAllowedCourses"]);
                productCode.CreatedBy = User.Identity.Name;
                productCode.CreatedOn = DateTime.Today;

                db.ProductCode.Add(productCode);
                db.SaveChanges();
                TempData["Successmsg"] = Constants.RecordSave;
            }
            return RedirectToAction("Index");

            //}
        }
    }
}