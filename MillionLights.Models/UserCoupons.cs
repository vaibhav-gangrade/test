using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
  public class UserCoupons
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
   
        public int Id { get; set; }
        public int ActivateBy { get; set; }
        [ForeignKey("ActivateBy")]
        public User users { get; set; }
        public DateTime? ActivateOn { get; set; }
        public int CouponId { get; set; }
        [ForeignKey("CouponId")]
        public Coupon coupon { get; set; }
        public string RedeemStatus { get; set; }    

    }
}
