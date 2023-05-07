using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Plant_Paradise.Startup))]
namespace Plant_Paradise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
