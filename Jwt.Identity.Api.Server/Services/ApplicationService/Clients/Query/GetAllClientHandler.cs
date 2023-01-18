using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.Clients.Query;
using MediatR;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.Clients.Query
{
    public class GetAllClientHandler : IRequestHandler<GetAllClient, IEnumerable<Client>>
    {
        private readonly UnitOfWork _unitOfWork;
     

        public GetAllClientHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }

        public async Task<IEnumerable<Client>> Handle(GetAllClient request, CancellationToken cancellationToken)
        {
       

            return _unitOfWork.ClientRepository.Get();
        }
    }
}
