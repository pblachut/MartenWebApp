using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Marten;

namespace MartenWebApp
{
    public class Repository<TAggregate> where TAggregate : class, IAggregate, new()
    {
        private IDocumentStore _documentStore;

        public Repository(IComponentContext componentContext)
        {
            _documentStore = componentContext.ResolveNamed<IDocumentStore>("commonDocumentStore");
        }


        public async Task<TAggregate> Get(Guid id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var entity = await session.Events.AggregateStreamAsync<TAggregate>(id);

                return entity;
            }
        }

        public async Task Add(TAggregate entity)
        {
            using (var session = _documentStore.OpenSession())
            {
                var id = session.Events.StartStream<TAggregate>(entity.Id, entity.EventsToStore.ToArray());

                await session.SaveChangesAsync();
            }
        }

        public void Update(TAggregate entity)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Events.Append(entity.Id, entity.EventsToStore.ToArray());

                session.SaveChanges();
            }
        }
    }

    public interface IAggregate
    {
        Guid Id { get; }
        List<object> EventsToStore { get; }
    }
}