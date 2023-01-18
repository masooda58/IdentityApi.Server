using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;

namespace Jwt.Identity.Domain.User.Entities
{
    public class ApplicationUserPolicy
    {
        public ApplicationUserPolicy(string userId, string policyId)
        {
            UserId = userId;
            PolicyId = policyId;
        }
       
        public string  PolicyId { get; private set; }
 
        public string UserId { get; private set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual UserLoginPolicyOptions UserLoginPolicyOption { get; set; }

    }
}
