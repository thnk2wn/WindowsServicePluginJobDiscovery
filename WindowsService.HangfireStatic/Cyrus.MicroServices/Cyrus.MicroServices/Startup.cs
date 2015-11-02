using System.Web.Http;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Cyrus.MicroServices.Startup))]

namespace Cyrus.MicroServices
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);

            // i.e. http://localhost:9000/hangfire
            app.UseHangfireDashboard();
           
        }
    }
}
