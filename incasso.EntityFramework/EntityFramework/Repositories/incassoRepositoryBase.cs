using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Incasso.EntityFramework.Repositories
{
    public abstract class incassoRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<IncassoDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected incassoRepositoryBase(IDbContextProvider<IncassoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class incassoRepositoryBase<TEntity> : incassoRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected incassoRepositoryBase(IDbContextProvider<IncassoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            
        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
