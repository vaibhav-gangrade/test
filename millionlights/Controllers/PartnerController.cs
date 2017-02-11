using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Millionlights.Models;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Mime;
using System.Drawing;
using System.Web.Helpers;

namespace Millionlights.Controllers
{
    public class PartnerController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        //
        // GET: /Partner/
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
        public ActionResult Index(Partner model)
        {
            var cd = new ContentDisposition
            {
                FileName = "PartnerData.csv",
                Inline = false
            };
            Response.AddHeader("Content-Disposition", cd.ToString());
            return Content(model.Csv, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost]
        public JsonResult GetParentDetails()
        {
            IEnumerable<Partner> PartnerDetails = db.Partners.OrderBy(x => x.Id).ToList();
            List<dynamic> miniPartnerDetails = new List<dynamic>();
            foreach (var item in PartnerDetails)
            {
                dynamic p = new ExpandoObject();
                p.Id = item.Id;
                p.Name = item.Name;
                p.PartnerName = item.PartnerName;
                p.CanAccessLMS = item.CanAccessLMS;
                p.CanPay = item.CanPay;
                p.CanAccessCertification = item.CanAccessCertification;
                p.Email = item.Email;
                p.PhoneNumber = item.PhoneNumber;
                p.ContactPerson = item.ContactPerson;
                p.IsActive = item.IsActive;
                p = JsonConvert.SerializeObject(p);
                miniPartnerDetails.Add(p);
            }
            return Json(miniPartnerDetails);
        }
        
        public ActionResult Create(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            List<SelectListItem> partnerTypeList = new List<SelectListItem>();
            IEnumerable<PartnerType> partnersType = db.PartnerType.Where(X => X.IsActive == true).ToList();

            foreach (var item in partnersType)
            {
                partnerTypeList.Add(new SelectListItem() { Text = item.PartnerTypeName, Value = item.Id.ToString() });
            }
            ViewBag.PartnerList = partnerTypeList;
            //var PartnerDetail = db.Partners.Where(X => X.IsActive == true).ToList();
            //ViewBag.PartnersDetails = PartnerDetail;
            return View();

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Create(Partner partnerDetails, int? id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Partner addpartner = new Partner();

            addpartner.Id = partnerDetails.Id;

            addpartner.Name = partnerDetails.Name;

            addpartner.Email = partnerDetails.Email;

            addpartner.PhoneNumber = partnerDetails.PhoneNumber;

            addpartner.ContactPerson = partnerDetails.ContactPerson;

            addpartner.Address = partnerDetails.Address;

            addpartner.PartnerUrl = partnerDetails.PartnerUrl;

            addpartner.PartnerTypeId = partnerDetails.PartnerTypeId;

            addpartner.Country = partnerDetails.Country;

            addpartner.State = partnerDetails.State;

            addpartner.City = partnerDetails.City;

            addpartner.CreatedOn = DateTime.Today;

            addpartner.ModifiedOn = DateTime.Today;

            addpartner.CreatedBy = Session["UserID"].ToString();

            addpartner.ModifiedBy = Session["UserID"].ToString();

            addpartner.DisplayMessage = null;
            addpartner.DisplayOnHomePage = partnerDetails.DisplayOnHomePage;

            addpartner.IsActive = true;

            db.Partners.Add(addpartner);
            
            db.SaveChanges();

            int PartnerId = addpartner.Id;

            Partner partner = db.Partners.Find(PartnerId);

            var file = Request.Files[0];

            if (Request.Files.Count > 0)
            {
                WebImage img = new WebImage(file.InputStream);

                if (file != null)
                {

                    var fileName = Path.GetFileName(file.FileName);

                    //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    var path = Server.MapPath("~/Images/Partner/" + PartnerId);

                    //string myLogo = Path.GetFileName(file.FileName);

                    //file.SaveAs(path);
                    if (Directory.Exists(path))
                    {

                        string myLogo = Path.GetFileName(file.FileName);
                        if (img.Width > 178)
                            img.Resize(178, 100, true, false);
                        img.Save(path + "\\" + fileName);
                        //file.SaveAs(path + "\\" + fileName);

                    }

                    else
                    {

                        DirectoryInfo createDirectory = Directory.CreateDirectory(path);

                        DirectorySecurity dirSecurity = createDirectory.GetAccessControl();

                        dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                        createDirectory.SetAccessControl(dirSecurity);

                        string myLogo = Path.GetFileName(file.FileName);

                        //file.SaveAs(path + "\\" + fileName);
                        if (img.Width > 178)
                            img.Resize(178, 100, true, false);
                        img.Save(path + "\\" + fileName);
                    }


                    partner.ImageLink = fileName;

                }

                //if (file != null && file.ContentLength > 0)
                //{

                //    var fileName = Path.GetFileName(file.FileName);

                //    var path = Server.MapPath("~/Images/Partner/" + addpartner.Id);

                //    if (Directory.Exists(path))
                //    {

                //        string myLogo = Path.GetFileName(file.FileName);

                //        file.SaveAs(path + "\\" + fileName);

                //    }

                //    else
                //    {

                //        DirectoryInfo createDirectory = Directory.CreateDirectory(path);

                //        DirectorySecurity dirSecurity = createDirectory.GetAccessControl();

                //        dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                //        createDirectory.SetAccessControl(dirSecurity);

                //        string myLogo = Path.GetFileName(file.FileName);

                //        file.SaveAs(path + "\\" + fileName);

                //    }

                //    partner.ImageLink = fileName;

                //}

                db.Entry(partner).State = EntityState.Modified;

                db.SaveChanges();

            }
            return RedirectToAction("Index","Partner");

        }

        public ActionResult Edit(int? id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Partner partnerExist = db.Partners.Find(id);

            if (partnerExist == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> partnerTypeList = new List<SelectListItem>();
            IEnumerable<PartnerType> partnersType = db.PartnerType.Where(X => X.IsActive == true).ToList();

            foreach (var item in partnersType)
            {
                partnerTypeList.Add(new SelectListItem() { Text = item.PartnerTypeName, Value = item.Id.ToString() });
            }
            ViewBag.PartnerList = partnerTypeList;

            //var PartnerDetail = db.Partners.Where(X => X.IsActive == true).ToList();
            //ViewBag.PartnersDetails = PartnerDetail;
            return View(partnerExist);

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Edit(Partner partnerDetails, int? id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            var partnerDetails1 = db.Partners.Find(partnerDetails.Id);

            partnerDetails1.Name = Request["Name"];
            partnerDetails1.PartnerTypeId = Convert.ToInt32(Request["PartnerTypeId"]);

            partnerDetails1.Email = Request["Email"];
            partnerDetails1.PartnerTypeId = Convert.ToInt32(Request["PartnerTypeId"]);
            partnerDetails1.PhoneNumber = Convert.ToInt64(Request["PhoneNumber"]);

            partnerDetails1.ContactPerson = Request["ContactPerson"];

            partnerDetails1.Address = Request["Address"];

            partnerDetails1.ModifiedOn = DateTime.Today;

            partnerDetails1.ModifiedBy = Session["UserID"].ToString();

            var file = Request.Files[0];

            if (file.ContentLength > 0)
            {
                WebImage  img = new WebImage(file.InputStream);
                
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    var path = Server.MapPath("~/Images/Partner/" + partnerDetails.Id);
                    //string myLogo = Path.GetFileName(file.FileName);
                    //file.SaveAs(path);
                    if (Directory.Exists(path))
                    {
                        string myLogo = Path.GetFileName(file.FileName);
                        if (img.Width > 178)
                        img.Resize(178, 100,true,false);
                        img.Save(path + "\\" + fileName);
                        //file.SaveAs(path + "\\" + fileName);

                    }
                    else
                    {
                        DirectoryInfo createDirectory = Directory.CreateDirectory(path);
                        DirectorySecurity dirSecurity = createDirectory.GetAccessControl();
                        dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        createDirectory.SetAccessControl(dirSecurity);
                        string myLogo = Path.GetFileName(file.FileName);
                        //file.SaveAs(path + "\\" + fileName);
                        if (img.Width > 178)
                        img.Resize(178, 100, true, false);
                        img.Save(path + "\\" + fileName);
                    }
                    partnerDetails1.ImageLink = fileName;

                }
            }
            partnerDetails1.DisplayMessage = null;
            partnerDetails1.DisplayOnHomePage = partnerDetails.DisplayOnHomePage;
            db.Entry(partnerDetails1).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        
        public ActionResult Disabled(int? id, bool status)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Partner partnerdetails = db.Partners.Find(id);
            if (status == true)
            {
                partnerdetails.IsActive = false;
            }
            else 
            {
                partnerdetails.IsActive = true;
            }
           

            db.Entry(partnerdetails).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index");

        }
        public ActionResult DeleteMultiplePartner(List<SearchPartnerModel> model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            } 
            Partner partnerModel = null;
            
            foreach (var partner in model)
            {
                partnerModel = db.Partners.Where(x => x.Id == partner.Id).FirstOrDefault();
                partnerModel.IsActive = false;
                db.Entry(partnerModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("index", "Partner");
        }
        public class SearchPartnerModel
        {
            public int Id { get; set; }
        }

    }

}
