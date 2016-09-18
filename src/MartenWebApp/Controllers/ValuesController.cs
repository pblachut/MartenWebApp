using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Marten;

namespace MartenWebApp.Controllers
{
    public class ValuesController : ApiController
    {

        private IDocumentStore GetDocumentStore()
        {
            return DocumentStore.For(options =>
            {
                options.Connection("host=localhost;database=marten_test;password=admin;username=postgres");
                options.AutoCreateSchemaObjects = AutoCreate.All;
            });
        }

        // GET api/values
        public async Task<IList<User>> Get()
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                return await session.Query<User>().ToListAsync();
            }
        }

        // GET api/values/5
        public async Task<User> Get(Guid id)
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                return await session.Query<User>().SingleAsync(user => user.Id == id);
            }
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            var store = GetDocumentStore();

            using (var session = store.LightweightSession())
            {
                var user = new User { FirstName = "Han", LastName = "Solo" };
                session.Store(user);

                session.SaveChanges();
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
