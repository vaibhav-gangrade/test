using Millionlights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Controllers
{
    public class StudentController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        // GET: /Student/
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult MyCourses(int Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<UsersCourses> userCourseList = db.UsersCourses.Where(x => x.UserId == Id).ToList();
            List<Course> courseListByUserId = new List<Course>();
            foreach (var course in userCourseList)
            {
                var courseTemp = db.Courses.Where(x => x.Id == course.CourseID).FirstOrDefault();
                courseListByUserId.Add(courseTemp);
            }
            ViewBag.CourseByCategories = courseListByUserId;
            return View();
        }
        public ActionResult MyOrders(int Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<Orders> ordersList = db.Orders.Where(x => x.UserID == Id).ToList();
            ViewBag.OrdersByUserID = ordersList;
            return View(ordersList);
        }
        public ActionResult MyCoupons(int Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<VoucherCode> couponsList = db.VoucherCode.Where(x => x.Id == Id).ToList();
            ViewBag.CouponsByUserID = couponsList;
            return View(couponsList);
        }
        public ActionResult MyExams()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            return View();
        }
	}
}