using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MartenWebApp.Domain
{
    public class EmployeeNameChanged
    {
        public Guid Id { get; }
        public string Name { get; }

        public EmployeeNameChanged(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}