using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using Marten;
using Marten.Events;
using MartenWebApp.Domain;

namespace MartenWebApp.Controllers
{
    [RoutePrefix("api/queries")]
    public class QueryController : ApiController
    {
        private IDocumentStore _documentStore;

        public QueryController(
            IComponentContext componentContext)
        {
            _documentStore = componentContext.ResolveNamed<IDocumentStore>("commonDocumentStore");
        }

        [HttpGet]
        [Route("emplyees/{id}/rawEvents")]
        public async Task<IList<IEvent>> GetRawEvents(Guid id)
        {
            using (var session = _documentStore.OpenSession())
            {
                return await session.Events.FetchStreamAsync(id);
            }
        } 
    }
}