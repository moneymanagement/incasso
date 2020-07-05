using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Incasso.Authorization;
using Incasso.Authorization.Roles;
using Incasso.Users;
using Incasso.Authorization.Users;
using incasso.Users.Dto;
using Incasso.Administrator;

namespace Incasso.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Users)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersAppService _userAppService;
        private readonly RoleManager _roleManager;
        private readonly IAdministrationsAppService administrationsAppService;
        private readonly UserManager _userManager;
        public UsersController(IAdministrationsAppService administrationsAppService,UserManager userManager, IUsersAppService userAppService, RoleManager roleManager)
        {
            _userManager = userManager;
            this.administrationsAppService = administrationsAppService;
            _userAppService = userAppService;
            _roleManager = roleManager;
        }
        public async Task<ActionResult> Index()
        {
            int? currentPage = 0; int? pageSize = 50;
            var model = await _userAppService.GetGrid(new CriteriaUserSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value,RequestedPage=currentPage.Value});
            model.Roles =( await _userAppService.GetRoles()).Items;
            model.Administrations = (await administrationsAppService.GetAll(new Abp.Application.Services.Dto.PagedResultRequestDto() { MaxResultCount= int.MaxValue})).Items;
            return View(model);
        }
        public async Task<ActionResult> GetGrid(string search="",int? requestedPage = 0, int? pageSize = 50)
        {
            var model = await _userAppService.GetGrid(new CriteriaUserSearch {Search=search, PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = requestedPage.Value});
            return View("_GetGrid", model);
        }
    }
}