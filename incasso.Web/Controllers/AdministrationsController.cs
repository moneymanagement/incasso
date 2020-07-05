using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using Incasso.Authorization;
using Incasso.MultiTenancy;
using Incasso.Administrator;
using Incasso.Users;
using Incasso.MultiTenancy.Dto;

namespace Incasso.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AdministrationsController : ControllerBase
    {
        private IAdministrationsAppService administratorAppService;
        private IUsersAppService usersAppService;

        public AdministrationsController(IAdministrationsAppService administratorAppService, IUsersAppService usersAppService)
        {
            this.administratorAppService = administratorAppService;
            this.usersAppService = usersAppService;
        }
        public async Task<ActionResult> Index()
        {
            int? currentPage = 0; int? pageSize = 50;
            var model = await administratorAppService.GetGrid(new CriteriaAdministratorSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = currentPage.Value });
            var user = await usersAppService.GetUsers("Viewer");
            model.Users = user;
            return View(model);
        }
        public async Task<ActionResult> GetGrid(string search = "", int? requestedPage = 0, int? pageSize = 50)
        {
            var model = await administratorAppService.GetGrid(new CriteriaAdministratorSearch { Search = search, PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = requestedPage.Value });
            return View("_GetGrid", model);
        }
    }
}