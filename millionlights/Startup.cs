using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Millionlights.Startup))]
namespace Millionlights
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
