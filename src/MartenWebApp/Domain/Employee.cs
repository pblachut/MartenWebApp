using System;
using System.Collections.Generic;

namespace MartenWebApp.Domain
{
    public class Employee : IAggregate
    {
        public static Employee Create(Guid id, string name)
        {
            var employee = new Employee();

            var @event = new EmployeeCreated(id, name);

            employee.EventsToStore.Add(@event);

            employee.Apply(@event);

            return employee;
        }


        public Guid Id { get; private set; }
        public List<object> EventsToStore { get; }
        public string Name { get; private set; }

        public Employee()
        {
            EventsToStore = new List<object>();
        }

        public void ChangeName(string name)
        {
            var @event = new EmployeeNameChanged(Id, name);

            EventsToStore.Add(@event);

            Apply(@event);
        }

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