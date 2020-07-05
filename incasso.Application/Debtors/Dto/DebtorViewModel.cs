using Abp.Application.Services.Dto;
using incasso.Administrators.dto;
using System.Collections.Generic;

namespace Incasso.MultiTenancy.Dto
{
    public class DebtorViewModel
    {
        public int PageSize { get; internal set; }
        public int RequestedPage { get; internal set; }
        public PagedResultDto<DebtorDto> Debtors { get;  set; }
        public IReadOnlyList<AdministratorDto>  Administrators{ get;  set; }
        public string Search { get; internal set; }
    }
}