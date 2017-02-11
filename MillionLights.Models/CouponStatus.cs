using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class CouponStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }

    }
}