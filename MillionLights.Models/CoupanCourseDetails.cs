using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class CoupanCourseDetails
    {
        private MillionlightsContext db = new MillionlightsContext();

        public int CoupanId { get; set; }
        public int? CCGId { get; set; }

        [DisplayName("Coupon Code")]
        public string CouponCode { get; set; }

        public int PartnerID { get; set; }

        public string PartnerName { get; set; }

        public int AllowedCourses { get; set; }
        public int BenifitId { get; set; }

        public string BenefitName { get; set; }

        public Double BenefitDiscont { get; set; }

        public DateTime ValidFrom { get; set; }
        public string ValidFromString
        {
            get
            {
                if (ValidFrom != null)
                {
                    return ((DateTime)ValidFrom).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public DateTime ValidTo { get; set; }
        public string ValidToString
        {
            get
            {
                if (ValidTo != null)
                {
                    return ((DateTime)ValidTo).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int? StatusId { get; set; }

        public string StatusType { get; set; }

        public DateTime? ActivatedOn { get; set; }
        public string ActivatedOnDatetimeString
        {
            get
            {
                if (ActivatedOn != null)
                {
                    return ((DateTime)ActivatedOn).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public DateTime? BlockedOn { get; set; }

        public string BlockedReason { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool IsPrepaid { get; set; }
        public bool IsActive { get; set; }
        public int CoupanCourseId { get; set; }
        public int CoupanCourseCouponId { get; set; }
        public int CoupanCourseCourseId { get; set; }
        public int CourseId { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string ShortDescription { get; set; }

        public decimal BasePrice { get; set; }
        [NotMapped]
        public int? ActivatedBy { get; set; }
        [NotMapped]
        public string ActivatedByUsername
        {
            get
            {
                var userName = string.Empty;
                var name = db.Users.Where(s => s.UserId == ActivatedBy);
                foreach (var item in name)
                {
                    userName = item.UserName;
                }

                return userName;

            }
        }
        [NotMapped]
        public string CouponRedeemStatus { get; set; }
        [NotMapped]
        public string CouponStatus { get; set; }
        //[NotMapped]
        //public int CourseCount { get; set; } 
    
    }
}