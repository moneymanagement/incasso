using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using System.Linq;

namespace Incasso.Upload
{
    public class UploadManager : ITransientDependency
    {
        private readonly IRepository<Upload> repository;
        public UploadManager(
            IRepository<Upload> repository
            )
        {
            this.repository = repository;

        }

        public async Task<Upload> GetByIdAsync(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task DeleteAsync(Upload entity)
        {
            await repository.DeleteAsync(entity);
        }

        public async Task<List<Upload>> GetAll()
        {
            return repository.GetAll().ToList();
        }
    }
}