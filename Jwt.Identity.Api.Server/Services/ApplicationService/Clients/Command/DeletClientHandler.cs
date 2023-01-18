using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Clients.Command;
using Jwt.Identity.Domain.Shared;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.Clients.Command
{
    public class DeletClientHandler:IRequestHandler<DeletClient,bool>
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IEasyCachingProviderBase _cacheProvider;

        public DeletClientHandler(UnitOfWork unitOfWork, IEasyCachingProviderBase cacheProvider)
        {
            _unitOfWork = unitOfWork;
            _cacheProvider = cacheProvider;
        }

        public async Task<bool> Handle(DeletClient request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ClientRepository.DeleteAsync(request.ClientName);
            _unitOfWork.Save();
             _cacheProvider.Set(KeyRes.clients, _unitOfWork.ClientRepository.Get(), TimeSpan.FromDays(10));
            return true;

        }
    }
}
