using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Models
{
    public class ProductCode
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Product Code")]
        public string ProdCode { get; set; }
        [DisplayName("Partner")]
        public int PartnerID { get; set; }
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

        [ForeignKey("PartnerID")]
        public Partner Partner { get; set; }
      
        [DisplayName("Course")]
        public int CourseID { get; set; }
        [ForeignKey("CourseID")]
        public Course Course { get; set; }
        public string CourseName
        {
            get
            {
                var courseName = string.Empty;
                var name = db.Courses.Where(s => s.Id == CourseID);
                foreach (var item in name)
                {
                    courseName = item.CourseName;
                }

                return courseName;

            }
        }

        [DisplayName("Fees")]
        public decimal Fees { get; set; }
        [DisplayName("Discount")]
        public decimal Discount { get; set; }
        [DisplayName("AllowedCourses")]
        public int NOfAllowedCourses { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
    }
}