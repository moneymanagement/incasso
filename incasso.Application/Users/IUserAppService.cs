using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Incasso.Roles.Dto;
using Incasso.Users.Dto;
using incasso.Users.Dto;
using System.Collections.Generic;

namespace Incasso.Users
{
    public interface IUsersAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
        Task<UserListViewModel> GetGrid(CriteriaUserSearch pagedResultRequestDto);
        Task<List<UserDto>> GetUsers(string v);
    }
}