using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Models
{
    public class CouponCourses
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CouponId { get; set; }
        [ForeignKey("CouponId")]
        public Coupon Coupon { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public string CourseName
        {
            get
            {
                var courseName = string.Empty;
                var name = db.Courses.Where(s => s.Id == CourseId);
                foreach (var item in name)
                {
                    courseName = item.CourseName;
                }

                return courseName;

            }
        }
        [NotMapped]
        public string CourseCode { get; set; }
        [NotMapped]
        public decimal BasePrice { get; set; }
        [NotMapped]
        public string ShortDescription { get; set; }
        [NotMapped]
        public string IsCourseRedeemed { get; set; }
        [NotMapped]
        public string CName { get; set; }
    }
}