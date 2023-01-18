using System;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Clients.Command;
using Jwt.Identity.Framework.Response;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Jwt.Identity.Domain.Shared;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.Clients.Command
{
    public class UpSertHandler : IRequestHandler<UpSert, ResultResponse>
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IEasyCachingProviderBase _cacheProvider;
     

        public UpSertHandler(UnitOfWork unitOfWork, IEasyCachingProviderBase cachingProvider, IEasyCachingProviderBase cacheProvider)
        {
            _unitOfWork = unitOfWork;
            _cacheProvider = cacheProvider;
        }


        public async Task<ResultResponse> Handle(UpSert request, CancellationToken cancellationToken)
        {
            var existUser = await _unitOfWork.ClientRepository.GetByIdNotraking(request.Client.ClientId);

            if (existUser != null)
            {

                _unitOfWork.ClientRepository.Update(request.Client);
                _unitOfWork.Save();
                _cacheProvider.Set(KeyRes.clients, _unitOfWork.ClientRepository.Get(), TimeSpan.FromDays(10));
               
                return new ResultResponse(true, $"Update client ={request.Client.ClientName}");
            }
            await _unitOfWork.ClientRepository.InsertAsync(request.Client);
            _unitOfWork.Save();
            _cacheProvider.Set(KeyRes.clients, _unitOfWork.ClientRepository.Get(), TimeSpan.FromDays(10));
           
            return new ResultResponse(true, $"Add client={request.Client.ClientName}", request.Client);
        }
    }
}
