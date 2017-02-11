using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class UsersTable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string ProviderKey { get; set; }
        public bool IsActive { get; set; }
        public List<User> users { get; set; }
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
        public int PartnerId { get; set; }
        public string Biography { get; set; }
        public string ImageURL { get; set; }

        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }
        public int RoleId { get; set; }
    }
}
