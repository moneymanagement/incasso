using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using incasso.Administrators.dto;
using incasso.Debtos;
using Incasso.Administrator;
using Incasso.Administrators;
using Incasso.Authorization;
using Incasso.MultiTenancy;
using Incasso.MultiTenancy.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Incasso.Web.Controllers
{
    [AbpMvcAuthorize]
    public class DebtorsController : ControllerBase
    {
        private readonly IDebtorsAppService _deptorAppService;
        private readonly IAdministrationsAppService _adminAppService;
        public DebtorsController(IAdministrationsAppService administratorAppService, IDebtorsAppService DebtorAppService)
        {
            _adminAppService = administratorAppService;
            _deptorAppService = DebtorAppService;
        }
        public async Task<ActionResult> Index()
        {
            int? currentPage = 0; int? pageSize = 50;
            var model = await _deptorAppService.GetGrid(new CriteriaDebtorSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = currentPage.Value });
            var   admins = await _adminAppService.GetAll(new  PagedResultRequestDto { MaxResultCount = int.MaxValue, SkipCount = 0 });
            model.Administrators = admins.Items;
            return View(model);
        }
        public async Task<ActionResult> GetGrid(int? adminId,string search = "", int? requestedPage = 0, int? pageSize = 50)
        {
            var model = await _deptorAppService.GetGrid(new CriteriaDebtorSearch { AdminId = adminId, Search = search, PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = requestedPage.Value });
            return View("_GetGrid", model);
        }
    }
}