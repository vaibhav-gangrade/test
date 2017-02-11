using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public int PartnerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int AddressId { get; set; }
        public Int64 Mobile { get; set; }
        public Int64? PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool CanAccessLMS { get; set; }
        public bool CanPay { get; set; }
        public bool CanAccessCertification { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string Institute { get; set; }
        public string Class { get; set; }
        public string Company { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string status { get; set; }
    }
}