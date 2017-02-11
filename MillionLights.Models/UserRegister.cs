using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class UserRegister
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        private MillionlightsContext db = new MillionlightsContext();
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string UserType { get; set; }
        public string ProviderKey { get; set; }
        public int UserDetailsId { get; set; }
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
        public string ConfirmPassword { get; set; }
        [DisplayName("UserRole")]
        public int RoleId { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        [AllowHtml]
        public string Biography { get; set; }
        public string ImageURL { get; set; }
        public int PartnerId { get; set; }

        public string PartnerName
        {
            get
            {
                var avail = string.Empty;
                var part = db.Partners.Where(s => s.Id == PartnerId);
                foreach (var item in part)
                {
                    avail = item.Name;
                }
                return avail;
            }
        }
        public string RoleName
        {
            get
            {
                var roleName = string.Empty;
                var roles = db.Roles.Where(s => s.RoleId == RoleId);
                foreach (var item in roles)
                {
                    roleName = item.RoleName;
                }
                return roleName;
            }
        }
        public string RegisteredDatetimeString
        {
            get
            {
                if (RegisteredDatetime != null)
                {
                    return ((DateTime)RegisteredDatetime).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public int? data { get; set; }
        [NotMapped]
        public string RefCode { get; set; }
    }
   
}