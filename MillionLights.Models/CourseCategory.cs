using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class CourseCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
       
    }
}