using System;

namespace MartenWebApp.Domain
{
    public class ChangeEmployeeNameCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}