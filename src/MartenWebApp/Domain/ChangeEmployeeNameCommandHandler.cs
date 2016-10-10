using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MartenWebApp.Domain
{
    public class ChangeEmployeeNameCommandHandler
    {
        private readonly Repository<Employee> _repository;

        public ChangeEmployeeNameCommandHandler(Repository<Employee> repository)
        {
            _repository = repository;
        }

        public async Task Handle(ChangeEmployeeNameCommand command)
        {
            var employee = await _repository.Get(command.Id);

            employee.ChangeName(command.Name);

            _repository.Update(employee);
        }
    }
}