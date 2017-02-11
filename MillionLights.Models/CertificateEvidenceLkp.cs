using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class CertificateEvidenceLkp
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Evidence Name")]
        public string EvidenceName { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
    }
}
