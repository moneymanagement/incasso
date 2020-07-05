using Abp.Application.Services;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using incasso.Dashboard.dto;
using Incasso.Administrator;
using Incasso.MultiTenancy.Dto;
using incasso.Invoices.Dto;
using System.Web.Mvc;

namespace incasso.Dashboard
{
    public interface IDashboardManager : ITransientDependency
    {
        Task<DashboardDto> GetDashboardDto();
        Task<CollectionDashboardDto> GetCollectionDashbaordSearchGrid(string invoiceType  ,string query="");
        Task<GraphDto> GetGraph(int debtorId, bool isCloseInvoice = false, string invoiceType= incasso.Catalogs.PortalType.Collection);
        Task<FileStreamResult> ExportOpenCollectionInvoice(InvoiceDownloadInput input,string logoImagePath="");
        Task<FileStreamResult> ExportCloseCollectionInvoice(InvoiceDownloadInput input, string logoImagePath = "");
        Task<FileStreamResult> ExportCloseOutsourcingInvoice(InvoiceDownloadInput invoiceDownloadInput, string root);
        Task<FileStreamResult> ExportOpenOutsourcingInvoice(InvoiceDownloadInput invoiceDownloadInput, string root);
    }
}
