using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Incasso.Authorization;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.Editions;
using Incasso.MultiTenancy.Dto;
using Microsoft.AspNet.Identity;
using Incasso.Administrator;
using incasso.Administrators.dto;
using Incasso.Administrators.dto;
using Incasso.Administrators;
using incasso.Debtors;
using System.Collections.Generic;
using Abp.AutoMapper;
using System.Data.Entity;
using Abp.UI;

namespace Incasso.MultiTenancy
{
    [AbpAuthorize]
    public class DebtorsAppService : IncassoAppServiceBase, IDebtorsAppService
    {
        private readonly AdministratorManager _AdministratorManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        //private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IncassoDebtorRepository _repository;
        public DebtorsAppService(
            AdministratorManager administratorManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            //IAbpZeroDbMigrator abpZeroDbMigrator, 
            IncassoDebtorRepository _repository
        )  
        {
            this._repository = _repository;
            _AdministratorManager = administratorManager;
            _roleManager = roleManager;
            //_abpZeroDbMigrator = abpZeroDbMigrator;
            _userManager = userManager;
        }

        public async Task<DebtorDto> Create(CreateDebtorDto input)
        {
            if (_repository.GetAll().Any(x => x.Number == input.Number))
            {
                throw new UserFriendlyException(L("DebiteurNumberalreadyexists"));
            }

            var record = input.MapTo<Debtor>();
            await _repository.InsertAsync(record);
            return record.MapTo<DebtorDto>();
        }

        public async Task<DebtorDto> Get(EntityDto<int> input)
        {
            var tenant= _repository.GetAll().Include(x => x.Administrator).FirstOrDefault(x => x.Id == input.Id);
            return tenant.MapTo<DebtorDto>();
        }

        public async Task Delete(EntityDto<int> input)
        {
            var tenant =  await _repository.GetAsync(input.Id);
            await _repository.DeleteAsync(tenant);
        }

        public async Task<DebtorViewModel> GetGrid(CriteriaDebtorSearch input)
        {
            input.SkipCount = (int)((input.RequestedPage * input.PageSize));
            var query =  _repository.GetAll().Include(x=>x.Administrator).Where(x => string.IsNullOrEmpty(input.Search) || x.Name.Contains(input.Search) || x.Number.Contains(input.Search) || x.Administrator.Name.Contains(input.Search)).OrderBy(x => x.Name);
            var count = query.Count();
            var users = query.Skip(input.SkipCount).Take(input.MaxResultCount).MapTo<List<DebtorDto>>();
            return new DebtorViewModel
            {
                Search = input.Search,
                PageSize = input.PageSize,
                RequestedPage = input.RequestedPage,
                Debtors = new PagedResultDto<DebtorDto> { Items = users, TotalCount = count },
            };
        }

        public async Task<DebtorDto> Update(EditDebtorDto input)
        {
            if (_repository.GetAll().Any(x => x.Number == input.Number && input.Id != x.Id))
            {
                throw new UserFriendlyException(L("DebiteurNumberalreadyexists"));
            }

            var record = await _repository.GetAsync(input.Id);
             input.MapTo(record);
            if (record.AdministratorId != input.AdministratorId)
            {
                var admin = await _AdministratorManager.GetByIdAsync(input.AdministratorId);
                record.Administrator = admin;
            }
            await _repository.UpdateAsync(record);
            return record.MapTo<DebtorDto>();
        }

        public  async Task<DebtorDto> ChangeStatus(DebtorDto input )
        {
            var record = _repository.Get(input.Id);
            record.Status = input.Status;
            await _repository.UpdateAsync(record);
            return record.MapTo<DebtorDto>();
        }
        public async Task SaveNotes(DebtorDto input)
        {
            var debtor = _repository.Get(input.Id);
            debtor.Notes = input.Notes;
            await _repository.UpdateAsync(debtor);
        }
    }
}