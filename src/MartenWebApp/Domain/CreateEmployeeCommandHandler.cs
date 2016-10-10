using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MartenWebApp.Domain
{
    public class CreateEmployeeCommandHandler
    {
        private readonly Repository<Employee> _repository;

        public CreateEmployeeCommandHandler(Repository<Employee> repository)
        {
            _repository = repository;
        }

        public Task Handle(CreateEmployeeCommand command)
        {
            var employee = Employee.Create(command.Id, command.Name);

            return _repository.Add(employee);
        }
    }
}