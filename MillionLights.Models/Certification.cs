using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Millionlights.Models
{
    public class Certification
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }
        [DisplayName("Long Description")]
        public string LongDescription { get; set; }
        [DisplayName("Price")]
        public decimal Price { get; set; }
        [DisplayName("Available Attempts")]
        public int AvailableAttempts { get; set; }
        [DisplayName("Certificate Image Link")]
        public string CertImageLink { get; set; }

        public string Objective { get; set; }
        public string Benifits { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int? CourseCategoryId { get; set; }
        public string CourseCategoryName
        {
            get
            {
                var catname = string.Empty;
                var name = db.CourseCategories.Where(s => s.Id == CourseCategoryId);
                foreach (var item in name)
                {
                    catname = item.Name;
                }
                return catname;
            }
        }

        public int? CourseId { get; set; }
        public string CourseNameList
        {
            get
            {
                var coursename = string.Empty;
                var name = db.Courses.Where(s => s.Id == CourseId);
                foreach (var item in name)
                {
                    coursename = item.CourseName;
                }

                return coursename;

            }
        }
        public int? PartnerID { get; set; }
        public string PartnerName
        {
            get
            {
                var partname = string.Empty;
                var name = db.Partners.Where(s => s.Id == PartnerID);
                foreach (var item in name)
                {
                    partname = item.Name;
                }
                return partname;
            }
        }
        public bool IsActive { get; set; }
        [NotMapped]
        [AllowHtml]
        public string Csv { get; set; }
    }
}