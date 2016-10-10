using System;

namespace MartenWebApp.Domain
{
    public class CreateEmployeeCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}