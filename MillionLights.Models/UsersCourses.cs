using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class UsersCourses
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UCID { get; set; }
        public int UserId { get; set; }
        public int CourseID { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Status { get; set; }
        public string CouponApplied { get; set; }
        public DateTime VoucherAssignedOn { get; set; }
        public bool CanAccessLMS { get; set; }
        public bool CanPay { get; set; }
        public bool CanAccessCertification { get; set; }
        public bool IsActive { get; set; }
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
        public string VoucherAssignedOnString
        {
            get
            {
                if (VoucherAssignedOn != null)
                {
                    return ((DateTime)VoucherAssignedOn).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string CourseStatus { get; set; }

    }
}