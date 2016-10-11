using System;

namespace MartenWebApp
{
    public class Company
    {
        public Company()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}