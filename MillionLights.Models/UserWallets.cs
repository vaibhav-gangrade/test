using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class UserWallets
    {
        private MillionlightsContext db = new MillionlightsContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User Users { get; set; }
        public decimal? FinalAmountInWallet { get; set; }
        public bool IsActive { get; set; }
    }
}
