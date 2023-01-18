using System.Collections;
using System.Collections.Generic;

namespace Jwt.Identity.Test.Helper
{
    internal class RoleDataForXuint : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<string>
                {
                    //new IdentityRole
                    "Admin",
                    // new IdentityRole
                    "RegularUser"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}