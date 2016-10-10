using System;

namespace MartenWebApp.Domain
{
    public class EmployeeCreated
    {
        public Guid Id { get; }
        public string Name { get; } 

        public EmployeeCreated(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}