using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Marten;

namespace MartenWebApp.Controllers
{
    [RoutePrefix("api/documents")]
    public class DocumentDbController : ApiController
    {
        private readonly IDocumentStore _documentStore;

        public DocumentDbController(IComponentContext componentContext)
        {
            _documentStore = componentContext.ResolveNamed<IDocumentStore>("documentDatabaseDocumentStore");
        }


        [HttpGet]
        [Route("users")]
        public async Task<IList<User>> Get()
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<User>().ToListAsync();
            }
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<User> Get(Guid id)
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<User>().SingleAsync(user => user.Id == id);
            }
        }

        [HttpPost]
        [Route("users")]
        public void CreateUserFromModel(User user)
        {
            using (var session = _documentStore.LightweightSession())
            {
                session.Store(user);

                session.SaveChanges();
            }
        }

        [HttpPost]
        [Route("companies")]
        public Guid CreateCompany(string name)
        {
            var company = new Company
            {
                Name = name
            };

            using (var session = _documentStore.LightweightSession())
            {
                session.Store(company);

                session.SaveChanges();
            }

            return company.Id;
        }

        [HttpGet]
        [Route("companies")]
        public async Task<IList<Company>> GetComapnies()
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<Company>().ToListAsync();
            }
        }


    }
}
