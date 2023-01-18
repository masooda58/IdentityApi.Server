using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jwt.Identity.Domain.Sessions.Entity;
using Jwt.Identity.Domain.User.Enum;

namespace Jwt.Identity.Domain.User.Entities
{
    // store in db for login an logout history
    public class UserLogInOutLog
    {
        [Key] public Guid IdGuid { get; set; }

        [ForeignKey("Id")] public string UserId { get; set; }
        public SessionEntity Session { get; set; }

        public SignInLogerType SignInOut { get; set; }
        public DateTime Time { get; set; }

     
    }
}