using System;
using Marten.Schema;
using MartenWebApp.Domain;

namespace MartenWebApp
{
    [PropertySearching(PropertySearching.ContainmentOperator)]
    public class EmployeeReadModel
    {
        public Guid Id { get; set; }


        [DuplicateField(PgType = "text")]
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