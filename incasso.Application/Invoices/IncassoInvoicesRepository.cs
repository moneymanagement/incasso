using Incasso.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using Incasso.EntityFramework;
using Abp.Dependency;
using Abp.Domain.Entities.Auditing;

namespace incasso.Invoices
{
    public class IncassoRepository<T> : incassoRepositoryBase<T>, ITransientDependency 
        where T : FullAuditedEntity
    {
        public IncassoRepository(IDbContextProvider<IncassoDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    } 
    public class IncassoInvoiceRepository : incassoRepositoryBase<Invoice>,ITransientDependency
    {
        public IncassoInvoiceRepository(IDbContextProvider<IncassoDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    }
}
