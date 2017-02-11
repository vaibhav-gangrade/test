using Millionlights.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Controllers
{
    public class NotificationController : Controller
    {
        //
        // GET: /Notification/
        private MillionlightsContext db = new MillionlightsContext();
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult Notifications()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            return View();
        }
        [HttpPost]
        public JsonResult ShowNotification(int eval)
        {
            UserNotitification NoteDetails = db.UserNotitifications.Find(eval);
            NoteDetails.IsRead = true;
            NoteDetails.ReadDate = DateTime.Now;
            db.Entry(NoteDetails).State = EntityState.Modified;
            db.SaveChanges();
            return Json("");
        }
        public ActionResult AllNotification(int? page, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int pageSize = 25;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<UserNotitification> notification = null;
            notification = db.UserNotitifications.Where(x => x.Receiver == id && x.IsAlert==false).OrderByDescending(x => x.Id).ToPagedList(pageIndex, pageSize);
            return PartialView("_Notification", notification);
        }
        public ActionResult EmailReports()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<SelectListItem> statusList= new List<SelectListItem>();
            IEnumerable<NotificationStatus> notStatus = db.NotificationStatus.Where(X => X.IsActive == true).ToList();

            foreach (var item in notStatus)
            {
                statusList.Add(new SelectListItem() { Text = item.Status, Value = item.Id.ToString() });
            }
            ViewBag.StatusList = statusList;
         
            return View();
        }
        public ActionResult SmsReports()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<SelectListItem> statusList = new List<SelectListItem>();
            IEnumerable<NotificationStatus> notStatus = db.NotificationStatus.Where(X => X.IsActive == true).ToList();

            foreach (var item in notStatus)
            {
                statusList.Add(new SelectListItem() { Text = item.Status, Value = item.Id.ToString() });
            }
            ViewBag.StatusList = statusList;

            return View();
        }
        [HttpPost]
        public JsonResult GetEmails(string dateRange, string statusId)
        {
            DateTime? fromDate1 = new DateTime();
            DateTime? toDate1 = new DateTime();
            if (dateRange != "")
            {
                var dates = dateRange.Split('-');
                var toDate = dates[1];
                var fromDate = dates[0];
                fromDate1 = DateTime.Parse(fromDate);
                toDate1 = DateTime.Parse(toDate).AddDays(1).AddSeconds(-1);
            }
            else
            {
                fromDate1 = null;
                toDate1 = null;
            }
            int? status = null;
            if (statusId != null && statusId != "")
            {
                status = int.Parse(statusId);
            }
            List<GetUsersEmailAndSMSResult> result = null;
            MillionlightsDataContext md = new MillionlightsDataContext();
            result = md.GetUsersEmailAndSMS(fromDate1, toDate1, "true", status).ToList();
            //fromDate1 = DateTime.Parse(fromDate1);
            //toDate1 = DateTime.Parse(toDate1).AddDays(1).AddSeconds(-1); ;
            //var emailList = db.UserNotitifications.Join(db.Users, a => a.Receiver, b => b.UserId, (a, b) => new { a, b })
            //    .Where((x =>(String.IsNullOrEmpty(dateRange) || (x.a.MailDate >= fromDate1 && x.a.MailDate <= toDate1))
            //            && (String.IsNullOrEmpty(statusId.ToString()) || x.a.NotificationStatusId == statusId)))
            //            .Select(x => x.a).ToList();


            //return Json(result, JsonRequestBehavior.AllowGet);
            var resultSet = Json(result, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }
        [HttpPost]
        public JsonResult GetSMS(string dateRange, string statusId)
        {
            //DateTime fromDate1 = new DateTime();
            //DateTime toDate1 = new DateTime();
            //if (dateRange != "")
            //{
            //    var dates = dateRange.Split('-');
            //    var toDate = dates[1];
            //    var fromDate = dates[0];
            //    fromDate1 = DateTime.Parse(fromDate);
            //    toDate1 = DateTime.Parse(toDate);

            //}
            //fromDate1 = DateTime.Parse(fromDate1.ToShortDateString());
            //toDate1 = DateTime.Parse(toDate1.ToShortDateString()).AddDays(1).AddSeconds(-1); ;
            //var smsList = db.UserNotitifications.Join(db.Users, a => a.Receiver, b => b.UserId, (a, b) => new { a, b }).Where((x =>
            //            (String.IsNullOrEmpty(dateRange) || (x.a.SMSDate >= fromDate1 && x.a.SMSDate <= toDate1))
            //            && (String.IsNullOrEmpty(statusId.ToString()) || x.a.NotificationStatusId == statusId))).Select(x => x.a).ToList();
            //return Json(smsList, JsonRequestBehavior.AllowGet);
            DateTime? fromDate1 = new DateTime();
            DateTime? toDate1 = new DateTime();
            if (dateRange != "")
            {
                var dates = dateRange.Split('-');
                var toDate = dates[1];
                var fromDate = dates[0];
                fromDate1 = DateTime.Parse(fromDate);
                toDate1 = DateTime.Parse(toDate).AddDays(1).AddSeconds(-1);
            }
            else
            {
                fromDate1 = null;
                toDate1 = null;
            }
            int? status = null;
            if (statusId != null && statusId != "")
            {
                status = int.Parse(statusId);
            }
            List<GetUsersEmailAndSMSResult> result = null;
            MillionlightsDataContext md = new MillionlightsDataContext();
            result = md.GetUsersEmailAndSMS(fromDate1, toDate1, "false", status).ToList();
            //return Json(result, JsonRequestBehavior.AllowGet);
            var resultSet = Json(result, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }
	}
}