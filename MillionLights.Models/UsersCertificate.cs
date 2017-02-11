using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class UsersCertificate
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User user { get; set; }

        public int CourseID { get; set; }
        [ForeignKey("CourseID")]
        public Course course { get; set; }

        public string CertificationID { get; set; }
        public string CertificatePath { get; set; }

        public DateTime? IssuedDate { get; set; }

        [NotMapped]
        public string IssuedDateString
        {
            get
            {
                return ((DateTime)IssuedDate).ToString(@"dd/MM/yyyy");
            }
        }
        [NotMapped]
        public string CertiStartDateString
        {
            get
            {
                return ((DateTime)IssuedDate).ToString(@"yyyyMM");
            }
        }
        [NotMapped]
        public string FullName
        {
            get
            {
                var userDetails = db.UsersDetails.Where(x => x.UserId == UserID).FirstOrDefault();
                return userDetails.FirstName + " " + userDetails.LastName;
            }
        }
        [NotMapped]
        public string Email
        {
            get
            {
                var userDetails = db.Users.Where(x => x.UserId == UserID).FirstOrDefault();
                return userDetails.EmailId;
            }
        }
 
        [NotMapped]
        public string CourseName
        {
            get
            {
                var course = db.Courses.Where(x => x.Id == CourseID).FirstOrDefault();
                return course.CourseName;
            }
        }
        [NotMapped]
        public string CourseNameEncode
        {
            get
            {
                var course = db.Courses.Where(x => x.Id == CourseID).FirstOrDefault();
                return System.Web.HttpUtility.UrlEncode(course.CourseName);
            }
        }
        [NotMapped] 
        public string PdfPath{
            get{
                var path = "https://www.millionlights.org" + CertificatePath;
                return path;
            }
        }
        [NotMapped]
        public string CourseImage
        {
            get
            {
                var course = db.Courses.Where(x => x.Id == CourseID).FirstOrDefault();
                return course.CourseImageLink;
            }
        }
        [NotMapped]
        public string CourseShortDesc
        {
            get
            {
                var course = db.Courses.Where(x => x.Id == CourseID).FirstOrDefault();
                return course.ShortDescription;
            }
        }

        [NotMapped]
        public string CoursePartner
        {
            get
            {
                 var course = db.Courses.Where(x => x.Id == CourseID).FirstOrDefault();
                 var cert = db.Partners.Where(x => x.Id == course.CertificationProvider).FirstOrDefault();
                return cert.Name;
            }
        }
    }
}
