using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DynamicMVC.Startup))]
namespace DynamicMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
