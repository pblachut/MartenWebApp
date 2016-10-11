using System;

namespace MartenWebApp
{
    public interface IUserFactory
    {
        User Create(string firstName, string lastName, Guid? companyId);
    }

    public class UserFactory : IUserFactory
    {
        public User Create(string firstName, string lastName, Guid? companyId)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
}