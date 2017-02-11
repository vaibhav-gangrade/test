using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class Partner
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public int PartnerTypeId { get; set; }

        public string PartnerName
        {
            get
            {
                var partnerName = string.Empty;
                var name = db.PartnerType.Where(s => s.Id == PartnerTypeId);
                foreach (var item in name)
                {
                    partnerName = item.PartnerTypeName;
                }
                return partnerName;
            }

        }
        public bool CanAccessLMS { get; set; }
        public bool CanPay { get; set; }
        public bool CanAccessCertification { get; set; }
        public string Email { get; set; }
        public Int64? PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ImageLink { get; set; }
        public string PartnerUrl { get; set; }
        public string DisplayMessage { get; set; }
        public bool IsActive { get; set; }
        public bool DisplayOnHomePage { get; set; }
        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }
    }

    public class PartnerImage
    {
        public int Id { get; set; }
        public string ImageLink { get; set; }
        public bool DisplayOnHomePage { get; set; }
    }
}