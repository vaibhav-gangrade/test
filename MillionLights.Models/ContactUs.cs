using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class ContactUs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactEnquireId { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Phone")]
        public Int64? PhoneNo { get; set; }
        [DisplayName("Message")]
        public string Message { get; set; }
    }

}
