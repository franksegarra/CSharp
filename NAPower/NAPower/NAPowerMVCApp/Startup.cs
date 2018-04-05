using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NAPowerMVCApp.Startup))]

namespace NAPowerMVCApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
