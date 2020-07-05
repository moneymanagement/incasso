using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Incasso.Upload.Dto;

namespace Incasso.Upload
{
    public interface IUploadDataAppService : IApplicationService //IAsyncCrudAppService<UploadDto, int, PagedResultRequestDto, CreateUploadInput, UpdateUploadDto>
    {
        Task<UploadViewModel> GetGrid(CriteriaUploadSearch criteriaUploadearch);
        Task<PagedResultDto<UploadDto>> GetAll(PagedResultRequestDto pagedResultRequestDto);
        Task Delete(EntityDto<int> input);
        Task<UploadDto> Get(EntityDto<int> input);
        Task<UploadDto> Create(CreateUploadInput input);
        Task<UploadDto> Update(EditUploadDto input);
    }
}
