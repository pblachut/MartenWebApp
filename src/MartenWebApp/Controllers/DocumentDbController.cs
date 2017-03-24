using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;
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
        public async Task<UserWithCompany> Get(Guid id)
        {
            using (var session = _documentStore.LightweightSession())
            {
                Company company = null;

                var result = await session.Query<User>()
                    .Include<Company>(user => user.CompanyId2, comp => company = comp)
                    .SingleAsync(user => user.Id == id);

                return new UserWithCompany
                {
                    Company = company,
                    User = result
                };
            }
        }

        public class UserWithCompany
        {
            public User User { get; set; }
            public Company Company { get; set; }
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

        [HttpGet]
        [Route("batch/{id}")]
        public async Task<object> GetBatchUsers(Guid id)
        {
            using (var session = _documentStore.LightweightSession())
            {
                var batch = session.CreateBatchQuery();

                var userPromise = batch.Load<User>(id);

                var usersPromise = batch.Query<User>().Where(u => u.FirstName.StartsWith("Name")).ToList();

                await batch.Execute();

                var user = await userPromise;
                var users = await usersPromise;

                return new BatchUsers
                {
                    User = user,
                    Users = users.ToList()
                };
            }
        }

        public class BatchUsers
        {
            public User User { get; set; }
            public List<User> Users { get; set; }
        }


    }
}
