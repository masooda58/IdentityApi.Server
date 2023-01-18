using System;

namespace Jwt.Identity.Domain.Token.Entitis
{
    public class RefreshToken
    {
        public string UserId { get; set; }
        public string TempRefreshToken { get; set; }
        public DateTime CreatTime { get; set; } = DateTime.Now;
    }
}