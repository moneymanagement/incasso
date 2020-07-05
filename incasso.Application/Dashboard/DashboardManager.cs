using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using incasso.Dashboard.dto;
using Incasso.Authorization.Users;
using Incasso.Authorization.Roles;
using Abp.Domain.Repositories;
using Incasso.Upload;
using Incasso.Administrators;
using Incasso.Administrator;
using Incasso.MultiTenancy.Dto;
using incasso.Invoices.Dto;
using incasso.Debtos;
using incasso.Invoices;
using Abp.AutoMapper;
using Abp.Runtime.Session;
using System.Data.Entity;
using System.Web.Mvc;
using incasso.Helper;
using ParityAccess.Utils.EPPlus;
using System.Data;
using Abp.Localization;

namespace incasso.Dashboard
{
    public class DashboardManager:IDashboardManager
    {
        private readonly UserManager _userManager;
        private readonly IInvoiceManager  _invoiceManager;
        private readonly DebtorManager _debtorManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<StatusCatalog> _StatusCatalogRepository;
        private readonly IRepository<InvoiceNotes.InvoiceNote> _notesRepo;
        private readonly IRepository<Upload> _uplaodRepository;
        private readonly IRepository<Administrator> _adminRepository;
        private readonly IInvoicesAppService _invoicesAppService;
        public IAbpSession AbpSession { get; set; }
        public DashboardManager(
            IRepository<InvoiceNotes.InvoiceNote> notesRepo,
            IInvoiceManager invoiceManager,
            IRepository<Administrator> adminRepository,
            IRepository<Upload> uplodRepo,
            IRepository<User, long> repository,
            IInvoicesAppService invoicesAppService,
            IRepository<StatusCatalog> StatusCatalogRepository,
            UserManager userManager,
            IRepository<Role> roleRepository, DebtorManager debtorManager,
            RoleManager roleManager)
        {
            _notesRepo = notesRepo;
            _StatusCatalogRepository = StatusCatalogRepository;
            _adminRepository = adminRepository;
            _debtorManager = debtorManager;
            _uplaodRepository = uplodRepo;
            _userManager = userManager;
            _invoiceManager = invoiceManager;
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _invoicesAppService = invoicesAppService;
        }

        public async Task<DashboardDto> GetDashboardDto()
        {
            var query = (from user in _userManager.Users
                         from ur in user.Roles
                         join role in _roleManager.Roles on ur.RoleId equals role.Id
                         select new { UserName = user.UserName, Name = user.Name, Id = user.Id, UserRole = role.Name });
            var material = query.ToList();
            var noOfAdmins = _adminRepository.Count();
            var latestUpload = _uplaodRepository.GetAll().OrderByDescending(x => x.CreationTime).FirstOrDefault();
            var fileName = string.Empty;
            var dateStr = string.Empty;
            if (latestUpload != null)
            {
                fileName = latestUpload.FileName;
                dateStr= $"{latestUpload.CreationTime.DayOfWeek.ToString()} {latestUpload.CreationTime.ToString("dd/MM/yyyy hh:mm tt")}" ;
            }
            var result = new DashboardDto
            {
                FileName = fileName,
                UploadedDate = dateStr,
                NoOfAdministrators= noOfAdmins,
                NoOfViewUser = query.Count(x => x.UserRole == "Viewer"),
                NoOfAdministrator = query.Count(x => x.UserRole == "Admin"),
            };

            return result;
        }

        public async Task<CollectionDashboardDto> GetCollectionDashbaordSearchGrid(string invoiceType, string query="" )
        {
            var userid = AbpSession.UserId;
            var user = _userManager.Users.Include(x => x.Administrators).First(x => x.Id == userid);

            var isAdmin = await _userManager.IsInRoleAsync(user.Id, StaticRoleNames.Host.Admin);
            var adminList = user.Administrators.ToList();
            if (isAdmin)
            {
                adminList = await _adminRepository.GetAllListAsync();
            }
            var adminId = adminList.Select(x => x.Id).ToList() ?? new List<int>();

            var queryDB = _debtorManager.GetAll().Include(x => x.Invoices)
                .Where(
                  x => (string.IsNullOrEmpty(query) || x.Name.Contains(query))
                   || x.Number.Contains(query) && adminId.Contains(x.AdministratorId));

            var result = queryDB.ToList();
            var grp = result.GroupBy(x => x.AdministratorId);
            var returnList = new List<KeyValuePair<string, List<DebtorInvoiceGridDto>>>();

            foreach (var item in grp)
            {
                if (!adminList.Any(x => x.Id == item.Key)) continue;
                var adminName = adminList.First(z => z.Id == item.Key).Name;
                var list = new List<DebtorInvoiceGridDto>();
                item.ToList().ForEach(x => {
                    if (x.Invoices.Any(z => z.Type == invoiceType))
                    {
                        var invoices = x.Invoices.Where(z => z.AdministratorId == item.Key && z.Type == invoiceType).MapTo<List<InvoiceDto>>();
                        var model = new DebtorInvoiceGridDto
                        {
                            Country = x.Country,
                            DebtorId = x.Id,
                            Name = x.Name,
                            IsActive = x.Status != 3,
                            DebtorNumber = x.Number
                        };
                        model.Summary = GetSummary(invoices, invoiceType);
                       
                        list.Add(model);
                    }
                });
                if (list.Any())
                {
                    returnList.Add(new KeyValuePair<string, List<DebtorInvoiceGridDto>>(adminName, list));
                }
            }
            var returnModel = new CollectionDashboardDto();

            foreach (var item in returnList)
            {
                if(item.Value.Where(x=>x.IsClosed).Any())
                returnModel.ClosedDebtorList.Add(new KeyValuePair<string, List<DebtorInvoiceGridDto>>(item.Key, item.Value.Where(x=>x.IsClosed).ToList()));
                if(item.Value.Where(x=>!x.IsClosed).Any())
                returnModel.DebtorList.Add(new KeyValuePair<string, List<DebtorInvoiceGridDto>>(item.Key, item.Value.Where(x=>!x.IsClosed).ToList()));
            }
            return returnModel;
        }

        public async Task<GraphDto> GetGraph(int debtorId, bool isCloseInvoice = false,string invoiceType = incasso.Catalogs.PortalType.Collection)
        {
            var debtor = await _debtorManager.GetByIdAsync(debtorId);
            var InvoiceList = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch { InvoiceType = invoiceType, Closed = null, DebtorId = debtorId });

            var graphDto =   GetSummary(InvoiceList,invoiceType);
            
            graphDto.DebtorDetails = debtor.MapTo<DebtorDto>();
            
            return graphDto;
        }

        public  GraphDto  GetSummary(List<InvoiceDto> invoiceList, string invoiceType = incasso.Catalogs.PortalType.Collection)
        {
            var dto= new GraphDto()
            {
                DossierNo = string.Join(";", invoiceList.Select(x => x.DossierNo?.Trim()).Distinct()),
                Currency = invoiceList.FirstOrDefault()?.Currency ?? "€",
                TotalInvoiceAmmount = invoiceList.Sum(x => x.Amount ?? 0),
                TotalPaidBeforeSubmission = invoiceList.Sum(x => x.Amount ?? 0) - invoiceList.Sum(x => x.Open ?? 0),
                TotalInterest = invoiceList.Sum(x => x.Interest ?? 0),
                TotalCollectionCharges = invoiceList.Sum(x => x.CollectionFee ?? 0),
                TotalAdminCosts = invoiceList.Sum(x => x.AdminCosts ?? 0),
                TotalOpen = invoiceType == incasso.Catalogs.PortalType.Collection ?  invoiceList.Sum(x => x.Open ?? 0): (invoiceList.Sum(x => x.Open ?? 0) + invoiceList.Sum(x => x.AdminCosts ?? 0)),
                TotalPaid = invoiceType == incasso.Catalogs.PortalType.Collection ?
                invoiceList.Where(x=>!x.Closed).Sum(x => x.Paidmm ?? 0) + invoiceList.Where(x => !x.Closed).Sum(x => x.PaidClient ?? 0)+
                invoiceList.Where(x=>x.Closed).Sum(x => x.TotalOpenInc ?? 0) :
                invoiceList.Sum(x=>x.Paid)
               
        };


            //Paid = 'Betaald bedrag MM' + 'Betaald bedrag' klant of the open invoices AND
            //when you remove the invoice from the paid overview it = ' Total open (incl. (I&C) amount' + 'betaald bedrag MM' + 'Betaald bedrag klant'.


            dto.OutsourcingOutStanding = invoiceList.Where(x => !x.Closed).Sum(x => x.Open?? 0) + invoiceList.Sum(x => x.AdminCosts ?? 0);

            var openInvoice = invoiceList.Where(x => !x.Closed).ToList();
            dto.TotalTotalOutStanding =
                openInvoice.Sum(x => x.Open ?? 0) +
                openInvoice.Sum(x => x.Interest ?? 0) +
                openInvoice.Sum(x => x.CollectionFee ?? 0) +
                openInvoice.Sum(x => x.AdminCosts ?? 0) -
                (openInvoice.Sum(x => x.Paidmm ?? 0) + openInvoice.Sum(x => x.PaidClient ?? 0));

            return dto;
        }

        public async   Task<FileStreamResult> ExportOpenCollectionInvoice(InvoiceDownloadInput input, string logoImagePath = "") {
            var invoices = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch() {InvoiceType=input.Portal, DebtorId = input.DebtorId, Closed = input.IsClosedInvoice });
            var debtor =await  _debtorManager.GetByIdAsync(input.DebtorId);
            var source = LocalizationHelper.GetSource("incasso");

            var dt = new DataTable();
            List<ExportParameterModel> parameters = new List<ExportParameterModel>();
            parameters.Add(new ExportParameterModel() { ColumnWidth = 150, DataField = "InvoiceNo", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("ExcelInvoiceNumber") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "InvoiceDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Date" )});
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "ExpiredDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("DueDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Amount",   HeaderText = source.GetString("ExcelAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Open",  HeaderText = source.GetString("ExcelOpenAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Interest", HeaderText = source.GetString("Interest") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "CollectionFee", HeaderText = source.GetString("ExcelCollectionCharges") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Paidmm",  HeaderText = source.GetString("PaidMM") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "PaidClient",  HeaderText = source.GetString("PaidClient") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "TotalPayment",   HeaderText = source.GetString("TotalPayment") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 200, DataField = "Status", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Status") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 500, DataField = "Notes", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText =  source.GetString("Notes") });
            

            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceNo" });
            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "ExpiredDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Amount" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Open" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Interest" }); 
            dt.Columns.Add(new DataColumn() { ColumnName = "CollectionFee" }); 
            dt.Columns.Add(new DataColumn() { ColumnName = "Paidmm" });
            dt.Columns.Add(new DataColumn() { ColumnName = "PaidClient" });
            dt.Columns.Add(new DataColumn() { ColumnName = "TotalPayment" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Status" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Notes" });

            var statusIds = invoices.Select(x => x.Status).ToList();
            var statusList = _StatusCatalogRepository.GetAll().Where(x => statusIds.Contains(x.Id)).ToList();


            var notesIds = invoices.Select(x => x.Id).ToList();
            var notes = _notesRepo.GetAll().Where(x => notesIds.Contains(x.InvoiceId)).ToList();

            var grpNotes = notes.GroupBy(x => x.InvoiceId).Select(y => new { y.Key, Notes = string.Join(".", y.OrderByDescending(x => x.NoteDate).Select(x => $" {x.NoteDate.ToGridFormat() } {x.Notes}")) });
            foreach (var invoice in invoices)
            {
                DataRow dr = dt.NewRow();
                dr["InvoiceNo"] = invoice.InvoiceNo;
                dr["InvoiceDate"] = DateHelper.ToGridDateFormat(invoice.InvoiceDate);
                dr["ExpiredDate"] = DateHelper.ToGridDateFormat(invoice.ExpiredDate);
                dr["Amount"] = invoice.Amount??0;
                dr["Open"] = invoice.Open??0;
                dr["Interest"] = invoice.Interest??0;
                dr["CollectionFee"] = invoice.CollectionFee ?? 0;
                dr["Paidmm"] = invoice.Paidmm??0;
                dr["PaidClient"] = invoice.PaidClient??0;
                dr["TotalPayment"] = (invoice.Paidmm??0)+(invoice.PaidClient??0) ;
                dr["Status"] = statusList.FirstOrDefault(x=>x.Id==invoice.Status)?.Catalog;
                dr["Notes"] = grpNotes.FirstOrDefault(x=>x.Key==invoice.Id)?.Notes;
                dt.Rows.Add(dr);
                //rowStyling.Add(new ExportParameterRowStyle()
                //{
                //    BorderThickness = invoice.IsTotalRow ? ExcelBorderStyle.Thick : ExcelBorderStyle.Thin,
                //    BorderColor = "#000000",
                //    Bold = invoice.IsTotalRow,
                //    RowIndex = dt.Rows.Count - 1
                //});
            }



            var file = FileExportHelper.ExportToExcelFromDataTableWithGroupedHeaders(dt, parameters, DateTime.Now.Millisecond.ToString() + ".XLSX", debtorName: debtor?.Name ?? string.Empty, titleHeader:  "MM", imagePath: logoImagePath);
            return file;
        }
        public async   Task<FileStreamResult> ExportCloseCollectionInvoice(InvoiceDownloadInput input, string logoImagePath = "") {
            var invoices = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch() {InvoiceType=input.Portal, DebtorId = input.DebtorId, Closed = input.IsClosedInvoice });
            var debtor =await  _debtorManager.GetByIdAsync(input.DebtorId);
            var source = LocalizationHelper.GetSource("incasso");

            var dt = new DataTable();
            List<ExportParameterModel> parameters = new List<ExportParameterModel>();
            parameters.Add(new ExportParameterModel() { ColumnWidth = 150, DataField = "InvoiceNo", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Invoicenr") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "InvoiceDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Date" )});
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "ExpiredDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("DueDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "PaymentDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("PaymentDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "Expired", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, HeaderText = source.GetString("Expired") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Amount",   HeaderText = source.GetString("InvoiceAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 500, DataField = "Notes", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText =  source.GetString("Notes") });

            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceNo" });
            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "ExpiredDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "PaymentDate" }); 
            dt.Columns.Add(new DataColumn() { ColumnName = "Expired" }); 
            dt.Columns.Add(new DataColumn() { ColumnName = "Amount" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Notes" });

            var statusIds = invoices.Select(x => x.Status).ToList();
            var statusList = _StatusCatalogRepository.GetAll().Where(x => statusIds.Contains(x.Id)).ToList();


            var notesIds = invoices.Select(x => x.Id).ToList();
            var notes = _notesRepo.GetAll().Where(x => notesIds.Contains(x.InvoiceId)).ToList();

            var grpNotes = notes.GroupBy(x => x.InvoiceId).Select(y => new { y.Key, Notes = string.Join(".", y.OrderByDescending(x => x.NoteDate).Select(x => $" {x.NoteDate.ToGridFormat() } {x.Notes}")) });
            foreach (var invoice in invoices)
            {
                var days = 0;
                DateTime? expired = invoice.ExpiredDate.ToDate();
                DateTime? paymentDate = invoice.PaymentDate.ToDate();
                if (expired.HasValue && paymentDate.HasValue)
                {
                    days = (int)((expired.Value - paymentDate.Value).TotalDays);
                }

                DataRow dr = dt.NewRow();
                dr["InvoiceNo"] = invoice.InvoiceNo;
                dr["InvoiceDate"] = DateHelper.ToGridDateFormat(invoice.InvoiceDate);
                dr["ExpiredDate"] = DateHelper.ToGridDateFormat(invoice.ExpiredDate);
                dr["PaymentDate"] = DateHelper.ToGridDateFormat(invoice.PaymentDate);
                dr["Expired"] = days;
                dr["Amount"] = invoice.Amount??0;
                dr["Notes"] = grpNotes.FirstOrDefault(x=>x.Key==invoice.Id)?.Notes;
                dt.Rows.Add(dr);
                //rowStyling.Add(new ExportParameterRowStyle()
                //{
                //    BorderThickness = invoice.IsTotalRow ? ExcelBorderStyle.Thick : ExcelBorderStyle.Thin,
                //    BorderColor = "#000000",
                //    Bold = invoice.IsTotalRow,
                //    RowIndex = dt.Rows.Count - 1
                //});
            }

            var file = FileExportHelper.ExportToExcelFromDataTableWithGroupedHeaders(dt, parameters, DateTime.Now.Millisecond.ToString() + ".XLSX", debtorName: debtor?.Name ?? string.Empty, titleHeader:  "MM", imagePath: logoImagePath);
            return file;
        }

        public async Task<FileStreamResult> ExportCloseOutsourcingInvoice(InvoiceDownloadInput input, string root)
        {
            var invoices = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch() { InvoiceType = input.Portal, DebtorId = input.DebtorId, Closed = input.IsClosedInvoice });
            var debtor = await _debtorManager.GetByIdAsync(input.DebtorId);
            var source = LocalizationHelper.GetSource("incasso");

            var dt = new DataTable();
            List<ExportParameterModel> parameters = new List<ExportParameterModel>();
            parameters.Add(new ExportParameterModel() { ColumnWidth = 150, DataField = "InvoiceNo", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("ExcelInvoiceNumber") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "InvoiceDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Date") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "ExpiredDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("DueDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "PaymentDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("PaymentDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "Expired", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, HeaderText = source.GetString("Expired") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Amount", HeaderText = source.GetString("ExcelAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 500, DataField = "Notes", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Notes") });

            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceNo" });
            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "ExpiredDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "PaymentDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Expired" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Amount" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Notes" });

            var statusIds = invoices.Select(x => x.Status).ToList();
            var statusList = _StatusCatalogRepository.GetAll().Where(x => statusIds.Contains(x.Id)).ToList();


            var notesIds = invoices.Select(x => x.Id).ToList();
            var notes = _notesRepo.GetAll().Where(x => notesIds.Contains(x.InvoiceId)).ToList();

            var grpNotes = notes.GroupBy(x => x.InvoiceId).Select(y => new { y.Key, Notes = string.Join(".", y.OrderByDescending(x=>x.NoteDate).Select(x => $" {x.NoteDate.ToGridFormat() } {x.Notes}")) });
            foreach (var invoice in invoices)
            {
                var days = 0;
                DateTime? expired = invoice.ExpiredDate.ToDate();
                DateTime? paymentDate = invoice.PaymentDate.ToDate();
                if (expired.HasValue && paymentDate.HasValue)
                {
                    days = (int)((expired.Value - paymentDate.Value).TotalDays);
                }

                DataRow dr = dt.NewRow();
                dr["InvoiceNo"] = invoice.InvoiceNo;
                dr["InvoiceDate"] = DateHelper.ToGridDateFormat(invoice.InvoiceDate);
                dr["ExpiredDate"] = DateHelper.ToGridDateFormat(invoice.ExpiredDate);
                dr["PaymentDate"] = DateHelper.ToGridDateFormat(invoice.PaymentDate);
                dr["Expired"] = days;
                dr["Amount"] = invoice.Amount ?? 0;
                dr["Notes"] = grpNotes.FirstOrDefault(x => x.Key == invoice.Id)?.Notes;
                dt.Rows.Add(dr);
                //rowStyling.Add(new ExportParameterRowStyle()
                //{
                //    BorderThickness = invoice.IsTotalRow ? ExcelBorderStyle.Thick : ExcelBorderStyle.Thin,
                //    BorderColor = "#000000",
                //    Bold = invoice.IsTotalRow,
                //    RowIndex = dt.Rows.Count - 1
                //});
            }

            var file = FileExportHelper.ExportToExcelFromDataTableWithGroupedHeaders(dt, parameters, DateTime.Now.Millisecond.ToString() + ".XLSX", debtorName: debtor?.Name ?? string.Empty, titleHeader: "MM", imagePath: root);
            return file;
        }

        public async Task<FileStreamResult> ExportOpenOutsourcingInvoice(InvoiceDownloadInput input, string root)
        {
            var invoices = await _invoiceManager.GetDebtorInvoiceList(new CriteriaInvoiceSearch() { InvoiceType = input.Portal, DebtorId = input.DebtorId, Closed = input.IsClosedInvoice });
            var debtor = await _debtorManager.GetByIdAsync(input.DebtorId);
            var source = LocalizationHelper.GetSource("incasso");

            var dt = new DataTable();
            List<ExportParameterModel> parameters = new List<ExportParameterModel>();
            parameters.Add(new ExportParameterModel() { ColumnWidth = 150, DataField = "InvoiceNo", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("ExcelInvoiceNumber") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "InvoiceDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Date") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "ExpiredDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("DueDate") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, DataField = "Exp", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("ExcelExpired") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Amount", HeaderText = source.GetString("ExcelAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "AdminCosts", HeaderText = source.GetString("ExcelAdministrationCosts") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Open", HeaderText = source.GetString("ExcelOpenAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Paid", HeaderText = source.GetString("ExcelPaidAmount") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 100, TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right, DataFormat = ExportDataFormatCatalog.Number, DataField = "Dispute", HeaderText = source.GetString("Dispute") });
            parameters.Add(new ExportParameterModel() { ColumnWidth = 500, DataField = "Notes", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = source.GetString("Notes") });

            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceNo" });
            dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "ExpiredDate" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Exp" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Amount" });
            dt.Columns.Add(new DataColumn() { ColumnName = "AdminCosts" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Open" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Paid" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Dispute" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Notes" });

             
            var notesIds = invoices.Select(x => x.Id).ToList();
            var notes = _notesRepo.GetAll().Where(x => notesIds.Contains(x.InvoiceId)).ToList();

            var grpNotes = notes.GroupBy(x => x.InvoiceId).Select(y => new { y.Key, Notes = string.Join(".", y.OrderByDescending(x => x.NoteDate).Select(x => $" {x.NoteDate.ToGridFormat() } {x.Notes}")) });
            foreach (var invoice in invoices)
            {
                var days = 0;
                DateTime? expired = invoice.ExpiredDate.ToDate();
                DateTime? paymentDate = invoice.PaymentDate.ToDate();
                if (expired.HasValue && paymentDate.HasValue)
                {
                    days = (int)((expired.Value - paymentDate.Value).TotalDays);
                }

                DataRow dr = dt.NewRow();
                dr["InvoiceNo"] = invoice.InvoiceNo;
                dr["InvoiceDate"] = DateHelper.ToGridDateFormat(invoice.InvoiceDate);
                dr["ExpiredDate"] = DateHelper.ToGridDateFormat(invoice.ExpiredDate);
                dr["Exp"] = days;
                dr["Amount"] = invoice.Amount ?? 0;
                dr["Open"] = invoice.Open ?? 0;
                dr["Paid"] = invoice.Paid ?? 0;
                dr["AdminCosts"] = invoice.AdminCosts ?? 0;
                dr["Dispute"] = invoice.DisputeAction;
                dr["Notes"] = grpNotes.FirstOrDefault(x => x.Key == invoice.Id)?.Notes;
                dt.Rows.Add(dr);
                //rowStyling.Add(new ExportParameterRowStyle()
                //{
                //    BorderThickness = invoice.IsTotalRow ? ExcelBorderStyle.Thick : ExcelBorderStyle.Thin,
                //    BorderColor = "#000000",
                //    Bold = invoice.IsTotalRow,
                //    RowIndex = dt.Rows.Count - 1
                //});
            }

            var file = FileExportHelper.ExportToExcelFromDataTableWithGroupedHeaders(dt, parameters, DateTime.Now.Millisecond.ToString() + ".XLSX", debtorName: debtor?.Name ?? string.Empty, titleHeader: "MM", imagePath: root);
            return file;
        }
    }
}
