using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using Marten;
using Marten.Events;

namespace MartenWebApp.Controllers
{
    [RoutePrefix("api/events")]
    public class EventStoreController : ApiController
    {
        private IDocumentStore _eventStore;

        public EventStoreController(IComponentContext componentContext)
        {
            _eventStore = componentContext.ResolveNamed<IDocumentStore>("eventStoreDocumentStore");
        }

        [Route("streams/{id}")]
        [HttpGet]
        public async Task<IList<IEvent>> GetStream(Guid id)
        {
            using (var session = _eventStore.OpenSession())
            {
                return await session.Events.FetchStreamAsync(id);
            }
        }


        [Route("streams")]
        [HttpPost]
        public void CreateStream(Guid id)
        {
            using (var session = _eventStore.OpenSession())
            {
                session.Events.StartStream<SomeClass>(id);

                session.SaveChanges();
            }
        }

        [Route("streams/{id}/events")]
        [HttpPost]
        public void AddEventToStream(Guid id, SomeEvent1 @event)
        {
            using (var session = _eventStore.OpenSession())
            {
                session.Events.Append(id, @event);

                session.SaveChanges();
            }
        }

        public class SomeClass
        {
            
        }

        public class SomeEvent1
        {
            public string Name { get; set; }
        }

    }
}