using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using Marten;
using Marten.Events;
using Marten.Linq;
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

        [HttpGet]
        [Route("emplyees/{id}")]
        public async Task<EmployeeReadModel> GetEmployeeReadModel(Guid id)
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<EmployeeReadModel>().SingleAsync(e => e.Id == id);
            }
        }

        [HttpGet]
        [Route("emplyees")]
        public async Task<EmployeeListResponse> GetEmployees()
        {
            using (var session = _documentStore.LightweightSession())
            {
                QueryStatistics statistics;

                var result = await session.Query<EmployeeReadModel>().Stats(out statistics).ToListAsync();

                return new EmployeeListResponse
                {
                    Items = result,
                    TotalCount = statistics.TotalResults
                };

            }
        }

        public class EmployeeListResponse
        {
            public IList<EmployeeReadModel> Items { get; set; }
            public long TotalCount { get; set; }
        }
    }
}