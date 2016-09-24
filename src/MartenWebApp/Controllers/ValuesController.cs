using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Marten;

namespace MartenWebApp.Controllers
{
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        private readonly IUserFactory _userFactory;

        public ValuesController(IUserFactory userFactory)
        {
            _userFactory = userFactory;
        }


        private IDocumentStore GetDocumentStore()
        {
            return DocumentStore.For(options =>
            {
                options.Connection("host=localhost;database=marten_test;password=admin;username=postgres");
                options.AutoCreateSchemaObjects = AutoCreate.All;
            });
        }

        [HttpGet]
        [Route("")]
        public async Task<IList<User>> Get()
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                return await session.Query<User>().ToListAsync();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<User> Get(Guid id)
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                return await session.Query<User>().SingleAsync(user => user.Id == id);
            }
        }

        [HttpPost]
        [Route("")]
        public void CreateUserFromModel(User user)
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                session.Store(user);

                session.SaveChanges();
            }
        }

        [HttpPost]
        [Route("other")]
        public void CreateUserFromFactory()
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                var user = _userFactory.Create("some first name", "some last name");

                session.Store(user);

                session.SaveChanges();
            }
        }

    }
}
