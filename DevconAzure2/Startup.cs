using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DevconAzure2.Startup))]
namespace DevconAzure2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
