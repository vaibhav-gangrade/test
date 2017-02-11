using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class BenifitType
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BenifitId { get; set; }
        public string BenifitName{get;set;}
        public bool IsActive { get; set; }
     
    }
}