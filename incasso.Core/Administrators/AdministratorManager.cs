using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Incasso.Authorization.Users;
using Incasso.Editions;
using System.Linq;

namespace Incasso.Administrators
{
    public class AdministratorManager : ITransientDependency
    {
        private readonly IRepository<Administrator> repository;
        public AdministratorManager(
            IRepository<Administrator> repository
            )
        {
            this.repository = repository;

        }

        public async Task<Administrator> GetByIdAsync(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task DeleteAsync(Administrator entity)
        {
            await repository.DeleteAsync(entity);
        }

        public async Task<List<Administrator>> GetAll()
        {
            return repository.GetAll().ToList();
        }

        public async Task<Administrator> GetAdminByDebtorId(int debtorId)
        {
            var admin = repository.GetAll().First(x=>x.Debtors.Any(y=>y.Id==debtorId));
            return admin;
        }
    }
}