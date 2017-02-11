using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Web;
using MandrillEmailService;
using Millionlights.Models;

namespace Millionlights.EmailService
{
    public class MillionLightsEmails
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="registrationInformation"></param>
        public void SendRegistrationCompleteEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string password )
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            if (password != null)
            {
                registrationMail = registrationMail.Replace("[Password]", password);
            }

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail ( fromEmail, fromName, subject, new List<string> {loginId} , registrationMail);
            return;
        }
        public void SendNewImportUserRegistrationCompleteEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string password, string partnerName, string CourseCount, string CoursesNames)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            //Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            ////envolop
            //Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            ////phone
            //Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            ////facebook
            //Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            ////twitter
            //Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            if (password != null)
            {
                registrationMail = registrationMail.Replace("[Password]", password);
            }

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);
            //PartnerName
            registrationMail = registrationMail.Replace("[PartnerName]", partnerName);
            //CourseCount
            registrationMail = registrationMail.Replace("[CourseCount]", CourseCount);
            //CoursesNames
            registrationMail = registrationMail.Replace("[CoursesNames]", CoursesNames);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
        public void SendImportUserRegistrationCompleteEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string password,string partnerName)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            //Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            ////envolop
            //Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            ////phone
            //Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            ////facebook
            //Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            ////twitter
            //Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            //registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            if (password != null)
            {
                registrationMail = registrationMail.Replace("[Password]", password);
            }

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);
            //PartnerName
            registrationMail = registrationMail.Replace("[PartnerName]", partnerName);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
        public void SendStudentRegistrationCompleteEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            //if (password != null)
            //{
            //    registrationMail = registrationMail.Replace("[Password]", password);
            //}

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
        public void SendPasswordChangedEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            //if (password != null)
            //{
            //    registrationMail = registrationMail.Replace("[Password]", password);
            //}

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
        public void SendExternalRegistrationCompleteEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            //registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            //if (password != null)
            //{
            //    registrationMail = registrationMail.Replace("[Password]", password);
            //}

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
        public void SendCertificationEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string loginId, string pdfPath, string pdfName, string name, string course, string link, string linkedInlink)
        {
            string registrationMail = ReadTemplate(registTemplate);

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);
            //envolop Image
            Uri currentEnvImgUrl = new Uri(HttpContext.Current.Request.Url, "../Images/generic_email_cert.png");
            registrationMail = registrationMail.Replace("../Images/generic_email_cert.png", currentEnvImgUrl.AbsoluteUri);
            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //Student Name
            registrationMail = registrationMail.Replace("[Course Name]", course);
            //Certificate Link
            registrationMail = registrationMail.Replace("[link]", link);
            //LinkedIn Link
            registrationMail = registrationMail.Replace("[LinkedInlink]", linkedInlink);
            
            MandrillEmailService.EmailService.SendEmailWithAttachment(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail, pdfPath, pdfName);
        }
        public void SendPasswordResendEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string password)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);

            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);
            //Password
            registrationMail = registrationMail.Replace("[Password]", password);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method
            MandrillEmailService.EmailService.SendIndividualEmail(fromEmail, fromName, subject, loginId, registrationMail);
            return;
        }
        public void SendCouponAssignEmail(string fromName, string fromEmail, string telephone, string email, string subject,
          List<string> recipients, string registTemplate,
          string name, string logo, string loginId, string copCode,decimal discountPrice,string courseWithPrice,int allowedCourses,string partnerType,string partnerName,DateTime ValidFrom,DateTime validTo)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //Logo
         //   registrationMail = registrationMail.Replace("[Base64Image]", "../Content/assets/img/slider/Logo.png");
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);

            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);

            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);

            //Login Id
            registrationMail = registrationMail.Replace("[LoginID]", loginId);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //Partner Type
            registrationMail = registrationMail.Replace("[PartnerType]", partnerType);
            //Partner Name
            registrationMail = registrationMail.Replace("[PartnerName]", partnerName);
            //Coupon Code
            registrationMail = registrationMail.Replace("[CouponCode]", copCode);
            //Valid From
            registrationMail = registrationMail.Replace("[ValidFromDate]", ValidFrom.ToString());
            //Valid Too
            registrationMail = registrationMail.Replace("[ValidToDate]",validTo.ToString());
            //Discount in %
            registrationMail = registrationMail.Replace("[Discount%]", discountPrice.ToString());
            //Allowed courses
            registrationMail = registrationMail.Replace("[NumberofCourses]", allowedCourses.ToString());
            //
            registrationMail = registrationMail.Replace("[CoursesWithPrice]", courseWithPrice);
            
            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, recipients, registrationMail);

            return;
        }
        public void SendCouponAssignToAdmin(string fromName, string fromEmail, string telephone, string email, string subject,
        List<string> recipients, string registTemplate,
        string name,int NoOfCoupon,string couponTag, string logo, string copCode, decimal discountPrice, string courseWithPrice, int allowedCourses, string partnerType, string partnerName, DateTime ValidFrom, DateTime validTo)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //Logo
            //   registrationMail = registrationMail.Replace("[Base64Image]", "../Content/assets/img/slider/Logo.png");
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);

            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);

            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);
            //No Of coupon
            registrationMail = registrationMail.Replace("[NoOfCoupons]", NoOfCoupon.ToString());

            //Coupon Tag
            registrationMail = registrationMail.Replace("[CouponTag]", couponTag);

            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //Partner Type
            registrationMail = registrationMail.Replace("[PartnerType]", partnerType);
            //Partner Name
            registrationMail = registrationMail.Replace("[PartnerName]", partnerName);
            //Coupon Code
            registrationMail = registrationMail.Replace("[CouponCode]", copCode);
            //Valid From
            registrationMail = registrationMail.Replace("[ValidFromDate]", ValidFrom.ToString());
            //Valid Too
            registrationMail = registrationMail.Replace("[ValidToDate]", validTo.ToString());
            //Discount in %
            registrationMail = registrationMail.Replace("[Discount%]", discountPrice.ToString());
            //Allowed courses
            registrationMail = registrationMail.Replace("[NumberofCourses]", allowedCourses.ToString());
            //
            registrationMail = registrationMail.Replace("[CoursesWithPrice]", courseWithPrice);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, recipients, registrationMail);
            return;
        }
        public void SendPaymentSuccessEmail(string fromName, string fromEmail, string telephone, string email, string subject,
         List<string> recipients, string registTemplate,
         string name, string logo, long orderNo, DateTime orderDate, decimal totalPrice, string courseName, decimal basePrice)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //Logo
        //    registrationMail = registrationMail.Replace("[Base64Image]", "../Content/assets/img/slider/Logo.png");
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);

            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);

            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);


            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //CustomerName
            registrationMail = registrationMail.Replace("[CustomerName]", name);
            //Order number
            registrationMail = registrationMail.Replace("[OrderNumber]", orderNo.ToString());
            //Order date
            registrationMail = registrationMail.Replace("[OrderDate]", orderDate.ToString());
            //Course Name
            registrationMail = registrationMail.Replace("[CourseName]", courseName);
            //Final price
            registrationMail = registrationMail.Replace("[FinalPrice]", basePrice.ToString());
            //Total price
            registrationMail = registrationMail.Replace("[TOTALPrice]", totalPrice.ToString());
            //Total price
            registrationMail = registrationMail.Replace("[TaxAmount]", "0.00");

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, recipients, registrationMail);

            return;
        }

        public void SendResetPasswordEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string link)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);
            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);
            //UserName
            registrationMail = registrationMail.Replace("[UserName]", name);
            //Login Id
            registrationMail = registrationMail.Replace("[ResetPasswordLink]", string.Format(Constants.ResetPasswordLink,link));
            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }

        public void SendJobApplicationEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string loginId, string pdfPath, string pdfName, string name,string userEmail, string phone, string message, string jobTitle)
        {
            string registrationMail = ReadTemplate(registTemplate);

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);

            //JobTitle
            registrationMail = registrationMail.Replace("[JobTitle]", jobTitle);

            //Name
            registrationMail = registrationMail.Replace("[Name]", name);

            //UserEmail
            registrationMail = registrationMail.Replace("[UserEmail]", userEmail);

            //Phone
            registrationMail = registrationMail.Replace("[UserPhone]", phone);

            //Message
            registrationMail = registrationMail.Replace("[Message]", message);

            MandrillEmailService.EmailService.SendEmailWithAttachment(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail, pdfPath, pdfName);
        }
        public string ReadTemplate( string fileName)
        {
            try
            {
                return System.IO.File.ReadAllText(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ImageToBase64(string imgPath)
        {
            try
            {
                return System.Convert.ToBase64String(System.IO.File.ReadAllBytes(imgPath));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SendEmailCampaign()
        {

        }

        public void SendRefCodeEmail(string fromName, string fromEmail, string telephone, string email, string subject, string registTemplate, string name, string loginId, string referralCode, string rewardAmount, string validity,string userType,string operationType)
        {
            //readRe
            string registrationMail = ReadTemplate(registTemplate);
            //Update with data

            //../Content/assets/img/slider/Logo.png
            Uri currentUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/Logo.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/Logo.png", currentUrl.AbsoluteUri);
            //envolop
            Uri currentEnvUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/envelope_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/envelope_000000_18.png", currentEnvUrl.AbsoluteUri);
            //phone
            Uri currentPhoneUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/phone_000000_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/phone_000000_18.png", currentPhoneUrl.AbsoluteUri);
            //facebook
            Uri currentFbUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/facebook_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/facebook_ffffff_18.png", currentFbUrl.AbsoluteUri);
            //twitter
            Uri currentTwitterUrl = new Uri(HttpContext.Current.Request.Url, "../Content/assets/img/slider/twitter_ffffff_18.png");
            registrationMail = registrationMail.Replace("../Content/assets/img/slider/twitter_ffffff_18.png", currentTwitterUrl.AbsoluteUri);

            //Replace tags in email template
            if (userType == "Sender" && operationType == "ShareCode")
            {
                registrationMail = registrationMail.Replace("[SenderName]", name);
                registrationMail = registrationMail.Replace("[ReferralCode]", referralCode);
                registrationMail = registrationMail.Replace("[RewardAmount]", rewardAmount);
                registrationMail = registrationMail.Replace("[ReferralCodeValidity]", validity);
            }
            if (userType == "Sender" && operationType == "ApplyCode")
            {
                registrationMail = registrationMail.Replace("[ReceiverName]", name);
                registrationMail = registrationMail.Replace("[ReferralCode]", referralCode);
                registrationMail = registrationMail.Replace("[RewardAmount]", rewardAmount);
            }
            if (userType == "Receiver" && operationType == "ApplyCode")
            {
                registrationMail = registrationMail.Replace("[ReferralCode]", referralCode);
                registrationMail = registrationMail.Replace("[RewardAmount]", rewardAmount);
            }

            //Password
            //if (password != null)
            //{
            //    registrationMail = registrationMail.Replace("[Password]", password);
            //}

            //telephone
            registrationMail = registrationMail.Replace("[Phone]", telephone);

            //email
            registrationMail = registrationMail.Replace("[Email]", email);

            //call SendEmailService Method

            MandrillEmailService.EmailService.SendFormattedEmail(fromEmail, fromName, subject, new List<string> { loginId }, registrationMail);
            return;
        }
    }
}
