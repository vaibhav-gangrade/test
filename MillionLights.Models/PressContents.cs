using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class PressContents
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Press Page Heading")]
        public string PressPageHeading { get; set; }
        [DisplayName("Press Image")]
        public string PressImage { get; set; }
        [DisplayName("Press ShortDescription")]
        public string PressShortDescription { get; set; }
        [DisplayName("Press LongDescription")]
        public string PressLongDescription { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [NotMapped]
        public List<PressContents> PressContent { get; set; }
    }
}
