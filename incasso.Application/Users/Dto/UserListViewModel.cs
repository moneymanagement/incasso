using Abp.Application.Services.Dto;
using Incasso.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using incasso.Administrators.dto;

namespace Incasso.Users.Dto
{
    public class UserListViewModel
    {
        public string  Search { get; set; }
        public double PageSize { get; set; }
        public PagedResultDto<UserDto> Users { get; set; }
        public double RequestedPage { get; set; }
        public UserListViewModel()
        {

        }
        public IReadOnlyList<RoleDto> Roles { get; set; }
        public IReadOnlyList<AdministratorDto> Administrations { get; set; }
    }
}
