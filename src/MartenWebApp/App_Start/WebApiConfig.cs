using System.Web.Http;
using Owin;

namespace MartenWebApp
{
    public static class WebApiConfig
    {
        public static void Configure(IAppBuilder appBuilder, HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}
