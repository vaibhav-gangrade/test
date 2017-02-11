using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class OrderItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        public int OrderId { get; set; }
        public string UserCourseId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}