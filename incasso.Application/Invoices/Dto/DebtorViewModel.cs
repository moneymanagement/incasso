using Abp.Application.Services.Dto;
using incasso.Administrators.dto;
using incasso.Invoices;
using System.Collections.Generic;

namespace Incasso.MultiTenancy.Dto
{
    public class InvoiceViewModel
    {
        public int PageSize { get; internal set; }
        public int RequestedPage { get; internal set; }
        public PagedResultDto<InvoiceDto> Invoices { get;  set; }
        public IReadOnlyList<AdministratorDto>  Administrators{ get;  set; }
        public string Search { get; internal set; }
        public string InvoiceType { get; internal set; }
        public List<StatusCatalog> StatusCatalog { get; set; } = new List<StatusCatalog>();
    }
}