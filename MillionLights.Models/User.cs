using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; } 
        public string UserType { get; set; }
        public string ProviderKey { get; set; }
        public bool IsActive { get; set; }
        public List<User> users { get; set; }
    }
}