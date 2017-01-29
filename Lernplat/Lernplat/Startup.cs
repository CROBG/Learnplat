using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lernplat.Startup))]
namespace Lernplat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
