using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;

namespace MartenWebApp
{
    public class MartenAppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<UserFactory>().As<IUserFactory>().InstancePerDependency();
        }
    }
}