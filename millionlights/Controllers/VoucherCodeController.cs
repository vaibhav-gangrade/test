using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;


namespace Millionlights.Controllers
{
    public class VoucherCodeController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        // GET: /VoucherCode/
        public ActionResult VoucherList()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<VoucherCode> voucherCodeDetails = db.VoucherCode.OrderBy(X => X.Id).ToList();
            ViewBag.voucherDetails = voucherCodeDetails;
            return View();
        }
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<SelectListItem> partnerList = new List<SelectListItem>();
            IEnumerable<Partner> partners = db.Partners.Where(X=>X.IsActive==true).ToList();

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
            IEnumerable<CourseCategory> cat = db.CourseCategories.Where(X => X.IsActive == true).ToList();

            foreach (var item in cat)
            {
                catList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.CatList = catList;

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
            VoucherCode voucherCode = new VoucherCode();
            VoucherCourses voucherCourses = new VoucherCourses();
            if (id == null)
            {
                string[] voucherCodes = VoucherGen.GenerateVouchers(1);
                voucherCode.VouchCode = voucherCodes[0];
                voucherCode.VoucherType = Request["VoucherType"];
                voucherCode.PartnerID = Convert.ToInt32(Request["PartnerID"]);
                voucherCode.ExpiryDate = Convert.ToDateTime(Request["ExpiryDate"]);
                
                voucherCode.CreatedOn = DateTime.Today;
                voucherCode.CreatedByUserId = 1;
                voucherCode.Discount = Convert.ToDecimal(Request["Discount"]);
                voucherCode.AllowedCourses = Convert.ToInt32(Request["AllowedCourses"]);
                voucherCode.StatusId = 2;
                db.VoucherCode.Add(voucherCode);
                db.SaveChanges();
                int voucherId = voucherCode.Id;
                TempData["Successmsg"] = Constants.RecordSave;
                //Insert the record into Voucher Courses table

                string courses = Request["CourseId"];
                string[] courseList = courses.Split(',');
                for (int i = 0; i < courseList.Count(); i++)
                {
                    if (courseList[i] != "")
                    {
                        voucherCourses.VoucherId = voucherId;
                        voucherCourses.CourseId = Convert.ToInt32(courseList[i]);
                        voucherCourses.IsActivated = false;
                        db.VoucherCourses.Add(voucherCourses);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("VoucherList");

            //}
        }
        public ActionResult Edit(int? id)
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


            VoucherCode voucherCode = db.VoucherCode.Find(id);
            if (voucherCode == null)
            {
                return HttpNotFound();
            }
            return View(voucherCode);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VoucherCode voucher, int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            voucher = db.VoucherCode.Find(voucher.Id);
            voucher.VoucherType = Request["VoucherType"];
            voucher.PartnerID = Convert.ToInt32(Request["PartnerID"]);
            voucher.ExpiryDate = Convert.ToDateTime(Request["ExpiryDate"]);
            voucher.Discount = Convert.ToDecimal(Request["Discount"]);
            voucher.AllowedCourses = Convert.ToInt32(Request["AllowedCourses"]);
            voucher.StatusId = 2;
            db.Entry(voucher).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("VoucherList");

        }
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
            VoucherCode voucher = db.VoucherCode.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
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
            VoucherCode voucher = db.VoucherCode.Find(id);
            db.VoucherCode.Remove(voucher);
            db.SaveChanges();
            return RedirectToAction("VoucherList");
        }

        public ActionResult ActivateVoucherCoupon(string vcc, int userId)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.VoucherCode = vcc;
            var voucher = db.VoucherCode.Where(f => f.VouchCode==vcc).Count();
            bool isValid=false;
            if (voucher > 0)
            {
                isValid=true;
                ViewBag.ValidVoucher = isValid;
                var isActivated = db.VoucherCode.Where(m => m.ActivatedById != null && m.VouchCode == vcc).FirstOrDefault();
                var isMultiRedeem = db.Coupons.Where(a=>a.CouponCode==vcc).FirstOrDefault();
                if (isMultiRedeem != null && isMultiRedeem.MultiRedeem == true)
                {
                    ViewBag.isMultiRedeem = "true";
                }
                else if (isActivated!=null)
                {
                    ViewBag.IsActivated = "true";
                }
                else
                {
                    ViewBag.Discount = db.VoucherCode.Where(m =>m.VouchCode == vcc).FirstOrDefault().Discount;
                    var courses = db.VoucherCode.Join(db.VoucherCourses, vc => vc.Id, vcs => vcs.VoucherId, (vc, vcs) => new { vc, vcs })
                        .Where(A => A.vc.VouchCode == vcc)
                        .Select(B=>B.vcs)
                        .ToList();
                  
                    List<Course> courseList= new List<Course>();
                    foreach (var course in courses)
                    {
                        var crs = db.Courses.Find(course.CourseId);
                        courseList.Add(crs);
                    }
                    ViewBag.CourseList = courseList;
                    ViewBag.IsActivated = "false";
                    TempData["CourseList"] = courseList;
                }
            }
            else
            {
                isValid = false;
                ViewBag.ValidVoucher = isValid;
            }
            TempData["ValidVoucher"] = ViewBag.ValidVoucher;
            TempData["IsActivated"] = ViewBag.IsActivated;
            TempData["VoucherCode"] = ViewBag.VoucherCode;
            TempData["Discount"] = ViewBag.Discount;

            return Json("");
        }

        public ActionResult ActivateVoucher()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var courseList = TempData["CourseList"];
            var validVoucher = TempData["ValidVoucher"];
            var isActivated = TempData["IsActivated"];
            var voucherCode = TempData["VoucherCode"];
            var discount = TempData["Discount"];
            var u = TempData["UserId"];
            ViewBag.CourseList = courseList;
            ViewBag.ValidVoucher = validVoucher;
            ViewBag.IsActivated = isActivated;
            ViewBag.VoucherCode = voucherCode;
            ViewBag.Discount = discount;
            return View();
        }
        public ActionResult VoucherStatus()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Course c = (Course)TempData["course"];
            ViewBag.Course = c;
            return View();
        }
       
        public ActionResult VoucherStatusAct(int Id, int userId, string vc)
        {
            if (vc != null)
            {
                var voucerCode = db.VoucherCode.Join(db.VoucherCourses, a => a.Id, b => b.VoucherId, (a, b) => new { a, b })
                    .Where(X => X.a.VouchCode == vc && X.b.CourseId == Id)
                    .Select(X => X.a)
                    .FirstOrDefault();
                if (voucerCode != null)
                {
                    VoucherCode voucher = db.VoucherCode.Find(voucerCode.Id);
                    voucher.ActivatedById = userId;
                    voucher.CourseId = Id;
                    voucher.StatusId = 3;
                    db.Entry(voucher).State = EntityState.Modified;
                    db.SaveChanges();

                    VoucherCourses voucherCourses = db.VoucherCourses.Where(y => y.VoucherId == voucher.Id && y.CourseId == Id).FirstOrDefault();
                    voucherCourses.IsActivated = true;
                    db.Entry(voucherCourses).State = EntityState.Modified;
                    db.SaveChanges();

                    Course course = db.Courses.Where(c => c.Id == Id).FirstOrDefault();
                    ViewBag.Course = course;
                    TempData["course"] = course;
                }
            }
            return Json("");
        }
	}
    public class VoucherGen
    {
        private const int IN_BYTE_SIZE = 8;
        private const int OUT_BYTE_SIZE = 5;
        private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        private static string Base32Encode(byte[] data)
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


        private static DateTime RandomDay()
        {
            DateTime start = new DateTime(1500, 12, 3);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        public static string[] GenerateVouchers(int count)
        {
            Random random = new Random();
            DateTime timeValue = RandomDay();
            List<string> voucherCodes = new List<string>();

            for (int i = 0; i < count; ++i)
            {
                int rand = random.Next(3600) + 1;
                timeValue = timeValue.AddMinutes(rand);

                byte[] b = System.BitConverter.GetBytes(timeValue.Ticks);

                string base64Code = Base32Encode(b);

                string code = string.Format("{0}-{1}-{2}", base64Code.Substring(0, 4), base64Code.Substring(4, 4), base64Code.Substring(8, 5)).ToUpperInvariant();
                voucherCodes.Add(code);
                Console.WriteLine(code );
            }
            return voucherCodes.ToArray();
        }
    }
}