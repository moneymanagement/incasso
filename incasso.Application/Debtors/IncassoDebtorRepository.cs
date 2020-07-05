using Incasso.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using Incasso.EntityFramework;
using Abp.Dependency;

namespace incasso.Debtors
{
    public class IncassoDebtorRepository : incassoRepositoryBase<Debtor>,ITransientDependency
    {
        public IncassoDebtorRepository(IDbContextProvider<IncassoDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    }
}
