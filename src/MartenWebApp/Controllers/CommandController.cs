using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MartenWebApp.Domain;

namespace MartenWebApp.Controllers
{
    [RoutePrefix("api/commands")]
    public class CommandController : ApiController
    {
        private readonly CreateEmployeeCommandHandler _createEmployeeCommandHandler;
        private readonly ChangeEmployeeNameCommandHandler _changeEmployeeNameCommandHandler;

        public CommandController(
            CreateEmployeeCommandHandler createEmployeeCommandHandler,
            ChangeEmployeeNameCommandHandler changeEmployeeNameCommandHandler)
        {
            _createEmployeeCommandHandler = createEmployeeCommandHandler;
            _changeEmployeeNameCommandHandler = changeEmployeeNameCommandHandler;
        }

        [HttpPost]
        [Route("employees")]
        public async Task<Guid> CreateEmployee(string name)
        {
            var command = new CreateEmployeeCommand
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            await _createEmployeeCommandHandler.Handle(command);

            return command.Id;
        }

        [HttpPut]
        [Route("employees/{id}")]
        public async Task ChangeEmployeeName(Guid id, string name)
        {
            var command = new ChangeEmployeeNameCommand()
            {
                Id = id,
                Name = name
            };

            await _changeEmployeeNameCommandHandler.Handle(command);
        }
    }
}