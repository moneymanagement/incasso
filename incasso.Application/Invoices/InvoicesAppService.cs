using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.MultiTenancy;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.Editions;
using Incasso.MultiTenancy.Dto;
using Incasso.Administrator;
using Incasso.Administrators;
using incasso.Invoices;
using System.Collections.Generic;
using Abp.AutoMapper;
using System.Data.Entity;
using Abp.Domain.Repositories;
using incasso.Debtors;
using incasso.Invoices.Dto;
using incasso.InvoiceNotes;
using System;
using incasso.Administrators.dto;

namespace Incasso.MultiTenancy
{
    [AbpAuthorize]
    public class InvoicesAppService : IncassoAppServiceBase, IInvoicesAppService
    {
        private readonly AdministratorManager _AdministratorManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        //private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IncassoInvoiceRepository _repository;
        private readonly IRepository<Debtor> debtorRepository;
        private readonly IRepository<InvoiceNote> _invoiceNoteRepository;
        public InvoicesAppService(
            IRepository<Debtor> debtorRepository,
            AdministratorManager administratorManager,
            EditionManager editionManager,
            IRepository<InvoiceNote> invoiceNoteRepository,
            UserManager userManager,
            RoleManager roleManager,
            //IAbpZeroDbMigrator abpZeroDbMigrator, 
            IncassoInvoiceRepository _repository
        )
        {
            _invoiceNoteRepository = invoiceNoteRepository;
            this.debtorRepository = debtorRepository;
            this._repository = _repository;
            _AdministratorManager = administratorManager;
            _roleManager = roleManager;
            //_abpZeroDbMigrator = abpZeroDbMigrator;
            _userManager = userManager;
        }

        public async Task<InvoiceDto> Create(CreateInvoiceDto input)
        {
            var record = input.MapTo<Invoice>();
            await _repository.InsertAsync(record);
            return record.MapTo<InvoiceDto>();
        }

        public async Task<InvoiceDto> Get(EntityDto<int> input)
        {
            var tenant = _repository.GetAll().Include(X=>X.Debtor).Include(x=>x.Administrator).FirstOrDefault(x => x.Id == input.Id);
            var result= tenant.MapTo<InvoiceDto>();
            return result;
        }

        public async Task Delete(EntityDto<int> input)
        {
            var tenant = await _repository.GetAsync(input.Id);
            await _repository.DeleteAsync(tenant);
        }

        public async Task<InvoiceViewModel> GetGrid(CriteriaInvoiceSearch input)
        {
            input.SkipCount = (int)((input.RequestedPage * input.PageSize));
            var query = _repository.GetAll().Include(X=>X.Debtor).Include(X=>X.Administrator)
                .Where(x => (string.IsNullOrEmpty(input.InvoiceType) || input.InvoiceType.ToLower()==x.Type.ToLower()) && 
                (string.IsNullOrEmpty(input.Search) ||
                x.Administrator.Name.Contains(input.Search) || x.DossierNo.Contains(input.Search)) 
                ).OrderBy(x => x.CreationTime);
            var count = query.Count();
            var users = query.Skip(input.SkipCount).Take(input.MaxResultCount).MapTo<List<InvoiceDto>>();
            return new InvoiceViewModel
            {
                InvoiceType = input.InvoiceType,
                Search = input.Search,
                PageSize = input.PageSize,
                RequestedPage = input.RequestedPage,
                Invoices = new PagedResultDto<InvoiceDto> { Items = users, TotalCount = count },
            };
        }

        public async Task<InvoiceDto> Update(EditInvoiceDto input)
        {
            var record = await _repository.GetAsync(input.Id);
            input.MapTo(record);
            await _repository.UpdateAsync(record);
            return record.MapTo<InvoiceDto>();
        }
        public async Task<InvoiceDto> ChangeStatus(InvoiceDto input)
        {
            var record = _repository.Get(input.Id);
            record.Status = input.Status;
            await _repository.UpdateAsync(record);
            return record.MapTo<InvoiceDto>();
        }

        

        public async Task AddNotes(AddNotesInputDto input)
        {
          if(input.Ids!=null && input.Ids.Any())
            {
                var invoices = _repository.GetAll().Include(x => x.Notes).Where(x => input.Ids.Contains(x.Id)).ToList();
                foreach (var item in input.Ids)
                {
                    var invoice = invoices.FirstOrDefault(x => x.Id == item);
                    await _invoiceNoteRepository.InsertAsync(new InvoiceNote
                    {
                        Added_By_Portal = invoice.Type,
                        Notes= input.Notes,
                        InvoiceId= item,
                        IsEnterByUser=true,
                        NoteDate= input.Date??DateTime.Now
                    });
                }
            }
        }
        public async Task UpdateNotes(UpdateNotesInputDto input)
        {
            var note = _invoiceNoteRepository.FirstOrDefault(x => x.Id == input.Id);
            note.NoteDate = input.Date;
            note.Notes = input.Notes;
            await _invoiceNoteRepository.UpdateAsync(note);
        }

        public async Task DeleteNotes(UpdateNotesInputDto input)
        {
          await  _invoiceNoteRepository.DeleteAsync(x => x.Id == input.Id);
        }
    }
}