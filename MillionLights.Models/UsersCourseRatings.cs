using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class UsersCourseRatings
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? RatingsProvidedBy { get; set; }
        [ForeignKey("RatingsProvidedBy")]
        public User UserId { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course CId { get; set; }
        public decimal? CourseRatings { get; set; }
        public string UsersComments { get; set; }

        public DateTime? RatingDatetime { get; set; }
       
        public bool IsActive { get; set; }
    }
}
