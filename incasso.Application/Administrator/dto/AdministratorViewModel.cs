using Abp.Application.Services.Dto;
using incasso.Administrators.dto;
using System.Collections.Generic;
using Incasso.Users.Dto;

namespace Incasso.MultiTenancy.Dto
{
    public class AdministratorViewModel
    {
        public string Search { get; set; }
        public int PageSize { get; internal set; }
        public int RequestedPage { get; internal set; }
        public PagedResultDto<AdministratorDto>  Administrators{ get;  set; }
        public IReadOnlyList<UserDto> Users { get; set; }
    }
}