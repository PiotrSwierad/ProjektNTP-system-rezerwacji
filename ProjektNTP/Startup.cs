using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjektNTP.Startup))]
namespace ProjektNTP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
