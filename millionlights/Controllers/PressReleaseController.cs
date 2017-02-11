using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;
using System.Net;
using Millionlights.EmailService;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
namespace Millionlights.Controllers
{
    public class PressReleaseController : Controller
    {
        private MillionlightsContext db = new MillionlightsContext();
        // GET: PressRelease

        public ActionResult OurTeam()
        {
            return View();
        }
        public ActionResult Press_Content()
        {
            return View();
        }
        
        public ActionResult Press()
        {
            //if (Session["RoleID"] != null)
            //{
            //    int roleId = int.Parse(Session["RoleID"].ToString());
            //    if (roleId == 1 || roleId == 3)
            //    {
            //        return RedirectToAction("Login", "Account");
            //    }
            //}
            var PressContentsdata = db.PressContents.Where(X => X.IsActive == true).ToList();
            if (PressContentsdata.Count > 0)
            {
                ViewBag.PressPageHeading = PressContentsdata.FirstOrDefault().PressPageHeading;
            }
            else
            {
                ViewBag.PressPageHeading = null;
            }
            ViewBag.message = TempData["Msg"];
            ViewBag.PressData = PressContentsdata;
           
            return View();
        }
        public ActionResult PressContents()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var PressContentsPageData = db.PressContents.Where(X => X.IsActive == true).ToList();
            if (PressContentsPageData.Count > 0)
            {
                ViewBag.PressPageHeading = PressContentsPageData.FirstOrDefault().PressPageHeading;
            }
            else
            {
                ViewBag.PressPageHeading = null;
            }
            ViewBag.message = TempData["Msg"];
            ViewBag.PressData = PressContentsPageData;

            string editedCourseIds = "";
            for (var i = 0; i <= PressContentsPageData.Count - 1; i++)
            {
                string cId = PressContentsPageData[i].Id.ToString();
                if (cId != "")
                {
                    editedCourseIds = editedCourseIds + "," + cId;
                }
            }

            Session["EditedCourseIds"] = editedCourseIds.TrimStart(',');

            return View();
        }
        public ActionResult UploadPressPic(FormCollection form)
        {
            PressContents presscontents = new PressContents();
            //PressContentsdata[0].Id
            var PressContentsdata = db.PressContents.Where(X => X.IsActive == true).ToList();
            if (PressContentsdata.Count > 0)
            {
                for (int i = 0; i < PressContentsdata.Count; i++)
                {
                    int count = i + 1;
                    int id = PressContentsdata[i].Id;
                    if (form["PressImage_" + count] == "")
                    {
                        var image = db.PressContents.Where(x => x.Id == id && x.IsActive == true).FirstOrDefault().PressImage;
                        //(from p in db.PressContents where p.Id == id select new { p.PressImage });

                        form["PressImage_" + count] = image;
                    }

                }
            }

            List<PressContents> results = (from p in db.PressContents
                                           select p).ToList();
            foreach (PressContents p in results)
            {
                p.IsActive = false;
            }
                if (Convert.ToInt32(form["StageCountHidden"]) != 0)
                {
                    var stageCount = Convert.ToInt32(form["StageCountHidden"]);

                    for (int i = 0; i < stageCount; i++)
                    {
                        int filecount = i + 1;

                        if (i == 0)
                        {
                            var path = form["PressImage_" + filecount];
                            //string filename = Path.GetFileName(path);
                            //var rootpath = Path.Combine("../Images/PressContentsImg/" + Path.GetFileName(path));
                            presscontents.PressPageHeading = form["PressPageHeading"];
                            presscontents.PressShortDescription = form["PressShortDescriptionC_" + filecount];
                            presscontents.PressLongDescription = form["PressLongDescriptionC_" + filecount];
                            presscontents.PressImage = path;
                            //presscontents.PressImage = Path.GetFileName(path);

                        }
                        else
                        {
                            var path = form["PressImage_" + filecount];
                            var chapNumber = "PressShortDescriptionS" + filecount + "_input";
                            var chapName = "PressLongDescriptionS" + filecount + "_input";
                            //var chapDesc = "PressImage_" + filecount;
                            //var rootpath = Path.Combine("../Images/PressContentsImg/" + Path.GetFileName(path));
                            presscontents.PressShortDescription = form[chapNumber];
                            presscontents.PressLongDescription = form[chapName];
                            presscontents.PressImage = path;
                            //presscontents.PressImage = Path.GetFileName(path);
                        }
                        presscontents.IsActive = true;
                        db.PressContents.Add(presscontents);
                        db.SaveChanges();

                    }
                }
            
            var result = db.PressContents.Where(x => x.IsActive == true).ToList();
            //int folderid;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                //folderid = result[i].Id;

                if (file != null && file.ContentLength != 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Server.MapPath("~/Images/PressContentsImg/");

                    if (Directory.Exists(path))
                    {
                        file.SaveAs(path + "\\" + fileName);
                    }
                    else
                    {
                        DirectoryInfo createDirectory = Directory.CreateDirectory(path);
                        DirectorySecurity dirSecurity = createDirectory.GetAccessControl();
                        dirSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        createDirectory.SetAccessControl(dirSecurity);
                        file.SaveAs(path + "\\" + fileName);
                    }

                }

            }

            return RedirectToAction("PressContents", "PressRelease");

        }
        public ActionResult PressContentDetails(int? Id)
        {
           // int? pId = null;
            if (Id != null)
            {

                var pressData = db.PressContents.Where(x => x.Id == Id).FirstOrDefault();
                ViewBag.pressdata = pressData;
            }
           
            return View();
        }
    }
}
      /*  [HttpPost]
        [ValidateInput(false)]
        public JsonResult SavePressContents(FormCollection form)
        {
            PressContents presscontents = new PressContents();
           
            //var PressContentsPageData = db.PressContents.Where(X => X.IsActive == true).ToList();
            
            List<PressContents> results = (from p in db.PressContents
                                           select p).ToList();
            foreach (PressContents p in results)
            {
                p.IsActive = false;
            }
            db.SaveChanges();
            // var Ids = Idsr.Length - 1;
            int newID = 0;
            //int courseId = press.Id;
            var stageCount = Convert.ToInt32(form["StageCountHidden"]);
            //string[] Ids =stageCount;

            for (int i = 0; i < stageCount; i++)
            {
                //string courseContentId = Ids[i];
                int count = i + 1;

                if (i == 0)
                {

                    var path = form["PressImage_1"];

                    //string extension = Path.GetExtension(path);
                    string filename = Path.GetFileName(path);
                    var rootpath = Path.Combine("../Images/PressContentsImg/" + Path.GetFileName(path));
                    //var rootpath = Path.GetFileName(path);
                    presscontents.PressPageHeading = form["PressPageHeading"];
                    presscontents.PressShortDescription = form["PressShortDescriptionC_" + count];
                    presscontents.PressLongDescription = form["PressLongDescriptionC_" + count];
                    presscontents.PressImage = rootpath;

                }
                else
                {
                    var path = form["PressImage_" + count];
                    //int ImageCount = i;
                    var chapNumber = "PressShortDescriptionS" + count + "_input";
                    var chapName = "PressLongDescriptionS" + count + "_input";
                    //var chapDesc = "StageDesc" + count + "_input";
                    var chapDesc = "PressImage_" + count;
                    //var path = chapDesc;
                    // var filename = Server.MapPath("../../" + chapDesc);
                    var rootpath = Path.Combine("../Images/PressContentsImg/" + Path.GetFileName(path));
                   // var rootpath = Path.GetFileName(path);
                    presscontents.PressShortDescription = form[chapNumber];
                    presscontents.PressLongDescription = form[chapName];
                    presscontents.PressImage = rootpath;
                }
                presscontents.IsActive = true;
                db.PressContents.Add(presscontents);
                db.SaveChanges();

            }
            var result = db.PressContents.Where(x => x.IsActive == true).ToList();
            return Json(result);

        }
    }
}*/
