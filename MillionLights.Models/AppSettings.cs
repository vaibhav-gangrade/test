using Millionlights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class AppSettings
    {
        private MillionlightsContext db = new MillionlightsContext();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ApplicationId { get; set; }
        [DisplayName("AppName")]
        public string AppName { get; set; }

        [DisplayName("SecretKey")]
        public DateTime SecretKey { get; set; }

        [DisplayName("ReturnURL")]
        public DateTime ReturnURL { get; set; }

    }
}
