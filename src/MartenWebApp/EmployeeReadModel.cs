using System;
using MartenWebApp.Domain;

namespace MartenWebApp
{
    public class EmployeeReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public void Apply(EmployeeNameChanged @event)
        {
            Name = @event.Name;

        }
        
        public void Apply(EmployeeCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;

        }

        
    }
}