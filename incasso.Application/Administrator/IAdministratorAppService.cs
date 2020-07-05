using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using incasso.Administrators.dto;
using Incasso.Administrators;
using Incasso.Administrators.dto;
using Incasso.MultiTenancy.Dto;

namespace Incasso.Administrator
{
    public interface IAdministrationsAppService : IApplicationService //IAsyncCrudAppService<AdministratorDto, int, PagedResultRequestDto, CreateAdministratorInput, UpdateAdministratorDto>
    {
        Task<AdministratorViewModel> GetGrid(CriteriaAdministratorSearch criteriaAdministratorSearch);
        Task<PagedResultDto<AdministratorDto>> GetAll(PagedResultRequestDto pagedResultRequestDto);
        Task Delete(EntityDto<int> input);
        Task<AdministratorDto> Get(EntityDto<int> input);
        Task<AdministratorDto> Create(CreateAdministratorInput input);
        Task<AdministratorDto> Update(UpdateAdministratorDto input);
    }
}
