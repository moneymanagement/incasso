using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Incasso.MultiTenancy.Dto;
using Abp.Dependency;
using System.Collections.Generic;
using incasso.Invoices.Dto;

namespace Incasso.Administrator
{
    public interface IInvoicesAppService : ITransientDependency
    {
        Task<InvoiceViewModel> GetGrid(CriteriaInvoiceSearch criteriaInvoiceSearch);
        Task<InvoiceDto> Create(CreateInvoiceDto input);
        Task<InvoiceDto> Update(EditInvoiceDto input);
        Task Delete(EntityDto<int> input);
        Task<InvoiceDto> Get(EntityDto<int> input);
        Task<InvoiceDto> ChangeStatus(InvoiceDto input); 
        Task AddNotes(AddNotesInputDto input);
        Task UpdateNotes(UpdateNotesInputDto input);
        Task DeleteNotes(UpdateNotesInputDto input);
    }
}
