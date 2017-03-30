using System;
using System.Diagnostics;
using System.Linq;
using Marten;
using Marten.Services;
using Npgsql;

namespace MartenWebApp
{
    public class WebConsoleMartenLogger : IMartenLogger, IMartenSessionLogger
    {
        public IMartenSessionLogger StartSession(IQuerySession session)
        {
            return (IMartenSessionLogger)this;
        }

        public void SchemaChange(string sql)
        {
            Debug.WriteLine("Executing DDL change:");
            Debug.WriteLine(sql);
            Debug.WriteLine(" ");
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            Debug.WriteLine(command.CommandText);
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            Debug.WriteLine("Postgresql command failed!");
            Debug.WriteLine(command.CommandText);
            Debug.WriteLine((object)ex);
        }

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
            IChangeSet changeSet = commit;
            Debug.WriteLine(string.Format("Persisted {0} updates, {1} inserts, and {2} deletions", (object)changeSet.Updated.Count<object>(), (object)changeSet.Inserted.Count<object>(), (object)changeSet.Deleted.Count<IDeletion>()));
        }
    }
}