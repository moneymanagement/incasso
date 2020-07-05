using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Incasso.MultiTenancy.Dto;

namespace Incasso.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
