using System.Collections.Generic;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.User.Enum;

namespace Jwt.Identity.Data.IntialData
{
    public static class InitialClients
    {
        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                new()
                {
                    ClientName = "Identity",
                    BaseUrl = "http://localhost:44379/",
                    EmailConfirmPage = "/ConfirmationEmail",
                    EmailResetPage = "/ResetPassword",
                    LoginType = LoginType.TokenAndCookie,
                    LoginUrl = "/Login",
                    SignInExternal = "/External-SIGN",
                    SignOut = "/SignOut",
                    Lockout = "/LouckOut"
                }
            };
        }
    }
}