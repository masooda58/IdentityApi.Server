using System;
using System.Linq;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.IntialData;
using Jwt.Identity.Domain.IdentityPolicy.Entity;

namespace Jwt.Identity.Data.InitialData
{
   public class IdentityDbContextSeed
    {
        public static void SeedData(IdentityContext context)
        {
            SeedInitialIdentitySetting(context);
            SeedClientInitial(context);
        }

        public static void SeedInitialIdentitySetting(IdentityContext context)
        {
            try
            {
                var settingExist = context.IdentitySettings.Any();
                if (!settingExist)
                {
                    context.IdentitySettings.Add(new IdentitySettingPolicy());
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("خطای دیتای اولیه");
            }
        }
        public static void SeedClientInitial(IdentityContext context)
        {
            try
            {
                var settingExist = context.Clients.Any();
                if (!settingExist)
                {
                    context.Clients.AddRange(InitialClients.GetClients());
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("خطای دیتای اولیه");
            }
        }
    }
}
