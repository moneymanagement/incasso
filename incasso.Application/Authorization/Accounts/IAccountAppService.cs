using System.Threading.Tasks;
using Abp.Application.Services;
using Incasso.Authorization.Accounts.Dto;

namespace Incasso.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
