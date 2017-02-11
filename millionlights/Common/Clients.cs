using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Millionlights.Common
{
    public static class Clients
    {
        public readonly static Client Client1 = new Client
        {
            Id = WebConfigurationManager.AppSettings["Client1AppId"],
            Secret = WebConfigurationManager.AppSettings["Client1Secret"],
            RedirectUrl = WebConfigurationManager.AppSettings["Client1RedirectUrl"]
        };

        public readonly static Client Client2 = new Client
        {
            Id = WebConfigurationManager.AppSettings["Client2AppId"],
            Secret = WebConfigurationManager.AppSettings["Client2Secret"],
            RedirectUrl = WebConfigurationManager.AppSettings["Client2RedirectUrl"]
        };

        public readonly static Client Client3 = new Client
        {
            Id = WebConfigurationManager.AppSettings["Client3AppId"],
            Secret = WebConfigurationManager.AppSettings["Client3Secret"],
            RedirectUrl = WebConfigurationManager.AppSettings["Client3RedirectUrl"]
        };

        public readonly static Client Client4 = new Client
        {
            Id = WebConfigurationManager.AppSettings["Client4AppId"],
            Secret = WebConfigurationManager.AppSettings["Client4Secret"],
            RedirectUrl = WebConfigurationManager.AppSettings["Client4RedirectUrl"]
        };
    }

    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string RedirectUrl { get; set; }
    }
}