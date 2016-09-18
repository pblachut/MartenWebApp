using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MartenWebApp
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Internal { get; set; }
        public string UserName { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}