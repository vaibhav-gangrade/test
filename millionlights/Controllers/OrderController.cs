using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Millionlights.Models;
using System.Dynamic;
using System.Configuration;
using System.IO;
using Millionlights.EmailService;
using CCA.Util;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Millionlights.Controllers
{
    public class OrderController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();

        // GET: /Order/
        public ActionResult Index(int? page)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<SelectListItem> courseAvlList = new List<SelectListItem>();
            IEnumerable<Course> courseAvailabilities = db.Courses.Where(x => x.IsActive == true).ToList();

            foreach (var item in courseAvailabilities)
            {
                courseAvlList.Add(new SelectListItem() { Text = item.CourseName, Value = item.Id.ToString() });
            }
            ViewBag.CourseAvailability = courseAvlList;
            List<SelectListItem> orderstatusAvlList = new List<SelectListItem>();
            IEnumerable<OrderStatus> orderStatusAvailabilities = db.OrderStatus.ToList();

            foreach (var item in orderStatusAvailabilities)
            {
                orderstatusAvlList.Add(new SelectListItem() { Text = item.Status, Value = item.OrderStatusID.ToString() });
            }
            ViewBag.AvailOrderStatusList = orderstatusAvlList;
            ViewBag.Messages = Millionlights.Models.Constants.Messages();
            return View();
        }

        [HttpPost]
        public ActionResult Index(OrderManagement model)
        {
            var cd = new ContentDisposition
            {
                FileName = "OrderListExportData.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetOrderDetails(string dateRange, string status, string CourseName)
        {
            DateTime? fromDate1 = new DateTime();
            DateTime? toDate1 = new DateTime();
            if (dateRange != "null")
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
            int? Status = null;
            if (status != "null")
            {
                Status = int.Parse(status);
            }


            //var orderInfo = db.Users.Join(db.ItemsOrders.Join(db.Orders.Join(db.OrderStatus, IO => IO.OrderStatusID, OS => OS.OrderStatusID, (IO, OS) => new { IO, OS }), IOI => IOI.OrderID, O => O.IO.OrderID, (IOI, O) => new { IOI, O }), U => U.UserId, OIO => OIO.O.IO.UserID, (OIO, U) => new { OIO, U }).ToList();
            var orderInfo = db.Users.
                Join(db.UsersDetails.
                Join(db.ItemsOrders.
                Join(db.Orders.
                Join(db.OrderStatus,
                IO => IO.OrderStatusID,
                OS => OS.OrderStatusID,
                (IO, OS) => new { IO, OS }),
                IOI => IOI.OrderID,
                O => O.IO.OrderID,
                (IOI, O) => new { IOI, O }),
                us => us.UserId,
                ud => ud.O.IO.UserID,
                (us, ud) => new { us, ud }),
                U => U.UserId,
                OIO => OIO.ud.O.IO.UserID,
                (OIO, U) => new { OIO, U }).ToList();
            //.Where(x => x.U.us.FirstName == "010")
            //var orderInfoTemp1 = orderInfo.Join(db.Courses, o => o.U.ud.IOI.CourseId, c => c.Id, (o, c) => new { o, c }).ToList();
            //orderInfo = orderInfoTemp1;
            // .Where(x => x.U.us.FirstName == "Priyanka")
            var orderdetailsList = orderInfo.Join(db.Courses, o => o.U.ud.IOI.CourseId, c => c.Id, (o, c) => new { o, c })
                         .Where(x => (String.IsNullOrEmpty(Status.ToString()) || (x.o.U.ud.O.OS.OrderStatusID == Status))
                         && ((String.IsNullOrEmpty(fromDate1.ToString()) && String.IsNullOrEmpty(toDate1.ToString())) || (x.o.U.ud.O.IO.OrderedDatetime >= fromDate1 && x.o.U.ud.O.IO.OrderedDatetime <= toDate1))
                         && (String.IsNullOrEmpty(CourseName) || (CourseName.Contains(x.o.U.ud.IOI.CourseId.ToString())))).ToList();
            IEnumerable<OrderManagement> OrderDetails = null;
            List<OrderManagement> orderManagementDetails = new List<OrderManagement>();
            var orderNo = string.Empty;
            #region
            //foreach (var orders in orderInfo)
            //{
            //    OrderManagement orderList = new OrderManagement();
            //    string coursename = string.Empty;
            //    if (orderNo != orders.U.O.IO.OrderNumber)
            //    {
            //        orderList.OrderID = orders.U.O.IO.OrderID;
            //        orderList.UserName = orders.OIO.UserName;
            //        orderList.OrderedDatetime = orders.U.O.IO.OrderedDatetime;
            //        orderList.TotalItems = orders.U.O.IO.TotalItems;
            //        orderList.CourseId = orders.U.IOI.CourseId;
            //        orderList.OrderNumber = orders.U.O.IO.OrderNumber;
            //        orderList.TotalPrice = orders.U.O.IO.TotalPrice;
            //        orderList.OrderStatusID = orders.U.O.IO.OrderStatusID;
            //        orderList.OrderStatus = orders.U.O.OS.Status;
            //        orderNo = orders.U.O.IO.OrderNumber;
            //        foreach (var orderName in orderInfo)
            //        {
            //            if (orderName.U.O.IO.OrderID == orders.U.O.IO.OrderID)
            //            {
            //                Course course = db.Courses.Where(x => x.Id == orderName.U.IOI.CourseId).FirstOrDefault();
            //                if (course != null)
            //                {
            //                    if (!string.IsNullOrEmpty(course.CourseName))
            //                    {
            //                        if (!string.IsNullOrEmpty(coursename))
            //                        {
            //                            coursename = course.CourseName + ',' + " " + coursename;
            //                        }
            //                        else
            //                        {
            //                            coursename = course.CourseName;
            //                        }
            //                    }
            //                }

            //            }
            //            orderList.CourseName = coursename;
            //        }
            //        orderList.IsActive = orders.U.O.IO.IsActive;
            //        orderManagementDetails.Add(orderList);
            //    }
            //}
            #endregion

            foreach (var orders in orderdetailsList)
            {
                OrderManagement orderList = new OrderManagement();
                string coursename = string.Empty;

                orderList.OrderedUserName = orders.o.U.us.FirstName + " " + orders.o.U.us.LastName;
                orderList.OrderID = orders.o.U.ud.O.IO.OrderID;
                orderList.UserName = orders.o.OIO.UserName;
                orderList.OrderedDatetime = orders.o.U.ud.O.IO.OrderedDatetime;
                orderList.TotalItems = orders.o.U.ud.O.IO.TotalItems;
                orderList.CourseId = orders.o.U.ud.IOI.CourseId;
                orderList.OrderNumber = orders.o.U.ud.O.IO.OrderNumber;
                orderList.TotalPrice = orders.o.U.ud.O.IO.TotalPrice;
                orderList.OrderStatusID = orders.o.U.ud.O.IO.OrderStatusID;
                orderList.OrderStatus = orders.o.U.ud.O.OS.Status;
                orderNo = orders.o.U.ud.O.IO.OrderNumber;

                orderList.CourseName = orders.c.CourseName;
                orderList.IsActive = orders.o.U.ud.O.IO.IsActive;
                int couponId;
                couponId = orders.o.U.ud.IOI.CouponId;
                var couponData = db.Coupons.Where(x => x.Id == couponId).FirstOrDefault();
                if (couponData != null)
                {
                    orderList.CouponCode = couponData.CouponCode;
                    orderList.CouponPartnerName = couponData.PartnerName;
                }
                else
                {
                    orderList.CouponCode = "";
                    orderList.CouponPartnerName = "";
                }
                orderManagementDetails.Add(orderList);

                // Commented this line of code to display the Individual order by Archana 25.11.2016

                ////if (orderNo != orders.o.U.ud.O.IO.OrderNumber)  
                //{
                //    orderList.OrderedUserName = orders.o.U.us.FirstName + " " + orders.o.U.us.LastName;
                //    orderList.OrderID = orders.o.U.ud.O.IO.OrderID;
                //    orderList.UserName = orders.o.OIO.UserName;
                //    orderList.OrderedDatetime = orders.o.U.ud.O.IO.OrderedDatetime;
                //    orderList.TotalItems = orders.o.U.ud.O.IO.TotalItems;
                //    orderList.CourseId = orders.o.U.ud.IOI.CourseId;
                //    orderList.OrderNumber = orders.o.U.ud.O.IO.OrderNumber;
                //    orderList.TotalPrice = orders.o.U.ud.O.IO.TotalPrice;
                //    orderList.OrderStatusID = orders.o.U.ud.O.IO.OrderStatusID;
                //    orderList.OrderStatus = orders.o.U.ud.O.OS.Status;
                //    orderNo = orders.o.U.ud.O.IO.OrderNumber;

                //    // Commented by Archana 25.11.2016

                //    //foreach (var orderName in orderdetailsList)
                //    //{
                //    //    if (orderName.o.U.ud.O.IO.OrderID == orders.o.U.ud.O.IO.OrderID)
                //    //    {
                //    //        Course course = db.Courses.Where(x => x.Id == orderName.o.U.ud.IOI.CourseId).FirstOrDefault();
                //    //        if (course != null)
                //    //        {
                //    //            if (!string.IsNullOrEmpty(course.CourseName))
                //    //            {
                //    //                if (!string.IsNullOrEmpty(coursename))
                //    //                {
                //    //                    coursename = course.CourseName + ',' + " " + coursename;
                //    //                }
                //    //                else
                //    //                {
                //    //                    coursename = course.CourseName;
                //    //                }
                //    //            }
                //    //        }

                //    //    }
                //    //    orderList.CourseName = coursename;
                //    //}

                //    // Done by Archana 25.11.2016
                //    orderList.CourseName = orders.c.CourseName;
                //    orderList.IsActive = orders.o.U.ud.O.IO.IsActive;
                //    orderManagementDetails.Add(orderList);
                //}
            }
            OrderDetails = orderManagementDetails;
            //return Json(OrderDetails, JsonRequestBehavior.AllowGet);
            var resultSet = Json(OrderDetails, JsonRequestBehavior.AllowGet);
            resultSet.MaxJsonLength = int.MaxValue;
            return resultSet;
        }

        public ActionResult Cancelled(int? OrderID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Orders orderDetail = db.Orders.Find(OrderID);
            orderDetail.OrderStatusID = 5;
            orderDetail.OrderStatus = "Cancelled";
            db.Entry(orderDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
            //string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
            ////Prashant Sir
            ////EdxCourseAPI courseAPI = new EdxCourseAPI(baseuri, secretKey);
            //User user = db.Users.Where(u => u.UserId == orderDetail.UserID).FirstOrDefault();

            var items = db.ItemsOrders.Where(x => x.OrderID == OrderID).ToList();
            foreach (var item in items)
            {
                UsersCourses userCourse = db.UsersCourses.Where(x => x.CourseID == item.CourseId).FirstOrDefault();
                userCourse.IsActive = false;
                db.Entry(userCourse).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Refunded(int? OrderID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Orders orderDetail = db.Orders.Find(OrderID);
            orderDetail.OrderStatusID = 6;
            orderDetail.OrderStatus = "Refunded";
            db.Entry(orderDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<Course> course = (List<Course>)TempData["courseDetails"];
            var username = TempData["UserName"];
            var u = Session["UserId"];
            var userId = Convert.ToInt32(u);
            ViewBag.courseInfo = course;
            ViewBag.userId = u;

            var walletAmount = db.UserWallets.Where(x => x.UserId == userId && x.IsActive == true).FirstOrDefault();
            if (walletAmount != null)
            {
                ViewBag.WalletAmount = walletAmount.FinalAmountInWallet.ToString();
            }
            //var userCourses = db.UsersCourses.Where(X => X.CourseID == 6 && X.UserId == userId);
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

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderNumber,UserID,TotalItems,TotalPrice,ShippingAddress1,ShippingAddress2,ShippingCity,ShippingState,ShippingCountry,ShippingZipCode,BillingAddress1,BillingAddress2,BillingCity,BillingState,BillingCountry,BillingZipCode,OrderStatusID,OrderedDatetime,OrderModifiedOn,IsActive")] Orders orders)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orders);
        }

        // GET: /Order/Edit/5
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
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderNumber,UserID,TotalItems,TotalPrice,ShippingAddress1,ShippingAddress2,ShippingCity,ShippingState,ShippingCountry,ShippingZipCode,BillingAddress1,BillingAddress2,BillingCity,BillingState,BillingCountry,BillingZipCode,OrderStatusID,OrderedDatetime,OrderModifiedOn,IsActive")] Orders orders)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orders);
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
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: /Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Orders orders = db.Orders.Find(id);
            db.Orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult StoreCourseInfo(string[] Id, int uId)
        {
            List<Course> courseDetails = new List<Course>();
            for (int i = 0; i < Id.Length; i++)
            {
                var courseDt = db.Courses.Find(Convert.ToInt32(Id[i]));
                courseDetails.Add(courseDt);
            }
            string couponCode = string.Empty;
            if (HttpContext.Request.Cookies["CouponCode"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                couponCode = cookie.Value;
                var CountActivatedCoupon = db.Coupons.Join(db.BenifitTypes, a => a.BenifitId, b => b.BenifitId, (a, b) => new { a, b })
                                        .Where(A => A.a.CouponCode == couponCode).FirstOrDefault();
                if (CountActivatedCoupon != null)
                {
                    foreach (var item in courseDetails)
                    {
                        if (CountActivatedCoupon.b.BenifitId == 1)
                        {
                            item.BasePrice = (item.BasePrice) - (item.BasePrice * (CountActivatedCoupon.a.DiscountPrice / 100));
                        }
                    }
                }
            }
            ViewBag.courseInfo = courseDetails;
            int usId = uId;
            User userdetails = db.Users.Where(x => x.UserId == usId).FirstOrDefault();
            ViewBag.userName = userdetails.UserName;
            ViewBag.userId = uId;
            TempData["courseDetails"] = courseDetails;
            TempData["UserName"] = userdetails.UserName;
            TempData["UserId"] = uId;
            return Json("");
        }
        [HttpPost]
        public void PostProcessPayment(FormCollection form)
        {
            decimal orderPrice;
            string OrderNumber;
            string checkOutCourses = form["CheckCourse"].ToString();
            string userId = form["UserId"].ToString();
            string orderAmount = form["TotalAmount"].ToString();

            SaveOrderDetails(checkOutCourses, userId, orderAmount, out orderPrice, out OrderNumber);
            var orderTotalPrice = orderPrice;
            var orderNumber = OrderNumber;
            string CourseIds = checkOutCourses;
            Session["CheckoutCourseIds"] = CourseIds;
            TempData["CheckoutCourseIds"] = CourseIds;

            //User data to display on CCAvenue gateway page
            int uId = int.Parse(form["UserId"]);
            var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
            var userName = userRegister.b.FirstName + userRegister.b.LastName;
            var address1 = userRegister.b.AddressLine1;
            var city = userRegister.b.City;
            var state = userRegister.b.State;
            var country = userRegister.b.Country;
            var pinCode = userRegister.b.ZipCode;
            var phoneNo = userRegister.b.PhoneNumber;
            var userEmailID = userRegister.a.EmailId;

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Write("<html>");
            System.Web.HttpContext.Current.Response.Write("<body  onload=\"document.customerData.submit();\"><form style='display:none;' name=\"customerData\" method=\"post\" action = \"/order/CCAvenue\"");
            System.Web.HttpContext.Current.Response.Write("<table align='center' width='300px'>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>TID:</td><td><input type='text' name='tid' id='tid' readonly /></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Id:</td><td><input type='text' name='merchant_id' id='merchant_id' value='80592'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Order Id:</td><td><input type='text' name='order_id' value='" + orderNumber + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Amount:</td><td><input type='text' name='amount' value='" + orderTotalPrice + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Currency:</td><td><input type='text' name='currency' value='INR'/></td></tr>");
            //System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Redirect URL:</td><td><input type='text' name='redirect_url' value='http://localhost:8094/Order/Success'/></td></tr>");
            //System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Cancel URL:</td><td><input type='text' name='cancel_url' value='http://localhost:8094/Order/Details'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Redirect URL:</td><td><input type='text' name='redirect_url' value='https://www.millionlights.org/Order/Success'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Cancel URL:</td><td><input type='text' name='cancel_url' value='https://www.millionlights.org/Order/Details'/></td></tr>");
            //System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Redirect URL:</td><td><input type='text' name='redirect_url' value='http://www.millionlights.in/Order/Success'/></td></tr>");
            //System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Cancel URL:</td><td><input type='text' name='cancel_url' value='http://www.millionlights.in/Order/Details'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Name:</td><td><input type='text' name='billing_name' value='" + userName + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Address:</td><td><input type='text' name='billing_address' value='" + address1 + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing City:</td><td><input type='text' name='billing_city' value='" + city + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing State:</td><td><input type='text' name='billing_state' value='" + state + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Zip:</td><td><input type='text' name='billing_zip' value='" + pinCode + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Country:</td><td><input type='text' name='billing_country' value='" + country + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Tel:</td><td><input type='text' name='billing_tel' value='" + phoneNo + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Billing Email:</td><td><input type='text' name='billing_email' value='" + userEmailID + "'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping Name:</td><td><input type='text' name='delivery_name' value='Chaplin'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping Address:</td><td><input type='text' name='delivery_address' value='room no.701 near bus stand'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping City:</td><td><input type='text' name='delivery_city' value='Hyderabad'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping State:</td><td><input type='text' name='delivery_state' value='Andhra'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping Zip:</td><td><input type='text' name='delivery_zip' value='425001'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping Country:</td><td><input type='text' name='delivery_country' value='India'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Shipping Tel:</td><td><input type='text' name='delivery_tel' value='9896426054'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Param1:</td><td><input type='text' name='merchant_param1' value='additional Info.'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Param2:</td><td><input type='text' name='merchant_param2' value='additional Info.'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Param3:</td><td><input type='text' name='merchant_param3' value='additional Info.'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Param4:</td><td><input type='text' name='merchant_param4' value='additional Info.'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Merchant Param5:</td><td><input type='text' name='merchant_param5' value='additional Info.'/></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Promo Code:</td><td><input type='text' name='promo_code' /></td></tr>");
            System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'>Customer Id:</td><td><input type='text' name='customer_identifier' /></td></tr>");
            //System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'></td><td><input type='submit' value='Checkout' /></td></tr>");
            System.Web.HttpContext.Current.Response.Write("</table></form>");
            System.Web.HttpContext.Current.Response.Write("</body></html>");
            System.Web.HttpContext.Current.Response.End();


        }
        //[HttpPost]
        //public void PostProcessPayment(FormCollection form)
        //{
        //    decimal orderPrice;
        //    string OrderNumber;
        //    SaveOrderDetails(form, out orderPrice, out OrderNumber);
        //    var orderTotalString = orderPrice;
        //    var orderNumber = OrderNumber;
        //    ////DirecPay Test Environment
        //    //var merchandID = "200904281000001";
        //    //var collaborationID = "TOML";
        //    //var operatingMode = "DOM";
        //    //var country = "IND";
        //    //var currency = "INR";
        //    //var KeySize = 128;
        //    //var CompleteEncodedKey = "qcAHa6tt8s0l5NN7UWPVAQ==";

        //    //DirecPay Production Environment
        //    var merchandID = "201504271000002";
        //    var collaborationID = "DirecPay";
        //    var operatingMode = "DOM";
        //    var country = "IND";
        //    var currency = "INR";
        //    var KeySize = 128;
        //    var CompleteEncodedKey = "vtSZWfEKd15/kYvnX0NXZA==";
        //    int uId = int.Parse(form["UserId"]);
        //    var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
        //    var userName = userRegister.b.FirstName + userRegister.b.LastName;
        //    var address1 = userRegister.b.AddressLine1;
        //    var city = userRegister.b.City;
        //    var state = userRegister.b.State;
        //    var pinCode = userRegister.b.ZipCode;
        //    var phoneNo = userRegister.b.PhoneNumber;
        //    var userEmailID = userRegister.a.EmailId;

        //    string gateWayURL = System.Configuration.ConfigurationManager.AppSettings["gateWayURL"];
        //    //string returnUrl = System.Configuration.ConfigurationManager.AppSettings["returnUrl"];
        //    string absUri = Request.UrlReferrer.AbsoluteUri;
        //    string[] vb = absUri.Split(new string[] { "/Details" }, StringSplitOptions.None);
        //    string returnUrl = vb[0] + "/Success";
        //    string cancelReturnUrl = vb[0] + "/Fail";

        //    string requestString = merchandID + "|" + operatingMode + "|" + country + "|" + currency + "|" + orderTotalString + "|" + orderNumber + "|None|" + returnUrl + "|" + cancelReturnUrl + "|" + collaborationID + "";
        //    string requestParamaterEncrypt = AES128Bit.Encrypt(requestString, CompleteEncodedKey, KeySize);
        //    string reqBill = userName + "|" + address1 + "|" + city + "|" + state + "|" + pinCode + "|IN|91|022|00000000|" + phoneNo + "| " + userEmailID + " |test transaction for direcpay";
        //    string reqBillParameterEncrypt = AES128Bit.Encrypt(reqBill, CompleteEncodedKey, KeySize);
        //    string reqShipping = userName + "|" + address1 + "|" + city + "|" + state + "|" + pinCode + "|IN|91|022|00000000|" + phoneNo;
        //    string reqShippingEncrypt = AES128Bit.Encrypt(reqShipping, CompleteEncodedKey, KeySize);


        //    System.Web.HttpContext.Current.Response.Clear();
        //    System.Web.HttpContext.Current.Response.Write("<html>");
        //    System.Web.HttpContext.Current.Response.Write("<body onload=\"document.frmPost.submit();\"><form name=\"frmPost\" method=\"post\" action = \"" + gateWayURL + "\"");
        //    System.Web.HttpContext.Current.Response.Write("<table align='center' width='300px'>");
        //    System.Web.HttpContext.Current.Response.Write("<tr><td width='120px'><b>After Encrypt >></b></td>");
        //    System.Web.HttpContext.Current.Response.Write("<td width='20px'><input type='hidden' name='requestparameter' value='" + requestParamaterEncrypt + "'/>");
        //    System.Web.HttpContext.Current.Response.Write("<input type='hidden; id='merchantId' name='merchantId' value='" + merchandID + "' runat='server'/></td>");
        //    System.Web.HttpContext.Current.Response.Write("<td><input type='hidden' id='billingDtls' name='billingDtls' value='" + reqBillParameterEncrypt + "' runat='server' />");
        //    System.Web.HttpContext.Current.Response.Write("<input type='hidden' id='shippingDtls' name='shippingDtls' value='" + reqShippingEncrypt + "' runat='server' /></td>");
        //    System.Web.HttpContext.Current.Response.Write("</tr></table></form>");
        //    System.Web.HttpContext.Current.Response.Write("</body></html>");
        //    System.Web.HttpContext.Current.Response.End();
        //}
        public void CouponSuccessEmail(string cName, long orderNo, DateTime orderDate, decimal totalPrice, decimal unitPrice, string cNameForEmail)
        {
            string recipientsEmail = null;
            string firstName = "";
            string lastName = "";
            var userId = Convert.ToInt32(Session["UserId"]);
            var userReg = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == userId).FirstOrDefault();
            if (userReg != null)
            {
                recipientsEmail = userReg.a.EmailId;
                firstName = userReg.b.FirstName;
                lastName = userReg.b.LastName;
                var UserName = firstName + " " + lastName;
                string regTemplate = Path.Combine(Server.MapPath("~/EmailTemplates"), "PaymentSuccess.htm");

                //Insert entry to User notification table
                MillionLightsEmails mEmail = new MillionLightsEmails();
                if (recipientsEmail != null)
                {
                    mEmail.SendPaymentSuccessEmail(
                    ConfigurationManager.AppSettings["SenderName"],
                    ConfigurationManager.AppSettings["SenderEmail"],
                     ConfigurationManager.AppSettings["Telephone"],
                    ConfigurationManager.AppSettings["EmailId"],
                    "MillionLights Course order Payment details",
                    new List<string> { recipientsEmail },
                    regTemplate,
                    UserName,
                    Path.Combine(Server.MapPath("~/Content/assets/img/slider/Logo.png")), orderNo, orderDate, totalPrice, cNameForEmail, unitPrice);


                    //email notification
                    UserNotitification userNotification = new UserNotitification();
                    userNotification.Receiver = userId;
                    userNotification.Sender = "System";
                    userNotification.Subject = String.Format(Constants.CourseCouponEnrollSubNotification, orderNo);
                    userNotification.Message = String.Format(Constants.CourseCouponEnrollMsgNotification, orderNo, cName);
                    userNotification.IsAlert = false;
                    userNotification.DateSent = DateTime.Now;
                    userNotification.ReadDate = null;
                    userNotification.SMSDate = null;
                    userNotification.MailDate = DateTime.Now;
                    userNotification.NotificationStatusId = 2;
                    db.UserNotitifications.Add(userNotification);
                    db.SaveChanges();
                }
            }
        }
        public static DateTime ConvertTimeFromUtc(DateTime timeUtc)
        {
            DateTime destTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);
            return destTime;
        }
        private void SaveOrderDetails(string checkOutCourses, string userId, string orderAmount, out decimal orderPrice, out string OrderNumber)
        {
            DateTime convertedDate = DateTime.SpecifyKind(
            DateTime.Parse(DateTime.UtcNow.ToString()),
            DateTimeKind.Local);
            DateTime dt = convertedDate.ToLocalTime();

            orderPrice = decimal.Parse(orderAmount);
            byte[] buffer = Guid.NewGuid().ToByteArray();
            OrderNumber = BitConverter.ToUInt32(buffer, 8).ToString();
            Orders ord = new Orders();
            ord.UserID = int.Parse(userId);
            ord.OrderNumber = OrderNumber;
            string CourseIds = checkOutCourses;
            string[] CourseIdsArray = CourseIds.Split(',');
            ord.TotalItems = CourseIdsArray.Length - 1;

            ord.ShippingAddress1 = null;
            ord.ShippingAddress2 = null;
            ord.ShippingCity = null;
            ord.ShippingCountry = null;
            ord.ShippingState = null;
            ord.ShippingZipCode = null;

            ord.BillingAddress1 = null;
            ord.BillingAddress2 = null;
            ord.BillingCity = null;
            ord.BillingState = null;
            ord.BillingCountry = null;
            ord.BillingZipCode = null;


            ord.OrderStatusID = 2;
            ord.OrderStatus = "Pending";
            ord.OrderedDatetime = dt;
            ord.OrderModifiedOn = dt;
            ord.IsActive = true;
            ord.TotalPrice = orderPrice;
            db.Orders.Add(ord);
            db.SaveChanges();

            for (int i = 0; i < CourseIdsArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(CourseIdsArray[i]))
                {
                    int crsId = int.Parse(CourseIdsArray[i]);
                    var courseById = db.Courses.Find(crsId);

                    //Suraj
                    string couponCode = string.Empty;
                    Coupon coup = null;
                    decimal effPrice = 0;
                    if (HttpContext.Request.Cookies["CouponCode"] != null)
                    {
                        HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                        couponCode = cookie.Value;

                        coup = db.Coupons.Where(x => x.CouponCode == couponCode).FirstOrDefault();

                        //For Individual Discounted Price
                        effPrice = (courseById.BasePrice) - (courseById.BasePrice * (coup.DiscountPrice / 100));
                    }

                    //Suraj
                    ItemsOrdered itemOrder = new ItemsOrdered();
                    itemOrder.OrderID = ord.OrderID;
                    itemOrder.CourseId = crsId;
                    itemOrder.UnitPrice = effPrice;
                    itemOrder.Quantity = 1;
                    itemOrder.IsActive = true;

                    var coupon = db.Coupons.Where(x => x.CouponCode == couponCode).FirstOrDefault();
                    if (coupon != null)
                    {
                        itemOrder.CouponId = coupon.Id;
                    }

                    db.ItemsOrders.Add(itemOrder);
                    db.SaveChanges();
                }
            }
        }
        public ActionResult Success(FormCollection form)
        {
            bool ccAvenueOrderInvalidStatus = false;
            // Back to Login if session is expired
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            #region CCAvenue Details
            NameValueCollection Params = new NameValueCollection();
            if (Request.Form["encResp"] != null)
            {
                //Read the CCAvenue response
                //string workingKey = "AB50AEC0D366360D0323865FE5656757";//Test - put in the 32bit alpha numeric key in the quotes provided here
                string workingKey = "1B33452A52B22B78913003E55481132F"; //Live
                //string workingKey = "09035A6FEC6A74AF61E9328ABF20FBB8"; //New Test
                CCACrypto ccaCrypto = new CCACrypto();
                string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"], workingKey);

                string[] segments = encResponse.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
                //End Response
            }
            #endregion

            if (Request.Form["encResp"] != null && Params[3] == "Failure")
            {
                return RedirectToAction("Fail", "Order");
            }


            string couponCode = string.Empty;
            if (HttpContext.Request.Cookies["CouponCode"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                cookie.Expires = DateTime.Now.AddDays(-1d);
                couponCode = cookie.Value;
                //cookie.Value = null;
            }
            decimal orderPrice;
            string orderNumber = null;


            Transaction txn = new Transaction();
            if (Request.Form["encResp"] == null)
            {
                if (form["TotalAmount"] != null && decimal.Parse(form["TotalAmount"]) == 0)
                {
                    string checkOutCourses = form["CheckCourse"].ToString();
                    string userId = form["UserId"].ToString();
                    string orderAmount = form["TotalAmount"].ToString();

                    SaveOrderDetails(checkOutCourses, userId, orderAmount, out orderPrice, out orderNumber);
                    txn.orderNumber = orderNumber;
                    txn.transactionId = 0;
                    txn.status = "Success";
                    //success.currency = "Rupees";
                    txn.price = decimal.Parse(orderAmount);
                    TempData["totalAmount"] = form["TotalAmount"];
                    TempData["CheckCourse"] = form["CheckCourse"];
                    TempData["orderNumber"] = orderNumber;
                    TempData["WalletUpdatedAmount"] = form["WalletUpdatedAmount"];
                    TempData["UpdateMyWallet"] = form["UpdateMyWallet"];

                }

            }
            else
            {
                ccAvenueOrderInvalidStatus = true;
                txn.orderNumber = Params[0];
                txn.transactionId = Convert.ToInt64(Params[1]);
                txn.status = Params[3];
                txn.currency = Params[9];
                txn.price = Convert.ToDecimal(Params[10]);
                orderNumber = Params[0];
            }
            //return RedirectToAction("Fail", "Order");
            //Change order status to success
            Orders order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
            order.OrderStatusID = 1;
            order.OrderStatus = "Completed";
            if (Request.Form["encResp"] != null)
            {
                order.ShippingAddress1 = Params[20];
                order.ShippingAddress2 = null;
                order.ShippingCity = Params[21];
                order.ShippingCountry = Params[24];
                order.ShippingState = Params[22];
                order.ShippingZipCode = Params[23];

                order.BillingAddress1 = Params[12];
                order.BillingAddress2 = null;
                order.BillingCity = Params[13];
                order.BillingState = Params[14];
                order.BillingCountry = Params[16];
                order.BillingZipCode = Params[15];
            }
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            //string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
            //string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
            //Prashant Sir
            //EdxCourseAPI courseAPI = new EdxCourseAPI(baseuri, secretKey);
            if (ccAvenueOrderInvalidStatus == true)
            {
                if (Params[3] == "Invalid")
                {
                    Transaction tr = new Models.Transaction();
                    tr.orderNumber = txn.orderNumber;
                    tr.transactionId = txn.transactionId;
                    if (TempData["CheckoutCourseIds"] != null)
                    {
                        tr.courseName = TempData["CheckoutCourseIds"].ToString();
                    }
                    tr.price = txn.price;
                    tr.status = txn.status;
                    TempData["UserOrderData"] = tr;
                    return RedirectToAction("Error", "Order");
                }
            }
            User user = db.Users.Where(u => u.UserId == order.UserID).FirstOrDefault();
            string cName = string.Empty;
            string cNameForEmail = string.Empty;
            List<ItemsOrdered> io = db.ItemsOrders.Where(x => x.OrderID == order.OrderID).ToList();
            foreach (var item in io)
            {
                var courseId = item.CourseId;
                UsersCourses userCourse = new UsersCourses();
                userCourse.UserId = order.UserID;
                userCourse.CourseID = item.CourseId;
                userCourse.CreatedOn = DateTime.Now;
                userCourse.Status = true;
                userCourse.CouponApplied = couponCode;
                userCourse.VoucherAssignedOn = DateTime.Now;
                userCourse.CanAccessLMS = true;
                userCourse.CanPay = true;
                userCourse.CanAccessCertification = true;
                userCourse.IsActive = true;
                db.UsersCourses.Add(userCourse);
                db.SaveChanges();

                //Prashant Sir
                //Enroll into LMS
                //courseAPI.EnrollUser(user.EmailId, course.LMSCourseId);

                Course course = db.Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                string viewCourse = "<br />(<a href=" + course.EDXCourseLink + " target='_blank'> View Course </a>)<br />";
                if (!string.IsNullOrEmpty(cName))
                {
                    cName = course.CourseName + viewCourse + ' ' + ',' + ' ' + cName;
                }
                else
                {
                    cName = course.CourseName + viewCourse;
                }
                if (!string.IsNullOrEmpty(cNameForEmail))
                {
                    cNameForEmail = course.CourseName + ' ' + ',' + ' ' + cNameForEmail;
                }
                else
                {
                    cNameForEmail = course.CourseName;
                }
                //API - Sergiy
                //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");
                string lmsCourseId = GetLMSCourseId(course.EDXCourseLink);
                if (lmsCourseId != null)
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
                    try
                    {
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

                            //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
                            string json = "{\"email\":\"" + user.EmailId + "\"," +

                                          "\"course_id\":\"" + lmsCourseId + "\"}";
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }

            long orderNo = Convert.ToInt64(order.OrderNumber);
            var orderDate = order.OrderedDatetime;
            decimal finalPrice = order.TotalPrice;
            decimal totalPrice = 0;

            if (Request.Form["encResp"] != null && Params[10] != null)
            {
                totalPrice = Convert.ToDecimal(Params[10]);
            }
            else
            {
                totalPrice = Convert.ToDecimal(form["TotalAmount"]);
            }
            CouponSuccessEmail(cName, orderNo, orderDate, finalPrice, finalPrice, cNameForEmail);
            txn.courseName = cName;

            //Check if the Coupon is completely used
            if (couponCode != null && couponCode != "")
            {
                //Coupon
                var coup = db.Coupons.Where(c => c.CouponCode == couponCode).FirstOrDefault();
                //Get the user Courses Enrolled
                var usrCourses = db.UsersCourses.Where(uc => uc.UserId == user.UserId && uc.CouponApplied == couponCode).ToList();

                UserCoupons userCoupon = db.UserCoupons.Where(uc => uc.CouponId == coup.Id && uc.ActivateBy == user.UserId).FirstOrDefault();
                if (userCoupon == null)
                {
                    UserCoupons newUserCoupon = new UserCoupons();
                    newUserCoupon.ActivateBy = user.UserId;
                    newUserCoupon.ActivateOn = DateTime.Now;
                    newUserCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
                    newUserCoupon.CouponId = coup.Id;
                    db.UserCoupons.Add(newUserCoupon);
                    db.SaveChanges();
                }
                else
                {
                    userCoupon.ActivateOn = DateTime.Now;
                    userCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
                    db.Entry(userCoupon).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            // Invalidate the Cache on the Client Side
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Request.Cookies.Remove(cookie);
            }
            foreach (var cookie in Response.Cookies.AllKeys)
            {
                Response.Cookies.Remove(cookie);
            }
            //Send SMS
            int uId = int.Parse(Session["UserID"].ToString());
            var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
            var userName = userRegister.b.FirstName + " " + userRegister.b.LastName;
            Int64? numberToSend = userRegister.b.PhoneNumber;
            string msg = String.Format(Millionlights.Models.Constants.OrderSuccessSMSMsg, orderNumber);
            var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
            string smsRequestUrl = string.Format(url, numberToSend, msg);
            HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
                    string resp = responseStream.ReadLine();
                }
            }
            if (HttpContext.Request.Cookies["CouponCode"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                cookie.Expires = DateTime.Now.AddDays(-10);
                HttpContext.Response.Cookies.Remove("CouponCode");
                cookie.Value = null;
                HttpContext.Response.SetCookie(cookie);
            }


            //Update users wallet
            if (form["WalletUpdatedAmount"] != null)
            {
                if (form["UpdateMyWallet"] != null && form["UpdateMyWallet"] == "true")
                {
                    var updatedAmt = decimal.Parse(form["WalletUpdatedAmount"]);
                    var userWallet = db.UserWallets.Where(x => x.UserId == order.UserID).FirstOrDefault();
                    if (userWallet != null)
                    {
                        userWallet.FinalAmountInWallet = updatedAmt;
                        db.Entry(userWallet).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View(txn);
        }
        //public ActionResult Success(FormCollection form)
        //{
        //    // Back to Login if session is expired
        //    if (Session["UserID"] == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    string couponCode = string.Empty;
        //    if (HttpContext.Request.Cookies["CouponCode"] != null)
        //    {
        //        HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
        //        cookie.Expires = DateTime.Now.AddDays(-1d);
        //        couponCode = cookie.Value;
        //        //cookie.Value = null;
        //    }
        //    decimal orderPrice;
        //    string orderNumber;


        //    Transaction txn = new Transaction();
        //    if (form["TotalAmount"] != null && decimal.Parse(form["TotalAmount"]) == 0)
        //    {
        //        SaveOrderDetails(form, out orderPrice, out orderNumber);
        //        txn.orderNumber = orderNumber;
        //        txn.transactionId = 0;
        //        txn.status = "Success";
        //        //success.currency = "Rupees";
        //        txn.price = decimal.Parse(form["TotalAmount"]);
        //        TempData["totalAmount"] = form["TotalAmount"];
        //        TempData["CheckCourse"] = form["CheckCourse"];
        //        TempData["orderNumber"] = orderNumber;
        //    }
        //    else
        //    {
        //        //Clear Coupon cookie
        //        //if (HttpContext.Request.Cookies["CouponCode"] != null)
        //        //{
        //        //    HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
        //        //    cookie.Expires = DateTime.Now.AddDays(-1d);
        //        //    couponCode = cookie.Value;
        //        //    cookie.Value = null;
        //        //}

        //        //Get order response 
        //        string responce = string.Empty;
        //        foreach (string key in form.AllKeys)
        //        {
        //            if (key.Equals("responseparams"))
        //            {
        //                responce = form.GetValues(key).FirstOrDefault();
        //            }
        //        }

        //        //Read order details from response
        //        string[] responceArray = responce.Split('|');
        //        txn.orderNumber = responceArray[5];
        //        txn.transactionId = Convert.ToInt64(responceArray[0]);
        //        txn.status = responceArray[1];
        //        txn.currency = responceArray[3];
        //        txn.price = Convert.ToDecimal(responceArray[6]);
        //        orderNumber = responceArray[5];
        //    }

        //    //Change order status to success
        //    Orders order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
        //    order.OrderStatusID = 1;
        //    db.Entry(order).State = EntityState.Modified;
        //    db.SaveChanges();

        //    //string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
        //    //string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
        //    //Prashant Sir
        //    //EdxCourseAPI courseAPI = new EdxCourseAPI(baseuri, secretKey);

        //    User user = db.Users.Where(u => u.UserId == order.UserID).FirstOrDefault();
        //    string cName = string.Empty;
        //    List<ItemsOrdered> io = db.ItemsOrders.Where(x => x.OrderID == order.OrderID).ToList();
        //    decimal basePrice = 0;
        //    foreach (var item in io)
        //    {
        //        var courseId = item.CourseId;
        //        UsersCourses userCourse = new UsersCourses();
        //        userCourse.UserId = order.UserID;
        //        userCourse.CourseID = item.CourseId;
        //        userCourse.CreatedOn = DateTime.Now;
        //        userCourse.Status = true;
        //        userCourse.CouponApplied = couponCode;
        //        userCourse.VoucherAssignedOn = DateTime.Now;
        //        userCourse.CanAccessLMS = true;
        //        userCourse.CanPay = true;
        //        userCourse.CanAccessCertification = true;
        //        userCourse.IsActive = true;
        //        db.UsersCourses.Add(userCourse);
        //        db.SaveChanges();

        //        //Prashant Sir
        //        //Enroll into LMS
        //        //courseAPI.EnrollUser(user.EmailId, course.LMSCourseId);

        //        Course course = db.Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();

        //        if (!string.IsNullOrEmpty(cName))
        //        {
        //            cName = course.CourseName + ' ' + ',' + ' ' + cName;
        //        }
        //        else
        //        {
        //            cName = course.CourseName;
        //        }
        //    }

        //    long orderNo = Convert.ToInt64(order.OrderNumber);
        //    var orderDate = order.OrderedDatetime;
        //    decimal finalPrice = order.TotalPrice;
        //    decimal totalPrice = 0;

        //    if (form["TotalAmount"] != null)
        //    {
        //        totalPrice = Convert.ToDecimal(form["TotalAmount"]);
        //    }
        //    CouponSuccessEmail(cName, orderNo, orderDate, finalPrice, finalPrice);
        //    txn.courseName = cName;

        //    //Check if the Coupon is completely used
        //    if (couponCode != null && couponCode != "")
        //    {
        //        //Coupon
        //        var coup = db.Coupons.Where(c => c.CouponCode == couponCode).FirstOrDefault();
        //        //Get the user Courses Enrolled
        //        var usrCourses = db.UsersCourses.Where(uc => uc.UserId == user.UserId && uc.CouponApplied == couponCode).ToList();

        //        UserCoupons userCoupon = db.UserCoupons.Where(uc => uc.CouponId == coup.Id && uc.ActivateBy == user.UserId).FirstOrDefault();
        //        if (userCoupon == null)
        //        {
        //            UserCoupons newUserCoupon = new UserCoupons();
        //            newUserCoupon.ActivateBy = user.UserId;
        //            newUserCoupon.ActivateOn = DateTime.Now;
        //            newUserCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
        //            newUserCoupon.CouponId = coup.Id;
        //            db.UserCoupons.Add(newUserCoupon);
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            userCoupon.ActivateOn = DateTime.Now;
        //            userCoupon.RedeemStatus = (usrCourses.Count == coup.AllowedCourses) ? "Complete" : "Partial";
        //            db.Entry(userCoupon).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }

        //    // Invalidate the Cache on the Client Side
        //    foreach (var cookie in Request.Cookies.AllKeys)
        //    {
        //        Request.Cookies.Remove(cookie);
        //    }
        //    foreach (var cookie in Response.Cookies.AllKeys)
        //    {
        //        Response.Cookies.Remove(cookie);
        //    }
        //    //Send SMS
        //    int uId = int.Parse(Session["UserID"].ToString());
        //    var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
        //    var userName = userRegister.b.FirstName + " " + userRegister.b.LastName;
        //    Int64? numberToSend = userRegister.b.PhoneNumber;
        //    string msg = String.Format(Millionlights.Models.Constants.OrderSuccessSMSMsg, orderNumber);
        //    var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
        //    string smsRequestUrl = string.Format(url, numberToSend, msg);
        //    HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        if (response.StatusCode.Equals(HttpStatusCode.OK))
        //        {
        //            StreamReader responseStream = new StreamReader(response.GetResponseStream());
        //            string resp = responseStream.ReadLine();
        //        }
        //    }
        //    if (HttpContext.Request.Cookies["CouponCode"] != null)
        //    {
        //        HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
        //        cookie.Expires = DateTime.Now.AddDays(-10);
        //        HttpContext.Response.Cookies.Remove("CouponCode");
        //        cookie.Value = null;
        //        HttpContext.Response.SetCookie(cookie);
        //    }
        //    return View(txn);
        //}

        public ActionResult FreeCourseSuccess()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Request.Cookies["CouponCode"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                cookie.Expires = DateTime.Now.AddDays(-10);
                HttpContext.Response.Cookies.Remove("CouponCode");
                cookie.Value = null;
                HttpContext.Response.SetCookie(cookie);
            }
            List<dynamic> freeCourses = new List<dynamic>();
            dynamic fc = new ExpandoObject();
            string cName = string.Empty;
            string tempTotal = TempData["totalAmount"].ToString();
            string tempcheckCourse = TempData["CheckCourse"].ToString().Trim(new char[] { ',' });

            int userId = Convert.ToInt32(Session["UserId"]);
            User user = db.Users.Where(u => u.UserId == userId).FirstOrDefault();

            //Prashant Sir
            //string baseuri = System.Configuration.ConfigurationManager.AppSettings["baseURLEdx"];
            //string secretKey = System.Configuration.ConfigurationManager.AppSettings["edxSecretKey"];
            //EdxCourseAPI courseAPI = new EdxCourseAPI(baseuri, secretKey);
            string[] courses = tempcheckCourse.Split(',');
            //string lmsCourseIds = string.Empty;
            foreach (var course in courses)
            {
                if (course != "")
                {
                    Int32 cID = Convert.ToInt32(course);
                    Course crs = db.Courses.Where(x => x.Id == cID).FirstOrDefault();
                    string viewCourse = "<br />(<a href=" + crs.EDXCourseLink + " target='_blank'> View Course </a>)<br />";
                    if (!string.IsNullOrEmpty(cName))
                    {
                        cName = crs.CourseName + viewCourse + ' ' + ',' + ' ' + cName;
                    }
                    else
                    {
                        cName = crs.CourseName + viewCourse;
                    }

                    //Prashant Sir
                    //courseAPI.EnrollUser(user.EmailId, crs.LMSCourseId);
                    //API - Sergiy
                    //string lmsCourseId = GetLMSCourseId("https://lms.millionlights.org/courses/Millionlights/NPTEL008/2015");
                    //string lmsCourseId = GetLMSCourseId(crs.EDXCourseLink);
                    //if (lmsCourseId != null)
                    //{
                    //    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.millionlights.org/api/sso_edx_ml/enrollment_course/");
                    //    httpWebRequest.ContentType = "application/json";
                    //    httpWebRequest.Method = "POST";
                    //    httpWebRequest.Headers.Add("X-Edx-Api-Key:f5dae379f770f6364caeb1da1a9c73f3");
                    //    try
                    //    {
                    //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    //        {
                    //            //string json = "{\"email\":\"Suraj@ncorpuscle.com\"," +

                    //            //              "\"course_id\":\"Millionlights/NPTEL008/2015\"}";
                    //            string json = "{\"email\":\"" + user.EmailId + "\"," +

                    //                          "\"course_id\":\"" + "test" + "\"}";
                    //            streamWriter.Write(json);
                    //            streamWriter.Flush();
                    //            streamWriter.Close();
                    //        }
                    //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //        {
                    //            var result = streamReader.ReadToEnd();
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        continue;
                    //    }
                    //}
                }
            }
            string orderNumber = TempData["orderNumber"].ToString();
            fc.OrderPrice = tempTotal;
            fc.CheckoutCourses = cName;
            fc.OrderNumber = orderNumber;
            fc.TransactionId = 0;
            fc.Status = "Success";
            freeCourses.Add(fc);
            ViewBag.FreeCourse = freeCourses;

            //Update users wallet
            if (TempData["WalletUpdatedAmount"] != null)
            {
                if (TempData["UpdateMyWallet"] != null && TempData["UpdateMyWallet"].ToString() == "true")
                {
                    var updatedAmt = decimal.Parse(TempData["WalletUpdatedAmount"].ToString());
                    var userWallet = db.UserWallets.Where(x => x.UserId == userId).FirstOrDefault();
                    if (userWallet != null)
                    {
                        userWallet.FinalAmountInWallet = updatedAmt;
                        db.Entry(userWallet).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View();

        }

        public ActionResult Fail(FormCollection form)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            #region CCAvenue Details
            NameValueCollection Params = new NameValueCollection();
            if (Request.Form["encResp"] != null)
            {
                //Read the CCAvenue response
                //string workingKey = "AB50AEC0D366360D0323865FE5656757";//Test - put in the 32bit alpha numeric key in the quotes provided here
                string workingKey = "1B33452A52B22B78913003E55481132F"; //Live
                //string workingKey = "09035A6FEC6A74AF61E9328ABF20FBB8"; //New Test
                CCACrypto ccaCrypto = new CCACrypto();
                string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"], workingKey);

                string[] segments = encResponse.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
                //End Response
            }
            #endregion
            decimal orderPrice;
            string orderNumber = null;
            string couponCode = string.Empty;
            Transaction txn = new Transaction();
            if (Request.Form["encResp"] == null)
            {
                if (form["TotalAmount"] != null && decimal.Parse(form["TotalAmount"]) == 0)
                {
                    string checkOutCourses = form["CheckCourse"].ToString();
                    string userId = form["UserId"].ToString();
                    string orderAmount = form["TotalAmount"].ToString();
                    SaveOrderDetails(checkOutCourses, userId, orderAmount, out orderPrice, out orderNumber);
                    txn.orderNumber = orderNumber;
                    txn.transactionId = 0;
                    txn.status = "Failed";
                    //success.currency = "Rupees";
                    txn.price = decimal.Parse(form["TotalAmount"]);
                    TempData["totalAmount"] = form["TotalAmount"];
                    TempData["CheckCourse"] = form["CheckCourse"];
                    TempData["orderNumber"] = orderNumber;

                }
            }
            else
            {
                if (HttpContext.Request.Cookies["CouponCode"] != null)
                {
                    HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
                    cookie.Expires = DateTime.Now.AddDays(-1d);
                    couponCode = cookie.Value;
                    cookie.Value = null;
                }

                //string responce = string.Empty;
                //foreach (string key in form.AllKeys)
                //{
                //    if (key.Equals("responseparams"))
                //    {
                //        responce = form.GetValues(key).FirstOrDefault();
                //    }
                //}

                //string[] responceArray = responce.Split('|');
                txn.orderNumber = Params[0];
                txn.transactionId = Convert.ToInt64(Params[1]);
                txn.status = Params[3];
                txn.currency = Params[9];
                txn.price = Convert.ToDecimal(Params[10]);
                orderNumber = Params[0];
            }

            Orders order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
            order.OrderStatusID = 4;
            order.OrderStatus = "Failed";
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            string cName = string.Empty;
            List<ItemsOrdered> orderedItems = db.ItemsOrders.Where(x => x.OrderID == order.OrderID).ToList();
            foreach (var item in orderedItems)
            {
                Course course = db.Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                string viewCourse = "<br />(<a href=" + course.EDXCourseLink + " target='_blank'> View Course </a>)<br />";
                //cName = course.CourseName + ',' + cName;
                if (!string.IsNullOrEmpty(cName))
                {
                    cName = course.CourseName + viewCourse + ' ' + ',' + ' ' + cName;
                }
                else
                {
                    cName = course.CourseName + viewCourse;
                }
            }

            // Invalidate the Cache on the Client Side
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Request.Cookies.Remove(cookie);
            }
            foreach (var cookie in Response.Cookies.AllKeys)
            {
                Response.Cookies.Remove(cookie);
            }
            txn.courseName = cName;

            //Send SMS
            int uId = int.Parse(Session["UserID"].ToString());
            var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
            var userName = userRegister.b.FirstName + " " + userRegister.b.LastName;
            Int64? numberToSend = userRegister.b.PhoneNumber;
            string msg = String.Format(Millionlights.Models.Constants.OrderFailSMSMsg, orderNumber);
            var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
            string smsRequestUrl = string.Format(url, numberToSend, msg);
            HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
                    string resp = responseStream.ReadLine();
                }
            }
            return View(txn);
        }

        //public ActionResult Fail(FormCollection form)
        //{
        //    if (Session["UserID"] == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    decimal orderPrice;
        //    string orderNumber;
        //    string couponCode = string.Empty;
        //    Transaction txn = new Transaction();
        //    if (form["TotalAmount"] != null && decimal.Parse(form["TotalAmount"]) == 0)
        //    {
        //        //SaveOrderDetails(form, out orderPrice, out orderNumber);
        //        txn.orderNumber = orderNumber;
        //        txn.transactionId = 0;
        //        txn.status = "Failed";
        //        //success.currency = "Rupees";
        //        txn.price = decimal.Parse(form["TotalAmount"]);
        //        TempData["totalAmount"] = form["TotalAmount"];
        //        TempData["CheckCourse"] = form["CheckCourse"];
        //        TempData["orderNumber"] = orderNumber;

        //    }
        //    else
        //    {
        //        if (HttpContext.Request.Cookies["CouponCode"] != null)
        //        {
        //            HttpCookie cookie = HttpContext.Request.Cookies.Get("CouponCode");
        //            cookie.Expires = DateTime.Now.AddDays(-1d);
        //            couponCode = cookie.Value;
        //            cookie.Value = null;
        //        }

        //        string responce = string.Empty;
        //        foreach (string key in form.AllKeys)
        //        {
        //            if (key.Equals("responseparams"))
        //            {
        //                responce = form.GetValues(key).FirstOrDefault();
        //            }
        //        }

        //        string[] responceArray = responce.Split('|');
        //        txn.orderNumber = responceArray[5];
        //        txn.transactionId = Convert.ToInt64(responceArray[0]);
        //        txn.status = responceArray[1];
        //        txn.currency = responceArray[3];
        //        txn.price = Convert.ToDecimal(responceArray[6]);
        //        orderNumber = responceArray[5];
        //    }
        //    Orders order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
        //    order.OrderStatusID = 4;
        //    db.Entry(order).State = EntityState.Modified;
        //    db.SaveChanges();

        //    string cName = string.Empty;
        //    List<ItemsOrdered> orderedItems = db.ItemsOrders.Where(x => x.OrderID == order.OrderID).ToList();
        //    foreach (var item in orderedItems)
        //    {
        //        Course course = db.Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
        //        cName = course.CourseName + ',' + cName;
        //    }

        //    // Invalidate the Cache on the Client Side
        //    foreach (var cookie in Request.Cookies.AllKeys)
        //    {
        //        Request.Cookies.Remove(cookie);
        //    }
        //    foreach (var cookie in Response.Cookies.AllKeys)
        //    {
        //        Response.Cookies.Remove(cookie);
        //    }
        //    txn.courseName = cName;

        //    //Send SMS
        //    int uId = int.Parse(Session["UserID"].ToString());
        //    var userRegister = db.Users.Join(db.UsersDetails, a => a.UserId, b => b.UserId, (a, b) => new { a, b }).Where(X => X.a.UserId == uId).FirstOrDefault();
        //    var userName = userRegister.b.FirstName + " " + userRegister.b.LastName;
        //    Int64? numberToSend = userRegister.b.PhoneNumber;
        //    string msg = String.Format(Millionlights.Models.Constants.OrderFailSMSMsg, orderNumber);
        //    var url = ConfigurationManager.AppSettings["SendSmsUrlFormat"];
        //    string smsRequestUrl = string.Format(url, numberToSend, msg);
        //    HttpWebRequest request = WebRequest.Create(smsRequestUrl) as HttpWebRequest;
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        if (response.StatusCode.Equals(HttpStatusCode.OK))
        //        {
        //            StreamReader responseStream = new StreamReader(response.GetResponseStream());
        //            string resp = responseStream.ReadLine();
        //        }
        //    }
        //    return View(txn);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult CCAvenue()
        {
            CCACrypto ccaCrypto = new CCACrypto();
            //string workingKey = "AB50AEC0D366360D0323865FE5656757";//Test - put in the 32bit alpha numeric key in the quotes provided here 	
            string workingKey = "1B33452A52B22B78913003E55481132F";//Live - put in the 32bit alpha numeric key in the quotes provided here 
            //string workingKey = "09035A6FEC6A74AF61E9328ABF20FBB8";//New Test - put in the 32bit alpha numeric key in the quotes provided here 
            string ccaRequest = "";
            string strEncRequest = "";
            //string strAccessCode = "AVEB07CK70CF55BEFC";// Test - put the access key in the quotes provided here.
            string strAccessCode = "AVLN07CK71CG08NLGC";// Live - put the access key in the quotes provided here.
            //string strAccessCode = "AVCY00DH50BY69YCYB";// New Test - put the access key in the quotes provided here.

            foreach (string name in Request.Form)
            {
                if (name != null)
                {
                    if (!name.StartsWith("_"))
                    {
                        ccaRequest = ccaRequest + name + "=" + Request.Form[name] + "&";
                    }
                }
            }
            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            ViewBag.CCAvenueAccessCode = strAccessCode;
            ViewBag.EncryptedRequest = strEncRequest;

            return View();
        }

        //Api -Sergiy
        public string GetLMSCourseId(string EDXCourseLink)
        {
            var message = string.Format("The course with this LMSID is not found. Please check the LMS Course ID.");
            string[] edxLinksArr = EDXCourseLink.Split(new string[] { "lms.millionlights.org/courses/" }, StringSplitOptions.None);
            if (edxLinksArr.Length > 1)
            {
                //string[] splitLMSCourseId = edxLinksArr[1].Split('/');
                ////EDXCourseLink = edxLinksArr[1];
                //if (splitLMSCourseId.Length > 1)
                //{
                //    EDXCourseLink = splitLMSCourseId[0] + "/" + splitLMSCourseId[1] + "/" + splitLMSCourseId[2];
                //    EDXCourseLink = EDXCourseLink.EndsWith('/');
                //}
                if (edxLinksArr[1].EndsWith("/"))
                {
                    EDXCourseLink = edxLinksArr[1].TrimEnd(new[] { '/' });
                }
                else
                {
                    EDXCourseLink = edxLinksArr[1];
                }
            }
            else
            {
                EDXCourseLink = null;
            }
            return EDXCourseLink;
        }
        public ActionResult Error()
        {
            Transaction failDetails = new Transaction();
            if (TempData["UserOrderData"] != null)
            {
                Transaction list = (Transaction)TempData["UserOrderData"];
                string cName = string.Empty;
                var courseId = list.courseName.Split(',');
                for (var i = 0; i < courseId.Length; i++)
                {
                    if (courseId[i] != "")
                    {
                        var id = int.Parse(courseId[i]);
                        Course course = db.Courses.Where(x => x.Id == id).FirstOrDefault();
                        string viewCourse = "<br />(<a href=" + course.EDXCourseLink + " target='_blank'> View Course </a>)<br />";
                        if (!string.IsNullOrEmpty(cName))
                        {
                            cName = course.CourseName + viewCourse + ' ' + ',' + ' ' + cName;
                        }
                        else
                        {
                            cName = course.CourseName + viewCourse;
                        }

                    }
                }

                failDetails.courseName = cName;
                failDetails.country = list.country;
                failDetails.currency = list.currency;
                failDetails.orderNumber = list.orderNumber;
                failDetails.price = list.price;
                failDetails.status = list.status;
                failDetails.transactionId = list.transactionId;
            }
            return View(failDetails);
        }

       [OverrideActionFilters]

        public ActionResult ExportCouponData()
        {

            List<dynamic> userCouponDetailList = new List<dynamic>();
            try
            {
                var couponListWithUserId = db.Coupons.Where(x => !string.IsNullOrEmpty(x.UserId.ToString())).ToList();

                // query to get coupons without User Id
              
                var userCouponList = db.UserCoupons.Join(db.Coupons, userCoupon => userCoupon.CouponId, coupon => coupon.Id, (userCoupon, coupon) => new { userCoupon, coupon })
                                                   .Where(x=>string.IsNullOrEmpty(x.coupon.UserId.ToString()))
                                                   .ToList();

                var couponListWithoutUserId = userCouponList.Join(db.Users, uc => uc.userCoupon.ActivateBy, u => u.UserId, (uc, u) => new { uc, u })
                                    .Join(db.UsersDetails, a => a.u.UserId, b => b.UserId, (a, b) => new { a, b }).ToList();

                List<userCouponDataViewModel> modelList = new List<userCouponDataViewModel>();

                // Add the couponListWithUserId List in to List
                foreach (var item in couponListWithUserId)
                {
                    userCouponDataViewModel model = new userCouponDataViewModel();
                    var userCouponId = db.UserCoupons.Where(x=>x.CouponId == item.Id).FirstOrDefault();
                    var user = db.Users.Where(x => x.UserId == item.UserId).FirstOrDefault();
                    var userDetails = db.UsersDetails.Where(x => x.UserId == item.UserId).FirstOrDefault();

                    if (userCouponId != null)
                    {
                        model.DateOfCouponActivation = userCouponId.ActivateOn.Value.ToShortDateString();
                    }
                    else
                    {
                        model.DateOfCouponActivation = null;
                    }
                    model.CouponCode = item.CouponCode;
                    model.EmailId = user.EmailId;
                    model.UserName = string.Format("{0} {1}", userDetails.FirstName, userDetails.LastName);
                    model.PartnerName = item.PartnerName;
                    modelList.Add(model);
                }

                // Add the couponListWithoutUserId List in to List
                foreach (var item in couponListWithoutUserId)
                {
                    userCouponDataViewModel model = new userCouponDataViewModel();
                    model.DateOfCouponActivation = item.a.uc.userCoupon.ActivateOn.Value.ToShortDateString(); ;
                    model.CouponCode = item.a.uc.coupon.CouponCode;
                    model.EmailId = item.a.u.EmailId;
                    model.UserName = string.Format("{0} {1}", item.b.FirstName, item.b.LastName);
                    model.PartnerName = item.a.uc.coupon.PartnerName;
                    modelList.Add(model);
                }

                GridView gv = new GridView();
                gv.DataSource = modelList;
                gv.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=CouponData.xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

                gv.RenderControl(objHtmlTextWriter);

                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();

                return RedirectToAction("Index");               
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    public class userCouponDataViewModel
    {
        //public int couponId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string PartnerName { get; set; }
        public string CouponCode { get; set; }
        public string DateOfCouponActivation { get; set; }

    }
}
