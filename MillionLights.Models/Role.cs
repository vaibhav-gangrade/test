using System.ComponentModel.DataAnnotations.Schema;

namespace Millionlights.Models
{
    public class Role
    {
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        
    }
}