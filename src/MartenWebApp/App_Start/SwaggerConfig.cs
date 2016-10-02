using System.Web.Http;
using Swashbuckle.Application;

namespace MartenWebApp
{
    public static class SwaggerConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "Marten App API"))
                .EnableSwaggerUi();

        }
    }
}