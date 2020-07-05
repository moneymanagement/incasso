using Abp.Dependency;
using Abp.Domain.Repositories;
using incasso.Debtors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Debtos
{
    public class DebtorManager : ITransientDependency
    {
        private readonly IRepository<Debtor> repository;
        public DebtorManager(
            IRepository<Debtor> repository
            )
        {
            this.repository = repository;

        }

        public async Task<Debtor> GetByIdAsync(int id)
        {
            return await repository.GetAsync(id);
        }

        public IQueryable<Debtor> GetAll( )
        {
            return  repository.GetAll();
        }

        public async Task DeleteAsync(Debtor entity)
        {
            await repository.DeleteAsync(entity);
        }

        public async Task DeleteByAdminId(int id)
        {
            var debtors = repository.GetAll().Where(x => x.AdministratorId == id).ToList();
            foreach (var item in debtors)
            {
                await DeleteAsync(item);
            }
        }
    }
}
