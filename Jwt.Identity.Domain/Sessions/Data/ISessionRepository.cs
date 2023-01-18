using System.Threading.Tasks;
using Jwt.Identity.Domain.Sessions.Entity;

namespace Jwt.Identity.Domain.Sessions.Data
{
    public interface ISessionRepository
    {
        Task AddSession(SessionEntity session);
    }
}
