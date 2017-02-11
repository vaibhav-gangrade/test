using Millionlights.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Millionlights.Controllers
{
    public class CourseCategoriesController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /CourseCategories/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View();
        }
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseCategory categories, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                categories.Name = Request["Name"];
                categories.IsActive = true;
            }

            db.CourseCategories.Add(categories);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
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
            CourseCategory categories = db.CourseCategories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseCategory categoriesDetails, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var categories = db.CourseCategories.Find(categoriesDetails.Id);
            categories.Name = Request["Name"];
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Disabled(int? id, bool status)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CourseCategory categories = db.CourseCategories.Find(id);
            if (status == true)
            {
                categories.IsActive = false;
            }
            else
            {
                categories.IsActive = true;
            }
            db.Entry(categories).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        [HttpPost]
        public JsonResult GetCategoriesDetails()
        {
            
            IEnumerable<CourseCategory> CategoriesDetails = db.CourseCategories.OrderBy(x => x.Id).ToList();
            List<dynamic> miniCategoriesDetails = new List<dynamic>();
            foreach (var item in CategoriesDetails)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.Name = item.Name;
                p.IsActive = item.IsActive;
                p = JsonConvert.SerializeObject(p);
                miniCategoriesDetails.Add(p);
            }
            return Json(miniCategoriesDetails);
        }
        public ActionResult DeleteMultipleCategories(List<SearchCategoriesModel> model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CourseCategory courseCategoryModel = null;

            foreach (var cat in model)
            {
                courseCategoryModel = db.CourseCategories.Where(x => x.Id == cat.Id).FirstOrDefault();
                courseCategoryModel.IsActive = false;
                db.Entry(courseCategoryModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("index", "Categories");
        }
        public class SearchCategoriesModel
        {
            public int Id { get; set; }
        }

	}
}