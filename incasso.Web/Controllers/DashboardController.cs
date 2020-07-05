using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using incasso.Dashboard;
using incasso.Dashboard.dto;
using System.Threading.Tasks;

namespace Incasso.Web.Controllers
{
    [AbpMvcAuthorize]
    public class DashboardController : ControllerBase
    {
        public IDashboardManager _dashboardManager { get; set; }
        public DashboardController(IDashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }
        public async Task<ActionResult> Index()
        {
            DashboardDto model = await _dashboardManager.GetDashboardDto();
            return View(model);
        }
	}
}