using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StorageAsp.Startup))]
namespace StorageAsp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
