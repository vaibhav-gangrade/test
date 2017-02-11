using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Millionlights.Models
{
    public class CourseContent
    {
        private MillionlightsContext db = new MillionlightsContext();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("CourseId")]
        public int CourseId { get; set; }
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
        [DisplayName("Chapter Number")]
        public int ChapterNumber { get; set; }
        [DisplayName("Chapter Name")]
        public string ChapterName { get; set; }
        [DisplayName("Chapter Description")]
        public string ChapterDescription { get; set; }
    }
}