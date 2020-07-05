using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using incasso.Catalogs;
using incasso.ExcelModel;
using incasso.Invoices;
using Incasso.Administrators;
using Incasso.Upload;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace incasso.Jobs.UploadJob
{
    public class ProcessUploadFile :  ITransientDependency
    {
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Incasso.Upload.Upload> _uploadRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger _logger;

        public ProcessUploadFile(ILogger logger, IUnitOfWorkManager  unitOfWorkManager, IInvoiceManager invoiceManager, IRepository<Incasso.Upload.Upload> repository)
        {
            _logger = logger;
            _invoiceManager = invoiceManager;
            _uploadRepository = repository;
            _unitOfWorkManager = unitOfWorkManager;
        } 
        public   void Execute(int id)
        {
            try
            {
                Upload upload;
                using (var vow = _unitOfWorkManager.Begin())
                {
                    upload = _uploadRepository.GetAll().Include(x => x.Administrators).First(x => x.Id == id);
                    vow.Complete();
                }
                if (!string.IsNullOrEmpty(upload.PhysicalFilePath))
                {
                    FileInfo existingFile = new FileInfo(upload.PhysicalFilePath);
                    var listExcelModel = new List<ExcelModel.ExcelModel>();
                    Administrator admin ;
                    if (upload.FileType == PortalType.Collection)
                    {
                        listExcelModel = GetCollectionModel(existingFile,upload,out admin);
                    }
                    else
                    {
                        listExcelModel = GetOutsourcingModel(existingFile, upload,out admin);
                    }
                    
                  var args =    _invoiceManager.ProcessExcelData(admin, upload,listExcelModel);

                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in Job Upload Id {id} <b>Message {ex.Message}</b>   Inner Message :{ ex.InnerException }");
            }
        }

        private List<ExcelModel.ExcelModel> GetCollectionModel(FileInfo existingFile, Upload upload,out Administrator admin)
        {
            admin = new Administrator();
            var listExcelModel = new List<ExcelModel.ExcelModel>();
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                if (package.Workbook.Worksheets.Any())
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    int rowCount = worksheet.Dimension.End.Row;
                    int checkCounter = 0;
                    //administrator info in row 1-9
                    for (int row = 1; row <= 9; row++)
                    {
                        switch (row)
                        {
                            case 1:
                                admin.Name = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 2:
                                admin.Number = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 3:
                                admin.AdminId = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 4:
                                admin.Contact = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 5:
                                admin.Phone = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 6:
                                admin.Phone2 = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 7:
                                admin.Email = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 8:
                                admin.Iban = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 9:
                                admin.Bic = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                    //10 11 header info


                    // row 12 invoices of debtors
                    for (int row = 12; row <= rowCount; row++)
                    {
                        int col = 1;
                        // consective 3 rows has empty start columns that mean empty rows
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 2].Value?.ToString() ?? String.Empty))
                        {
                            checkCounter++;
                            if (checkCounter == 3)
                                break;
                            continue;
                        }





                        var model = new ExcelModel.ExcelModel(upload);
                        model.I_ActionDate = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_DossierNo = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Number = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Name = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Address = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Postal = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_City = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Country = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Phone = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Mobile = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Email = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_InvoiceNo = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_InvoiceDate = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Expired = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Amount = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Open = worksheet.Cells[row, col].Value?.ToString().Trim()??string.Empty;
                        col++;
                        model.I_Currency = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_AdminCosts = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Paid = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Interest = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_CollectionFee = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Paidmm = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_PaidClient = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        col++;//Total payment
                        model.I_Action = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;//Continued action
                        model.I_Notes = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Status = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Contact = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        listExcelModel.Add(model);
                        // model. = worksheet.Cells[row, col].Value?.ToString().Trim()??string.Empty;

                    }

                }
            }
            return listExcelModel;
        }

        private List<ExcelModel.ExcelModel> GetOutsourcingModel(FileInfo existingFile, Upload upload, out Administrator admin)
        {
            var listExcelModel =new  List<ExcelModel.ExcelModel>();
            admin = null;
            admin = new Administrator();
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                if (package.Workbook.Worksheets.Any())
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    int rowCount = worksheet.Dimension.End.Row;
                    int checkCounter = 0;
                    //administrator info in row 1-9
                    for (int row = 1; row <= 9; row++)
                    {
                        switch (row)
                        {
                            case 1:
                                admin.Name = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 2:
                                admin.Number = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            case 3:
                                admin.Contact = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 4:
                                admin.Phone = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 5:
                                admin.Phone2 = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 6:
                                admin.Email = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 7:
                                admin.Iban = worksheet.Cells[row, 2].Value?.ToString();
                                break;

                            case 8:
                                admin.Bic = worksheet.Cells[row, 2].Value?.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                    //10 11 header info


                    // row 12 invoices of debtors
                    for (int row = 12; row <= rowCount; row++)
                    {
                        int col = 1;
                        // consective 3 rows has empty start columns that mean empty rows
                        var flag =
                            string.IsNullOrWhiteSpace(worksheet.Cells[row, 2].Value?.ToString() ?? String.Empty);

                        if (flag)
                        {
                            checkCounter++;
                            if (checkCounter == 3)
                                break;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Value?.ToString() ?? String.Empty))
                        {
                            continue;// if debtor number is zero
                        }
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 12].Value?.ToString() ?? String.Empty)) {
                            continue;// if invoice number is zero
                        }

                        var model = new ExcelModel.ExcelModel(upload);
                        model.I_ActionDate = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        //model.I_DossierNo = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        //col++;
                        model.d_Number = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Name = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Address = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Postal = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_City = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Country = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Phone = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Mobile = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.d_Email = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_InvoiceNo = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_InvoiceDate = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Expired = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Amount = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Open = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Currency = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_AdminCosts = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        col++;
                        model.I_Paid = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        col++;
                        col++;
                        //model.I_Interest = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        //col++;
                        //model.I_CollectionFee = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        //col++;
                        //model.I_Paidmm = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        //col++;
                        //model.I_PaidClient = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        //col++;
                        //col++;//Total payment
                        model.I_Action = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;//Continued action
                        model.I_Notes = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                       model.I_DisputeAction = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        col++;
                        col++;
                       // model.I_Status = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                       // col++;
                        col++;
                         model.d_Contact = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        col++;
                        model.I_Paractitioner = worksheet.Cells[row, col].Value?.ToString().Trim() ?? string.Empty;
                        listExcelModel.Add(model);
                        // model. = worksheet.Cells[row, col].Value?.ToString().Trim()??string.Empty;

                    }

                }
            }
            return listExcelModel;
        }
    }
}
