using System;

namespace Jwt.Identity.Domain.User.Entities
{
    // store in cache for check number of login and limit it
    public class CurrentUserLogin
    {
        public string UserId { get; set; }
        public string KeyGuid { get;  } = Guid.NewGuid().ToString();
        public string SessionId { get; set; }
        public string AccessToken { get; set; }

    }
}