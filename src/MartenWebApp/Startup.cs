using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Owin;

namespace MartenWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            AutofacConfig.Configure(appBuilder, config);

            WebApiConfig.Configure(appBuilder, config);
        }

    }
}