using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class CourseLanguage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("IsActive")]
        public string IsActive { get; set; }
    }
}