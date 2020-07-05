using Incasso.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using Incasso.EntityFramework;
using Abp.Dependency;

namespace Incasso.Uploads
{
    public class UploadCustomRepository : incassoRepositoryBase<Invoice>,ITransientDependency
    {
        public UploadCustomRepository(IDbContextProvider<IncassoDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    }
}
