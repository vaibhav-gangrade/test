using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class HomePageConfiguration
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsPromoEnabled { get; set; }
         [DisplayName("Promotion")]
        public string Promotext { get; set; }
        [DisplayName("Millionlight Youtube Channel")]
        public string VideoUrl { get; set; }
        public bool IsActive { get; set; }
         [DisplayName("Terms And Condition")]
        public string TermsAndCondition { get; set; }
        [DisplayName("Referral Code Reward Amount")]
         public string RewardAmount { get; set; }

    }
}
