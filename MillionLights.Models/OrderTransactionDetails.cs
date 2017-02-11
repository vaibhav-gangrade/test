using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class OrderTransactionDetails
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderTransactionDetailsId { get; set; }

        public Decimal OrderNumber { get; set; }
       
        public Decimal TransactionID { get; set; }
        public int OrderStatusID { get; set; }
        
    }
}