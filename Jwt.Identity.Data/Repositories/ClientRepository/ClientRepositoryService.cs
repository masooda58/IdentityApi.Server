using EasyCaching.Core;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.Repositories.BaseRepository;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Identity.Data.Repositories.ClientRepository
{
    public class ClientRepositoryService : BaseGenricRepository<Client>
    {
        private readonly IdentityContext _context;
        private readonly IEasyCachingProviderBase _cacheProvider;


        public ClientRepositoryService(IdentityContext context, IEasyCachingProviderBase cacheProvider) : base(context)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public override async Task<Client> GetByAsync(object clientName)
        {
            CacheValue<List<Client>> cliensInCache;
            cliensInCache = _cacheProvider.Get<List<Client>>(KeyRes.clients);

            if (cliensInCache.HasValue)
            {
                var clienInCache = cliensInCache.Value.FirstOrDefault(x => x.ClientName == clientName.ToString().ToUpper());
                if (clienInCache != null)
                    return clienInCache;
            }


            var client = _context.Clients.FirstOrDefault(x => x.ClientName == clientName.ToString().ToUpper());
            if (client != null)
            {
                _cacheProvider.Set(KeyRes.clients, await _context.Clients.ToListAsync(), TimeSpan.FromDays(10));
                return client;
            }

            return null;


        }

        public override async Task<Client> GetByIdNotraking(object id)
        {
            CacheValue<List<Client>> cliensInCache;
            cliensInCache = _cacheProvider.Get<List<Client>>(KeyRes.clients);
            if (cliensInCache.HasValue)
            {
                var clienInCache = cliensInCache.Value.FirstOrDefault(x => x.ClientId == (int)id);
                if (clienInCache != null)
                    return clienInCache;
            }

            var client = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == (int)id);
            if (client!=null)
            {
                _cacheProvider.Set(KeyRes.clients, await _context.Clients.ToListAsync(),TimeSpan.FromDays(10));
                return client;
            }

            return null;
        }
        public async Task<Client> GetByBaseUrlNotraking(string url)
        {
            var cliensInCache = _cacheProvider.Get<List<Client>>(KeyRes.clients);
            if (cliensInCache.HasValue)
            {
                var clienInCache = cliensInCache.Value.FirstOrDefault(x =>  x.BaseUrl.Contains(url));
                if (clienInCache != null)
                    return clienInCache;
            }
            var client= await _context.Clients.AsNoTracking().SingleOrDefaultAsync(c => c.BaseUrl.Contains(url));
            if (client != null)
            {
                _cacheProvider.Set(KeyRes.clients, await _context.Clients.ToListAsync(),TimeSpan.FromDays(10));
                return client;
               
            }

            return null;
        }

        //public override async Task DeleteAsync(Client entityToDelete)
        //{
        //    await base.DeleteAsync(entityToDelete);
        //  //  await _cacheProvider.SetAsync(KeyRes.clients, await _context.Clients.ToListAsync(),TimeSpan.FromDays(10));

        //}

        //public override async Task InsertAsync(Client entity)
        //{
        //    await base.InsertAsync(entity);
        //    await _cacheProvider.SetAsync(KeyRes.clients, await _context.Clients.ToListAsync(),TimeSpan.FromDays(10));
        //}
    }
}

//public ClientRepositoryService(IdentityContext context)
//      {
//          _context = context;

//      }

//      public async Task<IEnumerable<Client>> GetAllAsync()
//      {
//          try
//          {
//              return await _context.Clients.ToListAsync();
//          }
//          catch (Exception e)
//          {
//              throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
//          }


//      }

//      public async Task<ResultResponse> Add(Client client)
//      {
//          try
//          {
//              var result = await _context.Clients.AddAsync(client);
//              await _context.SaveChangesAsync();
//              return new ResultResponse(true, "کلاینت با موفقیت اضافه گردید", client);
//          }
//          catch (Exception e)
//          {
//              throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);

//          }

//      }

//      public async Task<ResultResponse> Update(Client client)
//      {
//          try
//          {
//              var clientExist = await _context.Clients.FirstOrDefaultAsync(x => x.ClientName == client.ClientName.ToUpper());
//              if (clientExist != null)
//              {
//                  clientExist = client;
//                  await _context.SaveChangesAsync();
//                  return new ResultResponse(true, "کلاینت آپدیت گردید", client);
//              }
//              return new ResultResponse(false, "کلاینت وجود ندارد", client);
//          }
//          catch (Exception e)
//          {
//              throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
//          }
//      }


//public async Task<ResultResponse> Delete(Client client)
//{
//    try
//    {
//        _context.Clients.Remove(client);
//        await _context.SaveChangesAsync();
//        return new ResultResponse(true, "کلاینت با موفقیت حذف گردید", client);
//    }
//    catch (Exception e)
//    {
//        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
//    }
//}

//public Task<Client> FindByConditionAsync(Expression<Func<Client, bool>> predicate)
//{
//    throw new NotImplementedException();
//}

//public async Task<Client> GetByClientNameAsync(string clientName)
//{
//    try
//    {
//        var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientName == clientName.ToUpper());
//        return client;
//    }
//    catch (Exception e)
//    {


//        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
//        //Exception ex = new Exception(result.Message.ToString());
//        //ex.Data.Add("cus",JsonConvert.SerializeObject(ee));
//        //throw result;
//    }



//}

//public async Task<ResultResponse> DeleteClientNameAsync(string clientName)
//{
//    try
//    {
//        var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientName == clientName.ToUpper());
//        if (client != null)
//        {
//            return await Delete(client);
//        }

//        throw new Exception("کلاینت وجود ندارد");
//    }
//    catch (Exception e)
//    {


//        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
//        //Exception ex = new Exception(result.Message.ToString());
//        //ex.Data.Add("cus",JsonConvert.SerializeObject(ee));
//        //throw result;
//    }
//}