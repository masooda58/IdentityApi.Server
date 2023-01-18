using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.Repositories.ClientRepository;
using Jwt.Identity.Data.Repositories.IdentitySettingRepository;
using Microsoft.Extensions.Caching.Memory;
using System;
using EasyCaching.Core;
using Jwt.Identity.Data.Repositories.UserLoginOptionRepository;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Jwt.Identity.Data.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private readonly IdentityContext _context;
        private ClientRepositoryService _client;
        private IdentitySettingRepositoryService _identitySetting;
        private UserPolicyManagementService _userLoginPolicy;
        private UserLoginOptionRepositoryServices _loginOption;
        private readonly  IEasyCachingProviderBase _cache;

        public UnitOfWork(IdentityContext context,  IEasyCachingProviderBase cache)
        {
            _context = context;
            _cache = cache;
        }

        public ClientRepositoryService ClientRepository
        {
            get
            {
                if (_client == null)
                {
                    _client = new ClientRepositoryService(_context,_cache);
                }

                return _client;
            }
        }
        public UserPolicyManagementService UserLoginPolicyRepository
        {
            get
            {
                if (_userLoginPolicy == null)
                {
                    _userLoginPolicy = new UserPolicyManagementService(_context);
                }

                return _userLoginPolicy;
            }
        }
        public UserLoginOptionRepositoryServices LoginOptionRepository
        {
            get
            {
                if (_loginOption == null)
                {
                    _loginOption = new UserLoginOptionRepositoryServices(_context);
                }

                return _loginOption;
            }
        }
        public IdentitySettingRepositoryService IdentitySettingPolicy
        {
            get
            {
                return _identitySetting ??= new IdentitySettingRepositoryService(_context, _cache);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
