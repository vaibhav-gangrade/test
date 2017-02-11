using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Millionlights.Models
{
    public class ItemsOrdered
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ItemOrderedID { get; set; }
        public int OrderID { get; set; }
        public int CourseId { get; set; }
        
        public string CourseName
        {
            get
            {
                var Coursename = string.Empty;
                var name = db.Courses.Where(s => s.Id == CourseId);
                foreach (var item in name)
                {
                    Coursename = item.CourseName;
                }

                return Coursename;

            }
        }
        //[ForeignKey("CourseId")]
        //public Course Course { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public int CouponId { get; set; }
    }
}