using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Identity.Data.InitialData
{
    public static  class InitialDb
    {
   
        public static void InstallationDb(IdentityContext _context)
        {
          
                var databased= _context.Database.EnsureCreated();
                IdentityDbContextSeed.SeedData(_context);
              
         
         
        }

    }
}
