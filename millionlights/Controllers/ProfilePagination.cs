using Millionlights.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Controllers
{
    public class ProfilePagination
    {
        private MillionlightsContext db = new MillionlightsContext();
        public int CoupanId { get; set; }
        public int? CCGId { get; set; }

        [DisplayName("Coupon Code")]
        public string CouponCode { get; set; }

        [DisplayName("Partner")]
        public int PartnerID { get; set; }
        [ForeignKey("PartnerID")]
        public Partner partner { get; set; }
        public string PartnerName
        {
            get
            {
                var partname = string.Empty;
                var name = db.Partners.Where(s => s.Id == PartnerID);
                foreach (var item in name)
                {
                    partname = item.Name;
                }

                return partname;

            }
        }
        public int AllowedCourses { get; set; }
        public int BenifitId { get; set; }
        [ForeignKey("BenifitId")]
        public BenifitType BenifitType { get; set; }

        public string BenefitName
        {
            get
            {
                var benifitType = string.Empty;
                var name = db.BenifitTypes.Where(s => s.BenifitId == BenifitId);
                foreach (var item in name)
                {
                    benifitType = item.BenifitName;
                }

                return benifitType;

            }
        }

        public Double BenefitDiscont
        {
            get
            {
                var benifitDiscount = 0.00;
               // var name = db.BenifitTypes.Where(s => s.BenifitId == BenifitId);
                var name = db.Coupons.Join(db.BenifitTypes, a => a.BenifitId, b => b.BenifitId, (a, b) => new { a, b }).ToList();
                foreach (var item in name)
                {
                    benifitDiscount = Convert.ToDouble(item.a.DiscountPrice);
                }

                return benifitDiscount;

            }
        }


        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        [DisplayName("Status")]
        public int? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public CouponStatus CouponStatus { get; set; }
        public string StatusType
        {
            get
            {
                var statusName = string.Empty;
                var name = db.CouponStatus.Where(s => s.StatusId == StatusId);
                foreach (var item in name)
                {
                    statusName = item.StatusName;
                }

                return statusName;

            }
        }
        public int? ActivatedBy { get; set; }
        [ForeignKey("ActivatedBy")]
        public User ActivatedByUser { get; set; }
        public string Username
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

        public DateTime? ActivatedOn { get; set; }
        public DateTime? BlockedOn { get; set; }
        public string BlockedReason { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User createdByUser { get; set; }
        public string CreatedUserName
        {
            get
            {
                var userName = string.Empty;
                var name = db.Users.Where(s => s.UserId == CreatedBy);
                foreach (var item in name)
                {
                    userName = item.UserName;
                }
                return userName;
            }
        }

        public DateTime CreatedOn { get; set; }
        public bool IsPrepaid { get; set; }
        public bool IsActive { get; set; }
        public string EmailId { get; set; }
        public long MobileNo { get; set; }
        public int NoOfCoupon { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User userid { get; set; }
        public int? ProsUserId { get; set; }
        [ForeignKey("ProsUserId")]
        public TmpUser TmpUser { get; set; }

    }
}