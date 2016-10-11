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

                options.Connection(connectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;

                options.Schema.For<User>().ForeignKey<Company>(x => x.CompanyId);
            });
        }
    }
}