using System.Collections;
using System.Collections.Generic;
using Jwt.Identity.Domain.User.Entities;

namespace Jwt.Identity.Test.Helper
{
    /// <summary>
    ///     // Theory در بخش ClassData
    /// </summary>
    public class UserDataForXuit : IEnumerable<object[]>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserDataForXuit.GetEnumerator()'
        public IEnumerator<object[]> GetEnumerator()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserDataForXuit.GetEnumerator()'
        {
            yield return new object[]
            {
                new ApplicationUser
                {
                    FirstName = "aFirstName",
                    LastName = "bLastName",
                    Email = "c@b.cEmail",
                    UserName = "d@b.cUserName",
                    Approved = true
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}