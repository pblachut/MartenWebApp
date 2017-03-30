using System;
using System.Configuration;
using Marten;

namespace MartenWebApp
{
    public static class DocumentDatabaseStoreFactory
    {
        public static IDocumentStore CreateDocumentStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["postgresDocumentDatabase"].ConnectionString;

            return DocumentStore.For(options =>
            {
                options.Logger(new WebConsoleMartenLogger());
                options.Connection(connectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;

                options.Schema.For<User>().ForeignKey<Company>(x => x.CompanyId);
                options.Schema.For<User>().ForeignKey<Company>(x => x.CompanyId2);

                options.Schema.For<GeneralUser>()
                    .AddSubClass<Employee>()
                    .AddSubClass(typeof(Administrator));

                options.Schema.For<IVehicle>()
                    .AddSubClassHierarchy(
                        typeof(Car),
                        typeof(Toyota)
                    );

            });
        }
    }

    public class GeneralUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class Employee : GeneralUser { }
    public class Administrator : GeneralUser { }

    public interface IVehicle
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }

    public class Car : IVehicle {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class Toyota : Car { }

}