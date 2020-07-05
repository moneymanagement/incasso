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
using System.Collections.Generic;
using Abp.AutoMapper;
using System;
using System.Data.Entity;
using Abp.UI;
using incasso.Debtos;
using incasso.Invoices;

namespace Incasso.MultiTenancy
{
    public class AdministrationsAppService : IncassoAppServiceBase,IAdministrationsAppService // AsyncCrudAppService<Incasso.Administrators.Administrator, AdministratorDto, int, PagedResultRequestDto, CreateAdministratorInput, UpdateAdministratorDto>, IAdministrationsAppService
    {
        private readonly AdministratorManager _AdministratorManager;
        private readonly DebtorManager _DebtorManager;
        private readonly IRepository<Administrators.Administrator, int> repository;
        private readonly IRepository<UsersAdministrators, int> usersAdminRepository;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IInvoiceManager _InvoiceManager;
        //private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        
        public AdministrationsAppService(
            IRepository<Administrators.Administrator, int> repository,
            // IRepository<UsersAdministrators, int> usersAdminRepository,
            DebtorManager DebtorManager,
            AdministratorManager administratorManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            //IAbpZeroDbMigrator abpZeroDbMigrator,
            IInvoiceManager invoiceManager
        ) 
        {
            //this.usersAdminRepository = usersAdminRepository;
            _AdministratorManager = administratorManager;
            this.repository = repository;
            _roleManager = roleManager;
            _DebtorManager = DebtorManager;
           // _abpZeroDbMigrator = abpZeroDbMigrator;
            _userManager = userManager;
            _InvoiceManager = invoiceManager;
        }

        public  async Task Delete(EntityDto<int> input)
        {
            //CheckDeletePermission();
            var tenant = await _AdministratorManager.GetByIdAsync(input.Id);
            await _DebtorManager.DeleteByAdminId(tenant.Id);
            await _InvoiceManager.DeleteByAdminId(tenant.Id);
            await _AdministratorManager.DeleteAsync(tenant);
        }

        public   async Task<AdministratorDto> Create(CreateAdministratorInput input)
        {
            if (repository.GetAll().Any(x => x.Number == input.Number  ))
            {
                throw new UserFriendlyException(L("AdministratorNumberalreadyexists"));
            }


            var record = input.MapTo<Administrators.Administrator>();
            AddUpdateUser(record,input.UsersList);
            await repository.InsertAsync(record);
            return record.MapTo<AdministratorDto>();
        }

        private void AddUpdateUser(Administrators.Administrator record, List<long> userList)
        {
            record.Users?.Clear();
            var users = _userManager.Users.Where(x => userList.Contains(x.Id)).ToList();
            record.Users = users;
        }

        public   async Task<AdministratorDto> Update(UpdateAdministratorDto input)
        {
            if (repository.GetAll().Any(x => x.Number == input.Number && input.Id != x.Id)) {
                throw new UserFriendlyException(L("AdministratorNumberalreadyexists"));
            }
            var admin = repository.GetAll().Include(x => x.Users).First(x => x.Id == input.Id);

            var viewUser = _userManager.Users.Include(x=>x.Roles).Where(x => input.UsersList.Contains(x.Id)).ToList();

            foreach (var item in admin.Users.ToList())
            {
                var flag = await _userManager.IsInRoleAsync(item.Id, StaticRoleNames.Viewer);
                if (flag)
                    admin.Users.Remove(item);
            }
            foreach (var item in viewUser)
            admin.Users.Add(item);
            input.MapTo(admin);
            await repository.UpdateAsync(admin);
            return admin.MapTo<AdministratorDto>();
        }

        public async Task<AdministratorViewModel> GetGrid(CriteriaAdministratorSearch input)
        {
            input.SkipCount =  (input.RequestedPage * input.PageSize);
            var query = repository.GetAll();

            if (!string.IsNullOrEmpty(input.Search))
            {
                query=query.Where(x => x.Number.Contains(input.Search) || x.Name.Contains(input.Search));
            }
            query=query.OrderBy(x => x.Name);

            var count = query.Count();
            var users = query.Skip(input.SkipCount).Take(input.MaxResultCount).MapTo<IReadOnlyList<AdministratorDto>>();

            return new AdministratorViewModel
            {
                Search = input.Search,
                PageSize = input.PageSize,
                RequestedPage = input.RequestedPage,
                Administrators = new PagedResultDto<AdministratorDto> { Items = users, TotalCount = count },
            };
        }

        public async Task<PagedResultDto<AdministratorDto>> GetAll(PagedResultRequestDto pagedResultRequestDto)
        {
            var result = repository.GetAll();
            return new PagedResultDto<AdministratorDto> { Items = result.MapTo<IReadOnlyList<AdministratorDto>>(), TotalCount = result.Count() };
        }

        public async Task<AdministratorDto> Get(EntityDto<int> input)
        {
            var record= repository.GetAll().Include(x=>x.Users).First(x=>x.Id==input.Id);
           var result= record.MapTo<AdministratorDto>();
           // result.Users?.ToList().ForEach(x => { x.Administrators = null; });
            return result;
        }
    }
}