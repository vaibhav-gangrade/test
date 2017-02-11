using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MandrillEmailService
{
    [DataContract]
    public class TemplateContent
    {
        //NOTE: the data members are case-sensitive
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string content { get; set; }
    }
}
