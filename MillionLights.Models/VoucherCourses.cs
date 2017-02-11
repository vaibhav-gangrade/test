using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Models
{
    public class VoucherCourses
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int VoucherId { get; set; }
        [ForeignKey("VoucherId")]
        public VoucherCode VoucherCode { get; set; }
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
        public bool IsActivated { get; set; }
    }
}