using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Incasso.Authorization;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.Roles.Dto;
using Incasso.Users.Dto;
using Microsoft.AspNet.Identity;
using incasso.Users.Dto;
using Abp.AutoMapper;
using AutoMapper;
using incasso.Debtors;
using System.Data.Entity;
using incasso.Helper;
using incasso.Administrators.dto;

namespace Incasso.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UsersAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>, IUsersAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Debtor> _debtorRepository;
        private readonly IRepository<UsersAdministrators> _userAdminRepository;
        private readonly IRepository<Administrators.Administrator> _adminRepository;
        private readonly IRepository<Role> _roleRepository;

        public UsersAppService(
            IRepository<User, long> repository,
           // IRepository<UsersAdministrators> userAdminRepository,
            UserManager userManager,
            IRepository<Administrators.Administrator> adminRepository,
            IRepository<Role> roleRepository,
            IRepository<Debtor> debtorRepository,
            RoleManager roleManager)
            : base(repository)
        {

          ///  _userAdminRepository = userAdminRepository;
            _adminRepository = adminRepository;
            _debtorRepository = debtorRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _roleManager = roleManager;
        }

        public override async Task<UserDto> Get(EntityDto<long> input)
        {
            var user = _userManager.Users.Include(x => x.Administrators).Include(x => x.Roles).FirstOrDefault(x=>x.Id==input.Id);
            var result = ObjectMapper.Map<UserDto>(user);
            var roles = _userManager.GetRoles(user.Id);
            result.ROLES = roles.ToArray();
            return result;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();
            var user = ObjectMapper.Map<User>(input);
            user.TenantId = AbpSession.TenantId;
            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.IsEmailConfirmed = true;
            CheckErrors(await _userManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();
            //Assign roles
            user.Roles = new Collection<UserRole>();
            var admins = (input.Admins ?? new List<string>()).Select(x=>int.Parse(x)).ToList();
            var administrators = _adminRepository.GetAll().Where(x => admins.Contains(x.Id)).ToList();
            user.Administrators = administrators;

            var roles = _roleManager.Roles.Where(r=>input.RoleNames.Any(y=>y==r.Name)).ToList();
            foreach (var roleName in roles)
                    _userManager.AddToRole(user.Id,roleName.Name);
            await CurrentUnitOfWork.SaveChangesAsync();
            return user.MapTo<UserDto>();
        }

        public override async Task<UserDto> Update(UpdateUserDto input)
        {

            var user = await _userManager.Users.Include(x=>x.Administrators).FirstOrDefaultAsync(X => X.Id == input.Id);
            user.Administrators?.Clear();
            var intialPasswor = user.Password;
            MapToEntity(input, user);

            if (!string.IsNullOrEmpty(input.Password))
                await _userManager.ChangePasswordAsync(user, input.Password);
            else
                user.Password = intialPasswor;

            var adminIdList = (input.Admins ?? new List<string>()).Select(x => int.Parse(x)).ToList();
            var admin = _adminRepository.GetAll().Where(x => adminIdList.Contains(x.Id)).ToArray();
                user.Administrators = new List<Administrators.Administrator>() ;
            foreach (var item in admin)
                user.Administrators.Add(item);

            CheckErrors(await _userManager.UpdateAsync(user));

            var roles = await _userManager.GetRolesAsync(user.Id);
            await _userManager.RemoveFromRolesAsync(user.Id, roles.ToArray());

            await UnitOfWorkManager.Current.SaveChangesAsync();
            if (input.RoleNames != null)
                CheckErrors(_userManager.AddToRoles(user.Id, input.RoleNames));
            var returnValue = user.MapTo<UserDto>();
            return returnValue;
        }
        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            return user;
        }

        protected override void MapToEntity(UpdateUserDto input, User user)
        {
            ObjectMapper.Map(input, user);
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = Repository.GetAllIncluding(x => x.Roles).FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(user);
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<UserListViewModel> GetGrid(CriteriaUserSearch input)
        {
            input.SkipCount = (int)((input.RequestedPage * input.PageSize));
            var query = (from user in _userManager.Users
                        from ur in user.Roles
                        join role in _roleManager.Roles on ur.RoleId equals role.Id into userRoles
                        from role in userRoles.DefaultIfEmpty()
                        where string.IsNullOrEmpty(input.Search) ||
                        user.UserName.Contains(input.Search) || input.Search.Contains(user.UserName)
                        || role.Name.Contains(input.Search) || input.Search.Contains(role.Name) || user.Administrators.Any(y=>y.Name.Contains(input.Search))
                        select new { UserName = user.UserName, Administrator = user.Administrators.ToList(), Name = user.Name, Id = user.Id, UserRole =role }
                         ).OrderBy(x => x.UserName);

            var count = query.Count();
            var queryResult = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            var userList = new List<UserDto>();
            foreach (var item in queryResult.GroupBy(x => x.UserName))
            {
                var tempUser = item.First();
                var administrators = item.SelectMany(x => x.Administrator).DistinctBy(x => x.Id).MapTo<List<AdministratorDto>>();
                userList.Add(new UserDto
                {
                    Id = tempUser.Id,
                    UserName=tempUser.UserName,
                    Name=tempUser.Name,
                    ROLES = item.Select(Y=>Y.UserRole.Name).ToArray(),
                    Administrators= administrators,
                });
            }
            return new UserListViewModel
            {
                Search = input.Search,
                PageSize = input.PageSize,
                RequestedPage = input.RequestedPage,
                Users = new PagedResultDto<UserDto> { Items = userList, TotalCount = count },
            };
        }

        public async Task<List<UserDto>> GetUsers(string v)
        {
            var query = from user in _userManager.Users
                        from ur in user.Roles
                        join role in _roleManager.Roles on ur.RoleId equals role.Id
                        where role.Name == v
                        select user;
            return query.MapTo<List<UserDto>>();

        }
    }
}