using System;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
   public class CoursesDetails
    {
       public int CourseId { get; set; }
       [DisplayName("Course Code")]
       public string CourseCode { get; set; }
       [DisplayName("Course Name")]
       public string CourseName { get; set; }
       [DisplayName("Short Description")]
       public string ShortDescription { get; set; }
        [AllowHtml]
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
       public string StartDateString
       {
           get
           {
               if (StartDate != null)
               {
                   return ((DateTime)StartDate).ToString(@"dd/MM/yyyy");
               }
               else
               {
                   return string.Empty;
               }
           }
       }
       [DisplayName("End Date")]
       public DateTime? EndDate { get; set; }
       public string EndDateString
       {
           get
           {
               if (EndDate != null)
               {
                   return ((DateTime)EndDate).ToString(@"dd/MM/yyyy");
               }
               else
               {
                   return string.Empty;
               }
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
       [DisplayName("Duration in (days)")]
       public string Duration { get; set; }
       [DisplayName("Hours per day")]
       public int Hours { get; set; }
       [DisplayName("No Of Sessions")]
       public int NoOfSessions { get; set; }
       [DisplayName("CourseAvailability")]
       public int CourseAvailability { get; set; }
       [ForeignKey("CourseAvailability")]
       public CourseAvailability CourseAvailabilityId { get; set; }
     
       [DisplayName("CourseCategory")]
       public string CourseCategory { get; set; }
       //[ForeignKey("CourseCategory")]
       //public CourseCategory CourseCategoryId { get; set; }
      
       [DisplayName("CourseLevels")]
       public int CourseLevels { get; set; }
       [ForeignKey("CourseLevels")]
       public CourseLevel CourseLevel { get; set; }
       [DisplayName("CourseLanguage")]
       public int CourseLanguage { get; set; }
       [ForeignKey("CourseLanguage")]
       public CourseLanguage CourseLanguageId { get; set; }
      
       [DisplayName("CourseTypes")]
       public int CourseTypes { get; set; }
       [ForeignKey("CourseTypes")]
       public CourseType CourseTypesId { get; set; }
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
       //public List<Course> courses { get; set; }
       public bool IsActive { get; set; }
       public int ChapterId { get; set; }

       [DisplayName("Stage Number")]
       public int ChapterNumber { get; set; }
       [DisplayName("Stage Name")]
       public string ChapterName { get; set; }
       [DisplayName("Stage Description")]
       public string ChapterDescription { get; set; }

       [DisplayName("Evidence required for course certificate")]
       public bool EvidenceRequired { get; set; }
       [DisplayName("Set priority level to display on home page")]
       public int? CoursePriority { get; set; }

       [DisplayName("Certificate Template")]
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
}
