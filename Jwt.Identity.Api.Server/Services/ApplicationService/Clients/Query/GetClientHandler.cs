using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.Clients.Query;
using MediatR;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.Clients.Query
{
    public class GetClientHandler:IRequestHandler<GetClient,Client>
    {
        private readonly UnitOfWork _unitOfWork;

        public GetClientHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Client> Handle(GetClient request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ClientRepository.GetByAsync(request.ClientName);
        }
    }
}
