using EasyCaching.Core;
using EasyCaching.Core.Bus;
using EasyCaching.Core.DistributedLock;
using EasyCaching.HybridCache;
using EasyCaching.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jwt.Identity.Api.Server.IOC.CustomCache
{
    public static class CustomCacheExtension
    {
        public static void AddCustomCacheConfig(this IServiceCollection services, IWebHostEnvironment env ,string nameOfInMemory,string nameOfHybridMemory)
        {
            if (env.IsDevelopment())
            {
                services.AddSingleton<IEasyCachingProviderBase, DefaultInMemoryCachingProvider>(x =>
                {
                    var mCache = x.GetServices<IInMemoryCaching>();
                    var optionsMon =
                        x.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<InMemoryOptions>>();
                    var options = optionsMon.Get(nameOfInMemory);
                    var dlf = x.GetService<IDistributedLockFactory>();
                    // ILoggerFactory can be null
                    var factory = x.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                    return new DefaultInMemoryCachingProvider(nameOfInMemory, mCache, options, dlf, factory);
                });
            }

            if (env.IsProduction())
            {
                services.AddSingleton<IEasyCachingProviderBase, HybridCachingProvider>(x =>
                {
                    var optionsMon =
                        x.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<HybridCachingOptions>>();
                    var options = optionsMon.Get(nameOfHybridMemory);

                    var providerFactory = x.GetService<IEasyCachingProviderFactory>();
                    var bus = x.GetService<IEasyCachingBus>();
                    var loggerFactory = x.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

                    return new HybridCachingProvider(nameOfHybridMemory, options, providerFactory, bus, loggerFactory);
                });
            }
        }

   
    }
}
