using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CassandraWebTest.Startup))]
namespace CassandraWebTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
