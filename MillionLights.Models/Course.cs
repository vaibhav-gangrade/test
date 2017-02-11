using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class Course
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Course Code")]
        public string CourseCode { get; set; }
        [DisplayName("Course Name")]
        public string CourseName { get; set; }
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }
        [DisplayName("Long Description")]
        public string LongDescription { get; set; }
        [AllowHtml]
        [DisplayName("Objective")]
        public string Objective { get; set; }
        [AllowHtml]
        [DisplayName("Exam Objective")]
        public string ExamObjective { get; set; }
        [DisplayName("EDX Course Link")]
        public string EDXCourseLink { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [NotMapped]
        public string stDateString
        {
            get
            {
                if (StartDate != null)
                {
                    return ((DateTime)StartDate).ToString(@"dd/MM/yyyy");
                }
                return string.Empty;
            }
        }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [NotMapped]
        public string endDateString
        {
            get
            {
                if (EndDate != null)
                {
                    return ((DateTime)EndDate).ToString(@"dd/MM/yyyy");
                }
                return string.Empty;
            }
        }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }

        [DisplayName("Course Image Link")]
        public string CourseImageLink { get; set; }
        [DisplayName("Certification")]
        public int? CertificationId { get; set; }
        [ForeignKey("CertificationId")]
        public Certification Certification { get; set; }
        [DisplayName("BasePrice")]
        public decimal BasePrice { get; set; }
        [DisplayName("LMS Course Id")]
        public string LMSCourseId { get; set; }
        [DisplayName("LMS")]
        public bool EnableForLMS { get; set; }
        [DisplayName("Certification")]
        public bool EnableForCertification { get; set; }
        [DisplayName("Accept Payment")]
        public bool EnableForPayment { get; set; }
        [DisplayName("Duration")]
        public string Duration { get; set; }
        [DisplayName("Hours per day")]
        public int Hours { get; set; }
        [DisplayName("No Of Sessions")]
        public int NoOfSessions { get; set; }
        [DisplayName("CourseAvailability")]
        public int CourseAvailability { get; set; }
        [ForeignKey("CourseAvailability")]
        public CourseAvailability CourseAvailabilityId { get; set; }
        public string Availability
        {
            get
            {
                var avail = string.Empty;
                var name = db.CourseAvailability.Where(s => s.Id == CourseAvailability);
                foreach (var item in name)
                {
                    avail = item.Name;
                }

                return avail;

            }
        }
        public string CourseCertificationName
        {
            get
            {
                var certName = string.Empty;
                var name = db.Certifications.Where(s => s.Id == CertificationId);
                foreach (var item in name)
                {
                    certName = item.Name;
                }

                return certName;

            }
        }
        [DisplayName("CourseCategory")]
        public string CourseCategory { get; set; }
        public string CategoryName
        {
            get
            {
                return db.CourseCategories.Where(x => CourseCategory.Contains(x.Id.ToString())).FirstOrDefault().Name;
            }
        }
        [DisplayName("CourseLevels")]
        public int CourseLevels { get; set; }
        [ForeignKey("CourseLevels")]
        public CourseLevel CourseLevel { get; set; }
        public string Level
        {
            get
            {
                var level = string.Empty;
                var name = db.CourseLevels.Where(s => s.Id == CourseLevels);
                foreach (var item in name)
                {
                    level = item.Name;
                }

                return level;

            }
        }
        [DisplayName("CourseLanguage")]
        public int CourseLanguage { get; set; }
        [ForeignKey("CourseLanguage")]
        public CourseLanguage CourseLanguageId { get; set; }
        public string Language
        {
            get
            {
                var lang = string.Empty;
                var name = db.CourseLanguages.Where(s => s.Id == CourseLanguage);
                foreach (var item in name)
                {
                    lang = item.Name;
                }

                return lang;

            }
        }
        [DisplayName("CourseTypes")]
        public int CourseTypes { get; set; }
        [ForeignKey("CourseTypes")]
        public CourseType CourseTypesId { get; set; }
        public string Type
        {
            get
            {
                var type = string.Empty;
                var name = db.CourseTypes.Where(s => s.Id == CourseTypes);
                foreach (var item in name)
                {
                    type = item.Name;
                }

                return type;

            }
        }
        [DisplayName("Sample Contents")]
        public bool HasSampleContents { get; set; }
        [DisplayName("Sample Contents Link")]
        public string SampleContentsLink { get; set; }
        [DisplayName("Credit Points")]
        public int CreditPoints { get; set; }
        [DisplayName("Certification Provider")]

        public int? CertificationProvider { get; set; }

        [DisplayName("Exam Manager")]
        public int? ExamManager { get; set; }
        [DisplayName("Course Provider")]
        public int? CourseProvider { get; set; }
        [DisplayName("Video Link")]
        public bool HasVideoLink { get; set; }
        [DisplayName("Video Link")]
        public string VideoLink { get; set; }
        [DisplayName("Schedule Applicable")]
        public bool ScheduleApplicable { get; set; }
        [DisplayName("Delivery Method")]
        public int DeliveryID { get; set; }
        [ForeignKey("DeliveryID")]
        public CourseDelivery CourseDelivery { get; set; }
        [DisplayName("Instructor")]
        public string Instructor { get; set; }
        [DisplayName("Rating")]
        public string Rating { get; set; }
        [DisplayName("Display On HomePage")]
        public bool DisplayOnHomePage { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }
        [DisplayName("Evidence required for course certificate")]
        public bool EvidenceRequired { get; set; }
        [DisplayName("Set priority level to display on home page")]
        public int CoursePriority { get; set; }
        public int CourseRatings { get; set; }

        [DisplayName("Certificate Template(Background Image)")]
        public string CertificateTemplate { get; set; }

        [DisplayName("Certificate Logo")]
        public string CertificateLogo { get; set; }

        [DisplayName("Certificate Signature")]
        public string CertificateSignature { get; set; }

        [DisplayName("Certificate Template(.htm File)")]
        public string CertificateTemplateHtmFile { get; set; }

        [DisplayName("Use Default Certificate Contents")]
        public bool UseDefaultCertificateContents { get; set; }
        
    }

    public class ShortCourse
    {
        private MillionlightsContext db = new MillionlightsContext();

        public int Id { get; set; }
        private string coursename;
        public string CourseName
        {
            get
            {
                return coursename;
            }
            set
            {
                coursename = value;
            }
        }
        public decimal BasePrice { get; set; }
        private string shortdesc;
        public string ShortDescription
        {
            get
            {
                return shortdesc;
            }
            set
            {
                shortdesc = value;
            }
        }
        public string Category { get; set; }
        public string CourseImageLink { get; set; }
        public bool DisplayonHomePage { get; set; }
        public string Duration { get; set; }
        public int AvailabilityId { get; set; }
        public string Availability { get; set; }
        public int CourseLevel { get; set; }
        public int CourseType { get; set; }
        public int CourseLanguage { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CoursePriority { get; set; }
        [NotMapped]
        public bool IsCourseEnrolled { get; set; }
        [NotMapped]
        public int CourseRatings { get; set; }
        [NotMapped]
        public string GoogleCourseDescription { get; set; }

        [NotMapped]
        public string CategoryName
        {
            get
            {
                //return db.CourseCategories.Where(x => x.Id.ToString().Equals(Category)).FirstOrDefault().Name;

                string[] cat;
                cat = Category.Split(',');
                string catId = cat[0].ToString();
                return db.CourseCategories.Where(x => x.Id.ToString().Equals(catId)).FirstOrDefault().Name;

            }
        }

        [NotMapped]
        public string EmailCourseName { get; set; }
        [NotMapped]
        public string EDXCourseLink { get; set; }
        [NotMapped]
        public string CourseDirectURL
        {
            get
            {
                return System.Web.HttpContext.Current.Request.Url.Host + "/Course/AboutCourse/" + System.Uri.EscapeDataString(CourseName.Trim()) + "/";
            }
        }
        public string EncodedDescription
        {
            get
            {
                return System.Uri.EscapeDataString(ShortDescription);
            }
        }
        public string EncodedCourseName
        {
            get
            {
                return System.Uri.EscapeDataString(CourseName);
            }
        }
        public string EncodedImageLink
        {
            get
            {
                return System.Uri.EscapeDataString(CourseImageLink);
            }
        }

        public string EncodedBasePrice
        {
            get
            {
                return System.Uri.EscapeDataString(BasePrice.ToString());
            }
        }
    }
}

