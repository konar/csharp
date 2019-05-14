using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GoPangService.Startup))]

namespace GoPangService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}