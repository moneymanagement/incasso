using Abp.Domain.Repositories;
using incasso.Invoices;
using Incasso.Administrator;
using Incasso.MultiTenancy.Dto;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Incasso.Web.Controllers
{
    [Authorize]
    public class ActionsController : ControllerBase
    {
        private readonly IInvoicesAppService invoicesAppService;
        private readonly IAdministrationsAppService _adminAppService;
        private readonly IRepository<StatusCatalog> _StatusCatalogRepository;
        public ActionsController(IInvoicesAppService administratorAppService , IRepository<StatusCatalog> StatusCatalogRepository)
        {
            invoicesAppService = administratorAppService;
            _StatusCatalogRepository = StatusCatalogRepository;
        }
        public async Task<ActionResult> Index()
        {  
            int? currentPage = 0; int? pageSize = 50;
            var model = await invoicesAppService.GetGrid(new CriteriaInvoiceSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = currentPage.Value });
            model.StatusCatalog = await _StatusCatalogRepository.GetAllListAsync();
            return View(model);
        }
        public async Task<ActionResult> GetGrid(int? adminId, string search = "", int? requestedPage = 0,string InvoiceType="" , int? pageSize = 50)
        {
            var model = await invoicesAppService.GetGrid(new CriteriaInvoiceSearch { AdminId = adminId, Search = search, InvoiceType= InvoiceType, PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = requestedPage.Value });
            return View("_GetGrid", model);
        }

        //public async Task<ActionResult> ActionsList()
        //{
        //    int? currentPage = 0; int? pageSize = 50;
        //    var model = await invoicesAppService.GetGrid(new CriteriaInvoiceSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = currentPage.Value });
        //    model.StatusCatalog = await _StatusCatalogRepository.GetAllListAsync();
        //    return View(model);
        //}
    }
}