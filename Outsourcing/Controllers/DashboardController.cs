using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using incasso.Dashboard;
using incasso.Dashboard.dto;
using System.Threading.Tasks;
using incasso.Debtos;
using Incasso.Administrators;
using incasso.Administrators.dto;
using Abp.AutoMapper;
using Incasso.MultiTenancy.Dto;
using incasso.Invoices;
using incasso.Invoices.Dto;
using incasso.Catalogs;
using System.Linq;
using System;

namespace Outsourcing.Web.Controllers
{
    [AbpMvcAuthorize]
    public class DashboardController : ControllerBase
    {
        public IDashboardManager _dashboardManager { get; set; }
        public DebtorManager _debtorManager { get; set; }
        public AdministratorManager _adminManager { get; set; }
        public IInvoiceManager _invoiceManager { get; set; }
        public DashboardController(IInvoiceManager invoiceManager, AdministratorManager adminManager, IDashboardManager dashboardManager, DebtorManager debtorManager)
        {
            _invoiceManager = invoiceManager;
            _adminManager = adminManager;
            _debtorManager = debtorManager;
            _dashboardManager = dashboardManager;
        }
        public async Task<ActionResult> Index()
        {
            var model = await _dashboardManager.GetCollectionDashbaordSearchGrid(PortalType.Outsourcing);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AdminDetails(int debtorId)
        {
            var admin = await _adminManager.GetAdminByDebtorId(debtorId);
            var admindto = admin.MapTo<AdministratorDto>();
            return View(admindto);
        }
        [HttpPost]

        public async Task<ActionResult> Graph(int debtorId)
        {
            GraphDto graphDto = await _dashboardManager.GetGraph(debtorId, invoiceType: PortalType.Outsourcing);

            return View(graphDto);
        }

        [HttpPost]
        public async Task<ActionResult> InvoiceDetails(int debtorId)
        {
            var dashboardDto = new CollectionDashboardDto()
            {
                InvoiceList = await _invoiceManager.GetDebtorInvoiceList(
                    new CriteriaInvoiceSearch
                    {
                        Closed = false,
                        InvoiceType = PortalType.Outsourcing,
                        DebtorId = debtorId
                    })
            };
            return View(dashboardDto);
        }

        [HttpPost]
        public async Task<ActionResult> DebtorDetails(int debtorId)
        {
            var debtor = await _debtorManager.GetByIdAsync(debtorId);
            var dto = debtor.MapTo<DebtorDto>();
            return View(dto);
        }

        [HttpPost]
        public async Task<ActionResult> InvoiceClosedDetails(int debtorId)
        {
            var dashboardDto = new CollectionDashboardDto()
            {
                InvoiceList = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch { InvoiceType = PortalType.Outsourcing, Closed = true, DebtorId = debtorId })
            };
            return View(dashboardDto);
        }

        [HttpPost]
        public async Task<ActionResult> DebtorList(string query)
        {
            var model = await _dashboardManager.GetCollectionDashbaordSearchGrid(PortalType.Outsourcing);
            return View("_DebtorList", model);
        }
        //[HttpGet]
        //public async Task<ActionResult> InvoiceDownload(int debtorId, bool? isClosed)
        //{
        //    var fileBytes =  await _dashboardManager.Export(new InvoiceDownloadInput { DebtorId = debtorId, IsClosedInvoice = isClosed.Value, Portal = PortalType.Outsourcing });
        //    return File(fileBytes, "application/vnd.ms-excel", DateTime.Now.Millisecond.ToString());

        //}


        //[HttpGet]
        //public async Task<ActionResult> Download(int? Id, bool? isClosed = false)
        //{
        //    var byteArray= await _dashboardManager.Export(new InvoiceDownloadInput { DebtorId = Id.Value, IsClosedInvoice = isClosed.Value, Portal = PortalType.Outsourcing });
        //    return File(byteArray, "application/vnd.ms-excel", DateTime.Now.Millisecond.ToString());

        //}

        [HttpGet]
        public async Task<FileResult> InvoiceDownload(int debtorId, bool? isClosed = false)
        {
            FileStreamResult filestreamResult = null;
            var virtualPath = $"~/Scripts/public/images/Logo-menu.png";
            var root = Server.MapPath(virtualPath);

            if (isClosed ?? false)
                filestreamResult = await _dashboardManager.ExportCloseOutsourcingInvoice(new InvoiceDownloadInput { DebtorId = debtorId, IsClosedInvoice = isClosed.Value, Portal = PortalType.Outsourcing }, root);
            else
                filestreamResult = await _dashboardManager.ExportOpenOutsourcingInvoice(new InvoiceDownloadInput { DebtorId = debtorId, IsClosedInvoice = isClosed.Value, Portal = PortalType.Outsourcing }, root);

            return filestreamResult;
        }
    }
}