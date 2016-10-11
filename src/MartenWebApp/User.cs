using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marten.Schema;

namespace MartenWebApp
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //[ForeignKey(typeof(Company))]
        public Guid? CompanyId { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}