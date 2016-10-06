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
        private readonly IUserFactory _userFactory;
        private readonly IDocumentStore _documentStore;

        public DocumentDbController(IUserFactory userFactory, IComponentContext componentContext)
        {
            _userFactory = userFactory;
            _documentStore = componentContext.ResolveNamed<IDocumentStore>("documentDatabaseDocumentStore");
        }


        [HttpGet]
        [Route("")]
        public async Task<IList<User>> Get()
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<User>().ToListAsync();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<User> Get(Guid id)
        {
            using (var session = _documentStore.LightweightSession())
            {
                return await session.Query<User>().SingleAsync(user => user.Id == id);
            }
        }

        [HttpPost]
        [Route("")]
        public void CreateUserFromModel(User user)
        {

            using (var session = _documentStore.LightweightSession())
            {
                session.Store(user);

                session.SaveChanges();
            }
        }

        [HttpPost]
        [Route("other")]
        public void CreateUserFromFactory()
        {

            using (var session = _documentStore.LightweightSession())
            {
                var user = _userFactory.Create("some first name", "some last name");

                session.Store(user);

                session.SaveChanges();
            }
        }

    }
}
