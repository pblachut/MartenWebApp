using System;

namespace MartenWebApp
{
    public interface IUserFactory
    {
        User Create(string firstName, string lastName);
    }

    public class UserFactory : IUserFactory
    {
        public User Create(string firstName, string lastName)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
}