using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UGPC_IIUI.Startup))]
namespace UGPC_IIUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
