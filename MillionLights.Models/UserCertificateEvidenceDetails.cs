using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
     public class UserCertificateEvidenceDetails
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CourseId { get; set; }
        //[ForeignKey("CourseId")]
        //public Course Course { get; set; }
        public int UsersCertificateId { get; set; }
        [ForeignKey("UsersCertificateId")]
        public UsersCertificate UsersCertificate { get; set; }
        public int? EvidenceId { get; set; }
        [ForeignKey("EvidenceId")]
        public CertificateEvidenceLkp CertificateEvidenceLkp { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string EvidenceNumber { get; set; }
        public string ImageUrl { get; set; }
        public string EvidenceUploadedPath { get; set; }
        public DateTime? EvidenceIssueDate { get; set; }
        public DateTime? EvidenceExpiry { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public string IssuedDateString
        {
            get
            {
                return ((DateTime)EvidenceIssueDate).ToString(@"dd/MM/yyyy");
            }
        }
        [NotMapped]
        public string EvidenceExpiryString
        {
            get
            {
                return ((DateTime)EvidenceExpiry).ToString(@"dd/MM/yyyy");
            }
        }
    }
}
