using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.Identity.Domain.Sessions.Entity
{
    public class SessionEntity
    {
        [Key] public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string IpAddress { get; set; }
        public string  DeviceName { get; set; }
        public string BrowserName { get; set; }
        public DateTime StartSessionTime =DateTime.Now;
    }
}
