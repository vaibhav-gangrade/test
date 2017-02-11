using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Models
{
    public class VoucherCode
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Voucher Code")]
        public string VouchCode { get; set; }
        [DisplayName("Voucher Type")]
        public string VoucherType { get; set; }
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [DisplayName("Partner")]
        public int? PartnerID { get; set; }
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
        [DisplayName("Status")]
        public int? StatusId { get; set; }
        public string VoucherStatus
        {
            get
            {
                var voucherStatus = string.Empty;
                var name = db.OrderStatus.Where(s => s.OrderStatusID == StatusId);
                foreach (var item in name)
                {
                    voucherStatus = item.Status;
                }

                return voucherStatus;

            }
        }
        [DisplayName("Activated By")]
        public int? ActivatedById { get; set; }
        public string ActivatedBy
        {
            get
            {
                var activatedBy = string.Empty;
                var name = db.Users.Where(s => s.UserId == ActivatedById);
                foreach (var item in name)
                {
                    activatedBy = item.UserName;
                }

                return activatedBy;

            }
        }
        [DisplayName("Activated Course")]
        public int? CourseId{ get; set; }
        public string ActivatedCourse
        {
            get
            {
                var courseName = string.Empty;
                var name = db.Courses.Where(s => s.Id == CourseId);
                foreach (var item in name)
                {
                    courseName = item.CourseName;
                }

                return courseName;

            }
        }
        [DisplayName("Blocked On")]
        public DateTime? BlockedOn { get; set; }
        [DisplayName("Blocked Reason")]
        public string BlockedReason { get; set; }
        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }
        [DisplayName("Activated Course")]
        public int? CreatedByUserId { get; set; }
        public decimal? Discount { get; set; }
        public string CreatedBy
        {
            get
            {
                var createdBy = string.Empty;
                var name = db.Users.Where(s => s.UserId == CreatedByUserId);
                foreach (var item in name)
                {
                    createdBy = item.UserName;
                }

                return createdBy;

            }
        }
        public int? AllowedCourses { get; set; }
       
    }
}