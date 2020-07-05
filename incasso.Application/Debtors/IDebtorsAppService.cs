using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Incasso.MultiTenancy.Dto;
using Abp.Dependency;

namespace Incasso.Administrator
{
    public interface IDebtorsAppService : ITransientDependency
    {
        Task<DebtorViewModel> GetGrid(CriteriaDebtorSearch criteriaDebtorSearch);
        Task<DebtorDto> Create(CreateDebtorDto input);
        Task<DebtorDto> Update(EditDebtorDto input);
        Task Delete(EntityDto<int> input);
        Task<DebtorDto> Get(EntityDto<int> input);
        Task<DebtorDto> ChangeStatus(DebtorDto input);
        Task SaveNotes(DebtorDto input);
    }
}
