using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
    public class Career
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("Experience Required")]
        public string Experience { get; set; }

        [DisplayName("Qualification")]
        public string Qualification { get; set; }

        [DisplayName("Job Description")]
        public string JobDescription { get; set; }

        [DisplayName("Technical Skills")]
        public string TechnicalSkills { get; set; }

        [DisplayName("Managerial / Business Skills")]
        public string BusinessSkills { get; set; }

        [DisplayName("Responsibilities")]
        public string Responsibilities { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

