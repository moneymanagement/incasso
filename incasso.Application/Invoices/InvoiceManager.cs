using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using AutoMapper;
using Castle.Core.Logging;
using incasso.Debtors;
using incasso.ExcelModel;
using incasso.Helper;
using incasso.Jobs.UploadJob;
using Incasso.Administrators;
using Incasso.MultiTenancy.Dto;
using Incasso.Upload;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices
{
    public interface IInvoiceManager : IDomainService, ITransientDependency
    {
        Task ProcessExcelData(Administrator newAdmin, Upload upload, List<ExcelModel.ExcelModel> listExcelModel);
        Task<List<InvoiceDto>> GetDebtorInvoiceList(CriteriaInvoiceSearch input);
        Task DeleteByAdminId(int id);

    }
    public class InvoiceManager : IDomainService, IInvoiceManager
    {
        private readonly IRepository<Invoice> _IncassoInvoiceRepository;
        private readonly IRepository<StatusCatalog> _StatusCatalogRepository;
        private readonly IRepository<Upload> _IncassoRepository;
        private readonly IRepository<InvoiceNotes.InvoiceNote> _NotesRepository;
        private readonly IRepository<Administrator> _adminRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger  _Logger;
        private readonly IRepository<Debtors.Debtor> _IncassoDebtorRepository;

        public InvoiceManager(ILogger Logger,IRepository<StatusCatalog> statusCatalogRepository,IRepository<InvoiceNotes.InvoiceNote> notesRepository,IUnitOfWorkManager unitOfWorkManager, IRepository<Administrator> adminRepositor, IRepository<Debtors.Debtor> incassoDeborRepository, IRepository<Upload> incassoRepository, IRepository<Invoice> incassoInvoiceRepository)
        {
            _Logger = Logger;
            _StatusCatalogRepository = statusCatalogRepository;
            _NotesRepository = notesRepository;
            _adminRepository = adminRepositor;
            _unitOfWorkManager = unitOfWorkManager;
            _IncassoInvoiceRepository = incassoInvoiceRepository;
            _IncassoRepository = incassoRepository;
            _IncassoDebtorRepository = incassoDeborRepository;
        }

        public async Task ProcessExcelData(Administrator newAdmin, Upload upload, List<ExcelModel.ExcelModel> listExcelModel)
        {
            try
            {
                IList<Administrator> administrators = upload.Administrators?.ToList()?? new List<Administrator>();
                using (var uow = _unitOfWorkManager.Begin())
                {

                    var existingAdmin = _adminRepository.GetAll().Include(x=>x.Uploads).FirstOrDefault(x => x.Number.Trim() == newAdmin.Number.Trim());
                    if ((!upload.Administrators?.Any() ?? true))
                    {
                        if (!string.IsNullOrEmpty(newAdmin.Name) && !string.IsNullOrEmpty(newAdmin.Number) && existingAdmin == null)
                        {
                            newAdmin.Uploads = newAdmin.Uploads ?? new List<Upload>();
                            newAdmin.Uploads.Add(upload);
                            newAdmin.Id = await _adminRepository.InsertAndGetIdAsync(newAdmin);
                            //if (!administrators.Any(z => z.Number == newAdmin.Number))
                            //    administrators.Add(newAdmin);
                        }
                        else  
                        {
                            existingAdmin.Account = newAdmin.Account;
                            existingAdmin.Name = newAdmin.Name;
                            existingAdmin.Bank= newAdmin.Bank;
                            existingAdmin.Contact= newAdmin.Contact;
                            existingAdmin.Bic = newAdmin.Bic;
                            existingAdmin.Email= newAdmin.Email;
                            existingAdmin.Iban= newAdmin.Iban;
                            existingAdmin.Phone= newAdmin.Phone;
                            existingAdmin.Phone2 = newAdmin.Phone2;
                            existingAdmin.Uploads = existingAdmin.Uploads ?? new List<Upload>();
                            existingAdmin.Uploads.Add(upload);
                            await _adminRepository.UpdateAsync(existingAdmin);
                            //if (!administrators.Any(z => z.Number == existingAdmin.Number))
                            //    administrators.Add(existingAdmin);
                        }
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }

                    var dirtyStatus = 
                        listExcelModel.Select(x => x.GetDebtor()).SelectMany(x => x.Invoices).Where(x=> x.StatusCatalog!=null&& !string.IsNullOrEmpty(x.StatusCatalog.Catalog))
                        .Select(x => x.StatusCatalog.Catalog.Trim()).Distinct().ToList();

                    var allStatus = await _StatusCatalogRepository.GetAllListAsync();

                    foreach (var item in dirtyStatus)
                    {
                        if (!allStatus.Any(x => x.Catalog == item))
                            _StatusCatalogRepository.InsertAndGetId(new StatusCatalog { Catalog = item });
                    }
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    allStatus = await _StatusCatalogRepository.GetAllListAsync();

                    upload = _IncassoRepository.GetAll().Include(x => x.Administrators).FirstOrDefault(x=>x.Id==upload.Id);
                    var adminIds = upload.Administrators.Select(x => x.Id).ToArray();

                    var debtors = _IncassoDebtorRepository.GetAll().Include(X => X.Invoices).Where(X => adminIds.Contains(X.AdministratorId)).ToList();
                    var dirtyInvoiceList = new List<Invoice>();


                    foreach (var admin in upload.Administrators)
                    {
                        var listRecords = listExcelModel.Select(x => x.GetDebtor()).ToList();
                        var grp = listRecords.GroupBy(x => x.Number);
                        //  Parallel.ForEach(grp, async (grpItem) =>
                        foreach (var grpItem in grp)
                        {

                            var invoices = grpItem.ToList()
                                .SelectMany(x => x.Invoices)
                                .ToList();

                            var entity = grpItem.FirstOrDefault();
                            entity.Id = 0;
                            entity = (Debtors.Debtor)entity.Clone();
                            entity.Administrator = admin;
                            entity.AdministratorId = admin.Id;
                            var debtro = debtors.FirstOrDefault(x => entity.Number == x.Number && x.AdministratorId == admin.Id);

                            var debINV = invoices.Clone().ToList();

                            debINV.ForEach(inv =>
                            {
                                var status = allStatus.FirstOrDefault(s => s.Catalog == inv.StatusCatalog.Catalog)??
                                allStatus.FirstOrDefault(y=>y.Catalog== "Minnelijke Incasso");
                                inv.Status = status?.Id;
                                inv.StatusCatalog = status;
                                inv.Id = 0;
                                // x.Administrator = admin;
                                //x.Upload = upload;
                                inv.UploadId = upload.Id;
                                inv.FileName = upload.FileName;
                                inv.Type = upload.FileType;
                                inv.DebtorId = debtro?.Id ?? 0;
                                inv.AdministratorId = admin.Id;
                                inv.Notes = inv.Notes?.DistinctBy(x => new { x.NoteDate, x.Notes }).ToList();
                            });


                            if (debtro == null)
                            {
                                entity.Status = 0;
                                entity.Invoices = debINV;
                                _IncassoDebtorRepository.Insert(entity);
                            }
                            else
                            {
                                MapTo(entity, debtro);
                                debtro.Status = 0;
                                _IncassoDebtorRepository.Update(debtro);
                                dirtyInvoiceList.AddRange(debINV);
                            }
                        }

                        var activeDebtors = listRecords.Select(x => x.Number).Distinct().ToList();
                        var otherDebtors = debtors.Where(x => x.AdministratorId == admin.Id && !activeDebtors.Contains(x.Number)).ToList();

                        foreach (var item in otherDebtors)
                        {
                            item.Status = 3;
                            foreach (var inv in item.Invoices)
                            {
                                inv.Closed = true;
                            }
                            await _IncassoDebtorRepository.UpdateAsync(item);
                        }
                    }


                    var debtorsList = upload.Administrators.SelectMany(x => x.Debtors).ToList();

                    var debtorIdList = debtorsList.Select(x => x.Id).Distinct().ToList();
                    var adminId = upload.Administrators.Select(X => X.Id);

                    var invoiceRecordsByDebtorId = _IncassoInvoiceRepository.GetAll() 
                        .Where(x => x.Type == upload.FileType && adminId.Contains(x.AdministratorId.Value) && debtorIdList.Contains(x.DebtorId)).ToList();

                    dirtyInvoiceList.ForEach(x => x.Notes?.ForEach(y => y.Added_By_Portal = upload.FileType));
                    var notes = new List<KeyValuePair<Invoice, List<InvoiceNotes.InvoiceNote>>>();

                    foreach (var item in debtorIdList)
                    {
                        if (upload.IsOverride)
                        {
                            foreach (var invoice in dirtyInvoiceList.Where(x=>x.DebtorId==item).ToList())
                            {
                                if (!invoiceRecordsByDebtorId.Any(x => invoice.DebtorId == x.DebtorId && x.AdministratorId == invoice.AdministratorId && x.InvoiceNo == invoice.InvoiceNo && x.DossierNo == invoice.DossierNo))
                                    _IncassoInvoiceRepository.Insert(invoice);// if invoice doesnt exist add to the table
                                else
                                {
                                    var dbInvoice = invoiceRecordsByDebtorId.First(x => invoice.DebtorId == x.DebtorId && x.AdministratorId == invoice.AdministratorId && x.InvoiceNo == invoice.InvoiceNo && x.DossierNo == invoice.DossierNo);
                                    MapTo(dbInvoice, invoice);   

                                    _IncassoInvoiceRepository.Update(dbInvoice);
                                    notes.Add(new KeyValuePair<Invoice, List<InvoiceNotes.InvoiceNote>>(dbInvoice,invoice.Notes?.ToList()?? new List<InvoiceNotes.InvoiceNote>()));
                                }
                            }
                        }
                        else
                        {
                            foreach (var inv in dirtyInvoiceList.Where(x => x.DebtorId == item).ToList())
                            {
                                if (!invoiceRecordsByDebtorId.Any(x => inv.DebtorId == x.DebtorId && inv.AdministratorId == x.AdministratorId && x.InvoiceNo == inv.InvoiceNo))
                                    _IncassoInvoiceRepository.Insert(inv);
                            }
                        }

                        var dirtyInvoiceNo = dirtyInvoiceList.Where(x => x.DebtorId == item).Select(x=>x.InvoiceNo).ToList();
                        var otherInvoice = invoiceRecordsByDebtorId.Where(x => x.DebtorId == item && !dirtyInvoiceNo.Contains(x.InvoiceNo)).ToList();
                        otherInvoice.ForEach(x => {
                            x.Closed = true;
                            x.PaymentDate = DateTime.Now;
                            x.Paid = x.Amount;
                            x.Open =x.Type== incasso.Catalogs.PortalType.Collection? x.Open: 0;
                          _IncassoInvoiceRepository.Update(x);
                        });
                    }

                    var invoiceIds = notes.Select(x => x.Key.Id).ToArray();//invoice Ids
                    var invNotes = _NotesRepository.GetAll().Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();

                    foreach (var item in invNotes)
                        if(!item.IsEnterByUser)
                        await _NotesRepository.DeleteAsync(item);
                    foreach (var item in notes)
                    {
                        item.Value.ForEach(x=> {

                            x.IsEnterByUser = false;
                            x.InvoiceId = item.Key.Id ;
                            _NotesRepository.InsertAndGetId(x);
                        });

                    }
                    await uow.CompleteAsync();

                }
            }
            catch (Exception e)
            {
                _Logger.Fatal($"Error in ProcessUploadFile<Br> <B> {e.Message} <br> <B> {e.InnerException}");
                throw e;
            }
        }

        //private async Task DeleteAndAddNewNotes(Invoice dbInvoice, Invoice invoice)
        //{
        //    var notes = _NotesRepository.GetAll().Where(x => x.InvoiceId == dbInvoice.Id).ToList();
        //    foreach (var item in notes)
        //    {
        //          _NotesRepository.DeleteAsync(item);
        //    }
        //    if (invoice.Notes?.Any() ?? false)
        //    {
        //        foreach (var item in invoice.Notes)
        //        {
        //            item.InvoiceId = dbInvoice.Id;
        //              _NotesRepository.InsertAndGetIdAsync(item);
        //        }
        //    }


        //}

        private void MapTo(Debtor entity, Debtor debtors)
        {
            debtors.Name = entity.Name;
            debtors.Address = entity.Address;
            debtors.Postal = entity.Postal;
            debtors.City = entity.City;
            debtors.Country = entity.Country;
            debtors.Phone = entity.Phone;
            debtors.Mobile = entity.Mobile;
            debtors.Email = entity.Email;
            debtors.Contact = entity.Contact;
            //debtors.Status = entity.Status;
        }

        public async Task<List<InvoiceDto>> GetDebtorInvoiceList(CriteriaInvoiceSearch input)
        {
            var invoices = _IncassoInvoiceRepository.GetAll().Include(x => x.Notes).Include(x=>x.StatusCatalog)
                .Where(x => x.Type == input.InvoiceType && (!input.Closed.HasValue || x.Closed == input.Closed) && x.DebtorId == input.DebtorId).ToList();
            return invoices.MapTo<List<InvoiceDto>>();
        }
        public static void MapTo(Invoice dbInvoice, Invoice dirtyInvoice)
        {
            dbInvoice.FileName = dirtyInvoice.FileName;
            dbInvoice.Type = dirtyInvoice.Type;
            dbInvoice.DossierNo = dirtyInvoice.DossierNo;
            dbInvoice.InvoiceNo = dirtyInvoice.InvoiceNo;
            dbInvoice.Currency = dirtyInvoice.Currency;
            dbInvoice.InvoiceDate = dirtyInvoice.InvoiceDate;
            dbInvoice.ExpiredDate = dirtyInvoice.ExpiredDate;
            dbInvoice.PaymentDate = dirtyInvoice.PaymentDate;
            dbInvoice.Amount = dirtyInvoice.Amount;
            dbInvoice.Open = dirtyInvoice.Open;
            dbInvoice.Paid = dirtyInvoice.Paid;
            dbInvoice.Paidmm = dirtyInvoice.Paidmm;
            dbInvoice.PaidClient = dirtyInvoice.PaidClient;
            dbInvoice.Interest = dirtyInvoice.Interest;
            dbInvoice.CollectionFee = dirtyInvoice.CollectionFee;
            dbInvoice.AdminCosts = dirtyInvoice.AdminCosts;
            dbInvoice.Status = dirtyInvoice.Status;
            dbInvoice.StatusCatalog = dirtyInvoice.StatusCatalog;
            dbInvoice.Closed = dirtyInvoice.Closed;
            dbInvoice.DisputeAction = dirtyInvoice.DisputeAction;
            dbInvoice.ActionDate = dirtyInvoice.ActionDate;
            dbInvoice.Action = dirtyInvoice.Action;
            dbInvoice.UploadId = dirtyInvoice.UploadId;
            dbInvoice.Paractitioner = dirtyInvoice.Paractitioner;
            //dbInvoice.Notes?.Clear();
            //dbInvoice.Notes = dbInvoice.Notes ?? new List<InvoiceNotes.InvoiceNote>();
            //dbInvoice.Notes.AddRange( dirtyInvoice.Notes);
            dbInvoice.DebtorId = dirtyInvoice.DebtorId;
            //invoice.User = dbInvoice.User;
            //invoice.Debtor = dbInvoice.Debtor;
            // invoice.Upload = dbInvoice.Upload;
            //invoice.AdministratorId = dbInvoice.AdministratorId;
            //invoice.Administrator = dbInvoice.Administrator;
            //invoice.Administrator = dbInvoice.Administrator;
        }


        public async Task DeleteByAdminId(int id)
        {
            var invoice = _IncassoInvoiceRepository.GetAll().Where(x => x.AdministratorId == id).ToList();
            foreach (var item in invoice)
            {
                await _IncassoInvoiceRepository.DeleteAsync(item);
            }
        }
    }
}
