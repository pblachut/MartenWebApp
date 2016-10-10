using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Marten;
using MartenWebApp.Domain;

namespace MartenWebApp
{
    public class MartenAppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<UserFactory>().As<IUserFactory>().InstancePerDependency();

            builder.Register(c => DocumentDatabaseStoreFactory.CreateDocumentStore()).Named<IDocumentStore>("documentDatabaseDocumentStore").SingleInstance();
            builder.Register(c => EventStoreFactory.CreateEventStore()).Named<IDocumentStore>("eventStoreDocumentStore").SingleInstance();
            builder.Register(c => CommonDocumentStoreFactory.CreateCommonStore()).Named<IDocumentStore>("commonDocumentStore").SingleInstance();

            builder.RegisterType<CreateEmployeeCommandHandler>().AsSelf().InstancePerDependency();
            builder.RegisterType<ChangeEmployeeNameCommandHandler>().AsSelf().InstancePerDependency();

            builder.RegisterGeneric(typeof(Repository<>)).AsSelf().InstancePerRequest();
        }
    }
}