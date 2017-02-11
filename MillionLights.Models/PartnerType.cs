using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class PartnerType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PartnerTypeName { get; set; }

        public bool IsActive { get; set; }
    }
}