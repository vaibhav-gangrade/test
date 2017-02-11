using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class CATS
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{get;set;}

        [DisplayName("Customer Id")]
        public int CustomerId { get; set; }
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        [DisplayName("Customer Type")]
        public string CustomerTypeDesc { get; set; }
        [DisplayName("Account Status")]
        public string AccountStatus { get; set; }
        [DisplayName("Sector")]
        public string Sector { get; set; }
        [DisplayName("Country Name")]
        public string CountryName { get; set; }
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }
        [DisplayName("Address Line1")]
        public string AddressLine1 { get; set; }

        [DisplayName("Address Line2")]
        public string AddressLine2 { get; set; }
        [DisplayName("Web Address")]
        public string WebAddress { get; set; }
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        [DisplayName("ModifiedOn")]
        public DateTime ModifiedOn { get; set; }
        [DisplayName("City")]
        public string City { get; set; }

    }
}