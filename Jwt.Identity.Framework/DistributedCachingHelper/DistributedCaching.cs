using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Jwt.Identity.Framework.DistributedCachingHelper
{
    public static class DistributedCaching  
    {  
        public static async Task SetObjAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))  
        {  
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);  
        }  
  
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class  
        {  
            var result = await distributedCache.GetAsync(key, token);  
            return result.FromByteArray<T>();  
        }  
    }  
}
