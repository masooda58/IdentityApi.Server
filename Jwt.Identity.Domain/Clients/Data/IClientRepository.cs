using System.Threading.Tasks;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Domain.Clients.Data
{
    public interface IClientRepository : IBaseGenricRepository<Client>
    {
      
    }
}