using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class UserDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserDetailsId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public Int64? PhoneNumber { get; set; }
        public DateTime RegisteredDatetime { get; set; }
        public bool IsActive { get; set; }
        public int PartnerId { get; set; }
        public string Biography { get; set; }
        public string ImageURL { get; set; }
       
        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }        //public List<UserDetails> userDetails { get; set; }
    }
}