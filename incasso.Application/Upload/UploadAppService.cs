using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Abp.AutoMapper;
using Incasso.Upload.Dto;
using incasso.Administrators.dto;
using System;
using incasso.Jobs;
using Abp.BackgroundJobs;
using Hangfire;
using incasso.Jobs.UploadJob;
//using Hangfire;

namespace Incasso.Upload
{
    public class UploadDataAppService : IncassoAppServiceBase, IUploadDataAppService // AsyncCrudAppService<Incasso.Upload, UploadDto, int, PagedResultRequestDto, CreateUploadInput, UpdateUploadDto>, IUploadsAppService
    {
        private readonly IRepository<Upload> repository;
        private readonly IRepository<Administrators.Administrator> _administratorRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ProcessUploadFile _ProcessUploadFile;
        
        public UploadDataAppService(ProcessUploadFile processUploadFile,
            IRepository<Upload> repository, IRepository<Administrators.Administrator> administratorRepository
        )
        {
            _ProcessUploadFile = processUploadFile;
            this.repository = repository;
            _administratorRepository = administratorRepository;
        }

        public async Task Delete(EntityDto<int> input)
        {
            var tenant = await repository.GetAsync(input.Id);
            await repository.DeleteAsync(tenant);
        }

        public async Task<UploadDto> Create(CreateUploadInput input)
        {
            var record = input.MapTo<Upload>();
            record.Date = DateTime.Now;
            AddUpdateAdmin(record, input.Admins);
            var id = await repository.InsertAndGetIdAsync(record);
            //_backgroundJobManager.Enqueue<ProcessUploadFile, int>(id, delay: new TimeSpan(0, 0, 5));
            BackgroundJob.Enqueue(() => _ProcessUploadFile.Execute(id) );
            return record.MapTo<UploadDto>();
        }

        private void AddUpdateAdmin(Upload record, List<int> userList)
        {
            var admins = _administratorRepository.GetAll().Where(x => userList.Contains(x.Id)).ToList();
            record.Administrators = record.Administrators ?? new List<Administrators.Administrator>();
            record.Administrators.Clear();

            foreach (var item in admins)
                record.Administrators.Add(item);
        }

        public async Task<UploadDto> Update(EditUploadDto input)
        {
            var record = input.MapTo<Upload>();
            AddUpdateAdmin(record, input.Administrators.Select(x => x.Id).ToList());
            await repository.UpdateAsync(record);
            return record.MapTo<UploadDto>();
        }

        public async Task<UploadViewModel> GetGrid(CriteriaUploadSearch input)
        {
            input.SkipCount = (input.RequestedPage * input.PageSize);
            var query = repository.GetAll();

            if (!string.IsNullOrEmpty(input.Search))
            {
                query = query.Where(x => x.FileName.Contains(input.Search));
            }
            query = query.OrderByDescending(x => x.CreationTime);

            var count = query.Count();
            var users = query.Skip(input.SkipCount).Take(input.MaxResultCount).MapTo<IReadOnlyList<UploadDto>>();
            var admins = _administratorRepository.GetAll().ToList();

            return new UploadViewModel
            {
                Search = input.Search,
                Administrators = admins.MapTo<List<AdministratorDto>>(),
                PageSize = input.PageSize,
                RequestedPage = input.RequestedPage,
                Uploads = new PagedResultDto<UploadDto> { Items = users, TotalCount = count },
            };
        }

        public async Task<PagedResultDto<UploadDto>> GetAll(PagedResultRequestDto pagedResultRequestDto)
        {
            var result = repository.GetAll();
            return new PagedResultDto<UploadDto> { Items = result.MapTo<IReadOnlyList<UploadDto>>(), TotalCount = result.Count() };
        }

        public async Task<UploadDto> Get(EntityDto<int> input)
        {
            var record = repository.Get(input.Id);

            return record.MapTo<UploadDto>();
        }
    }
}