using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Marten;
using MartenWebApp.Domain;

namespace MartenWebApp
{
    public static class CommonDocumentStoreFactory
    {
        public static IDocumentStore CreateCommonStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["common"].ConnectionString;

            return DocumentStore.For(options =>
            {

                options.Connection(connectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Events.AddEventType(typeof(EmployeeCreated));
                options.Events.AddEventType(typeof(EmployeeNameChanged));
            });
        }
    }
}