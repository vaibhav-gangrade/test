using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class DuplicateRecords
    {
        [Key]
        public int TmpId { get; set; }

        public string FirsrName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public Int64? MobileNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public int? CCGId { get; set; }
        public string UserType { get; set; }
        public int? PartnerId { get; set; }
        [ForeignKey("PartnerId")]
        public Partner Partner { get; set; }
    }
}