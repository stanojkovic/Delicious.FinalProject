using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Delicious.Startup))]
namespace Delicious
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
