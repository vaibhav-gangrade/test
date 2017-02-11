using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Millionlights.Models;

namespace Millionlights.Controllers
{
    public class CareerController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /Career/
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Career career = db.Careers.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.message = TempData["Successmsg"];
            return View(career);
        }

        [HttpPost, ValidateInput(false), ValidateAntiForgeryToken]
        public ActionResult Create(Career careerModel, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Career career = new Career();

            if (id == null)
            {
                career.JobTitle = careerModel.JobTitle;
                career.Location = careerModel.Location;
                career.Experience = careerModel.Experience;
                career.Qualification = careerModel.Qualification;
                career.JobDescription = careerModel.JobDescription;
                career.TechnicalSkills = careerModel.TechnicalSkills;
                career.BusinessSkills = careerModel.BusinessSkills;
                career.Responsibilities = careerModel.Responsibilities;
                career.IsActive = true;
                career.CreatedOn = DateTime.Today;
                career.ModifiedOn = DateTime.Today;
                db.Careers.Add(career);
                db.SaveChanges();
                TempData["Successmsg"] = "Success";
            }
            else
            {
                career = db.Careers.Find(id);
                career.JobTitle = careerModel.JobTitle;
                career.Location = careerModel.Location;
                career.Experience = careerModel.Experience;
                career.Qualification = careerModel.Qualification;
                career.JobDescription = careerModel.JobDescription;
                career.TechnicalSkills = careerModel.TechnicalSkills;
                career.BusinessSkills = careerModel.BusinessSkills;
                career.Responsibilities = careerModel.Responsibilities;
                career.IsActive = true;
                career.ModifiedOn = DateTime.Today;
                db.Careers.Add(career);
                db.SaveChanges();
            }

            return RedirectToAction("Create");
        }

        public ActionResult Career()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.IsSuccess = TempData["Success"];
            }

            Career career = db.Careers.Where(X => X.IsActive == true).OrderByDescending(y => y.Id).FirstOrDefault();
            ViewBag.careerDetails = career;
            return View();
        }
	}
}