using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class PasswordResetRequest
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User user { get; set; }
        public string VerificationId { get; set; }
        public bool IsPasswordReset { get; set; }
        public DateTime PasswordResetRequestDateTime { get; set; }
        public DateTime? PasswordResetDateTime { get; set; }
    }
}
