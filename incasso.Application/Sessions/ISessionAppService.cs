using System.Threading.Tasks;
using Abp.Application.Services;
using Incasso.Sessions.Dto;

namespace Incasso.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
