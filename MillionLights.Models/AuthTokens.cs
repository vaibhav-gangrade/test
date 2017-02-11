using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Millionlights.Models;
using System.ComponentModel;
using System.Web.Mvc;


namespace Millionlights.Models
{
    public class AuthTokens
    {
        private MillionlightsContext db = new MillionlightsContext();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int TokenId { get; set; }
        [DisplayName("Token")]
        public string Token { get; set; }

        [DisplayName("Issued On")]
        public DateTime IssuedOn { get; set; }

        [DisplayName("Expires On")]
        public DateTime ExpiresOn { get; set; }

        [DisplayName("UserId")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
