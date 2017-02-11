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
    public class ReferralCodes
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReferralCode { get; set; }
        public int? Referrer { get; set; }
        [ForeignKey("Referrer")]
        public User ReferredBy { get; set; }
        [NotMapped]
        public string ReferrerEmail
        {
            get
            {
                string emailId = db.Users.Where(s => s.UserId == Referrer).FirstOrDefault().EmailId;
                return emailId;
            }
        }
        public string ReceiverEmail { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public DateTime? SharedOn{ get; set; }
        [NotMapped]
        public string SharedOnString
        {
            get
            {
                if (SharedOn != null)
                {
                    return ((DateTime)SharedOn).ToString(@"dd/MM/yyyy H:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public bool IsCodeUsed{ get; set; }
        [NotMapped]
        public string CodeUsed
        {
            get
            {
                string yesNo = null;
                if (IsCodeUsed == true)
                {
                    yesNo = "YES";
                }
                else
                {
                    yesNo = "NO";
                }
                return yesNo;
            }
        }
        public DateTime? CodeUsedOn{ get; set; }
        [NotMapped]
        public string CodeUsedOnString
        {
            get
            {
                if (CodeUsedOn != null)
                {
                    return ((DateTime)CodeUsedOn).ToString(@"dd/MM/yyyy H:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public int? Receiver { get; set; }
        [ForeignKey("Receiver")]
        public User ReceivedBy { get; set; }
        public decimal? ReferrerRewardAmount { get; set; }
        public decimal? ReceiverRewardAmount { get; set; }
        public DateTime? CodeGeneratedOn { get; set; }
        [NotMapped]
        public string CodeGeneratedOnString
        {
            get
            {
                if (CodeGeneratedOn != null)
                {
                    return ((DateTime)CodeGeneratedOn).ToString(@"dd/MM/yyyy H:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public DateTime? CodeValidity { get; set; }
        [NotMapped]
        public string CodeValidityString
        {
            get
            {
                if (CodeValidity != null)
                {
                    return ((DateTime)CodeValidity).ToString(@"dd/MM/yyyy H:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public bool IsActive{ get; set; }
    }
}
