using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class Coupon
    {
        public Coupon()
        {
        }
        public Coupon(int id, int status, string code, int benefitId, int partnerId, DateTime validFrom, DateTime validTo, int? createdBy, bool isPrepaid,int allowCourses)
        {
            Id = id;
            StatusId = status;
            CouponCode = code;
            CreatedOn = DateTime.Now;
            IsActive = true;
            BlockedOn = null;
            BenifitId = benefitId;
            PartnerID = partnerId;
            ValidFrom = validFrom;
            ValidTo = validTo;
            CreatedBy = createdBy;
            IsPrepaid = isPrepaid;
            AllowedCourses = allowCourses;
        }
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CCGId { get; set; }

        [DisplayName("Coupon Code")]
        public string CouponCode { get; set; }

        [DisplayName("Partner")]
        public int? PartnerID { get; set; }
        [ForeignKey("PartnerID")]
        public Partner partner { get; set; }
        public string PartnerName
        {
            get
            {
                Partner partner = db.Partners.Where(s => s.Id == PartnerID).FirstOrDefault();
                return partner.Name;
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

        //public Double BenefitDiscont
        //{
        //    get
        //    {
        //        var benifitDiscount = 0.00;
        //        var name = db.BenifitTypes.Where(s => s.BenifitId == BenifitId);
        //        foreach (var item in name)
        //        {
        //            benifitDiscount = Convert.ToDouble(item.DiscountPrice);
        //        }

        //        return benifitDiscount;

        //    }
        //}


        public DateTime ValidFrom { get; set; }

        [NotMapped]
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
        public string CouponValidity
        {
            get
            {
                return (ValidFromString + "-" + ValidToString);
            }
        }
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
        //public int? ActivatedBy { get; set; }
        //[ForeignKey("ActivatedBy")]
        //public User ActivatedByUser { get; set; }
        //public string Username
        //{
        //    get
        //    {
        //        var userName = string.Empty;
        //        var name = db.Users.Where(s => s.UserId == ActivatedBy);
        //        foreach (var item in name)
        //        {
        //            userName = item.UserName;
        //        }

        //        return userName;

        //    }
        //}
        //public DateTime? ActivatedOn { get; set; }
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
        public string CreatedOnString
        {
            get
            {
                if (CreatedOn != null)
                {
                    return ((DateTime)CreatedOn).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public bool IsPrepaid { get; set; }
        public bool IsActive { get; set; }
        public string EmailId { get; set; }
        public long? MobileNo { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User usersid { get; set; }
        public int? ProsUserId { get; set; }
        [ForeignKey("ProsUserId")]
        public TmpUser TmpUser { get; set; }
        public decimal DiscountPrice { get; set; }
        [DisplayName("Allow anyone to redeem")]
        public bool MultiRedeem { get; set; }
        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }
        public string CouponTag { get; set; }
        [NotMapped]
        public string CouponRedeemStatus { get; set; } 
    }

}