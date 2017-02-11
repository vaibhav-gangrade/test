using System;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;

namespace Millionlights.Models
{
    public class CurrentCourse
    {
        private MillionlightsContext db = new MillionlightsContext();

        public int Id { get; set; }
        [DisplayName("Code")]
        public string Code { get; set; }
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
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }

        [DisplayName("Course Image Link")]
        public string CourseImageLink { get; set; }
        [DisplayName("Certification")]
        public int CertificationId { get; set; }
        //[ForeignKey("CertificationId")]
        public Certification Certification { get; set; }
        [DisplayName("BasePrice")]
        public decimal BasePrice { get; set; }
        [DisplayName("LMS Course Id")]
        public int LMSCourseId { get; set; }
        [DisplayName("LMS")]
        public bool EnableForLMS { get; set; }
        [DisplayName("Certification")]
        public bool EnableForCertification { get; set; }
        [DisplayName("Accept Payment")]
        public bool EnableForPayment { get; set; }
        [DisplayName("Duration")]
        public string Duration { get; set; }
        [DisplayName("Hours")]
        public int Hours { get; set; }
        [DisplayName("No Of Sessions")]
        public int NoOfSessions { get; set; }
        [DisplayName("CourseAvailability")]
        public int CourseAvailability { get; set; }
//        [ForeignKey("CourseAvailability")]
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
        [DisplayName("CourseCategory")]
        public int CourseCategory { get; set; }
        //[ForeignKey("CourseCategory")]
        public CourseCategory CourseCategoryId { get; set; }
        public string Category
        {
            get
            {
                var cats = string.Empty;
                var name = db.CourseCategories.Where(s => s.Id == CourseCategory);
                foreach (var item in name)
                {
                    cats = item.Name;
                }

                return cats;

            }
        }
        [DisplayName("CourseLevels")]
        public int CourseLevels { get; set; }
        //[ForeignKey("CourseLevels")]
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
        //[ForeignKey("CourseLanguage")]
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
        //[ForeignKey("CourseTypes")]
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
        //[ForeignKey("DeliveryID")]
        public CourseDelivery CourseDelivery { get; set; }
        [DisplayName("Instructor")]
        public string Instructor { get; set; }
        [DisplayName("Rating")]
        public string Rating { get; set; }
        [DisplayName("Display On HomePage")]
        public bool DisplayOnHomePage { get; set; }
        //public List<Course> courses { get; set; }
        public bool IsActive { get; set; }


    }
}