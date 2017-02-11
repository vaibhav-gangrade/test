using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class OrderStatus
    {
       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OrderStatusID { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

    }
}