using Millionlights.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace Millionlights.Controllers
{
    public class CATCController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /CATC/
        public ActionResult SearchExamCenters()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            return View();
        }
        public ActionResult Index(int ? page)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<CATS> CATCList = db.Cats.OrderBy(X => X.Id).ToPagedList(pageIndex, pageSize);
            return View(CATCList);
        }
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            CATS cat = new CATS();
            return View(cat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CATS cats = new CATS();
            if (id == null)
            {
                cats.CustomerId = Convert.ToInt32(Request["CustomerId"]);
                cats.CustomerName = Request["CustomerName"];
                cats.CustomerTypeDesc = Request["CustomerTypeDesc"];
                cats.AccountStatus = Request["AccountStatus"];
                cats.Sector = Request["Sector"];
                cats.CountryName = Request["CountryName"];
                cats.ZipCode = Request["ZipCode"];
                cats.AddressLine1 = Request["AddressLine1"];
                cats.AddressLine2 = Request["AddressLine2"];
                cats.WebAddress = Request["WebAddress"];
                cats.Phone = Request["Phone"];
                cats.City = Request["City"];
                cats.CreatedBy = Session["UserID"].ToString();
                cats.CreatedOn = DateTime.Today;
                cats.ModifiedBy = Session["UserID"].ToString();
                cats.ModifiedOn = DateTime.Today;
                db.Cats.Add(cats);
                db.SaveChanges();
                TempData["Successmsg"] = "Record saved successfully";
            }
            else
            {
                cats = db.Cats.Find(id);
                cats.CustomerId = Convert.ToInt32(Request["CustomerId"]);
                cats.CustomerName = Request["CustomerName"];
                cats.CustomerTypeDesc = Request["CustomerTypeDesc"];
                cats.AccountStatus = Request["AccountStatus"];
                cats.Sector = Request["Sector"];
                cats.CountryName = Request["CountryName"];
                cats.ZipCode = Request["ZipCode"];
                cats.AddressLine1 = Request["AddressLine1"];
                cats.AddressLine2 = Request["AddressLine2"];
                cats.WebAddress = Request["WebAddress"];
                cats.Phone = Request["Phone"];
                cats.City = Request["City"];
                cats.CreatedBy = Session["UserID"].ToString();
                cats.CreatedOn = DateTime.Today;
                cats.ModifiedBy = Session["UserID"].ToString();
                cats.ModifiedOn = DateTime.Today;
                db.Entry(cats).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
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
            CATS cats = db.Cats.Find(id);
            if (cats == null)
            {
                return HttpNotFound();
            }
            return View(cats);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerId,CustomerName,CustomerTypeDesc,AccountStatus,Sector,CountryName,ZipCode,AddressLine1,AddressLine2,WebAddress,Phone,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn")] CATS cats)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            if (ModelState.IsValid)
            {
                db.Entry(cats).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cats);
        }
	}
}