using System.Configuration;
using Marten;

namespace MartenWebApp
{
    public static class EventStoreFactory
    {
        public static IDocumentStore CreateEventStore()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["postgresEventStore"].ConnectionString;

            return DocumentStore.For(options =>
            {

                options.Connection(connectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;
            });
        }
    }
}