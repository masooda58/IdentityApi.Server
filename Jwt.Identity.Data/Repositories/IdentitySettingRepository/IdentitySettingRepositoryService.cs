using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.IdentityPolicy.Data;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Domain.Shared;
using Jwt.Identity.Framework.DistributedCachingHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Jwt.Identity.Data.Repositories.IdentitySettingRepository
{
    public class IdentitySettingRepositoryService:IIdentityPolicyRepository
    {
        private readonly IdentityContext _context;
       
       // private readonly  IMemoryCache _memoryCache;
       private readonly IEasyCachingProviderBase _cacheProvider;
        public IdentitySettingRepositoryService(IdentityContext context, 
            IEasyCachingProviderBase cacheProvider)
        {
            _context = context;

            _cacheProvider = cacheProvider;
            
        }

        public  IdentitySettingPolicy GetSetting()
        {
            // check memmory cache
          CacheValue<IdentitySettingPolicy> settingInMemmory;
          settingInMemmory = _cacheProvider.Get<IdentitySettingPolicy>(KeyRes.IdentitySetting);
          //if Memory cash empty
           if (settingInMemmory.HasValue)
           {
               return settingInMemmory.Value;
           }
            var settingInDb = _context.IdentitySettings.AsNoTracking().ToList();
            if (settingInDb.Count == 1)
            {
                _cacheProvider.Set(KeyRes.IdentitySetting, settingInDb[0],TimeSpan.FromDays(1));
                return settingInDb[0];
            }

            if (settingInDb.Count == 0)
            {
                _context.IdentitySettings.Add(new IdentitySettingPolicy());
                _context.SaveChanges();
                var defaultSetting = _context.IdentitySettings.AsNoTracking().ToList();
                _cacheProvider.Set(KeyRes.IdentitySetting, settingInDb[0],TimeSpan.FromDays(1));
                return defaultSetting[0];

            }
          

            throw new Exception("اطلاعات در دیتا بیس دچار مشکل است");

        }

        public void UpdateSetting(IdentitySettingPolicy setting)
        {
            bool isDetached = _context.Entry(setting).State == EntityState.Detached;
            if (isDetached)
              _context.Attach(setting);
            _context.Entry(setting).State = EntityState.Modified;
            // set memmory cache
           // _memoryCache.Set(CacheKey.IdentitySetting, setting);
            _cacheProvider.Set(KeyRes.IdentitySetting, setting,TimeSpan.FromDays(1));

        }

      

        public bool SettingExist(string id)
        {
            return _context.IdentitySettings.Any(i => i.Id == id);
        }
    }
}
