using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IdentityPolicy.Command;
using Jwt.Identity.Domain.IdentityPolicy.Entity;

namespace Jwt.Identity.Domain.IdentityPolicy.Data
{
  public  interface IIdentityPolicyRepository
    {
        public IdentitySettingPolicy GetSetting();
        public void UpdateSetting(IdentitySettingPolicy setting);
        public bool SettingExist(string id);
    }
}
