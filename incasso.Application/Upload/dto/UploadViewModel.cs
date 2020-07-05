using Abp.Application.Services.Dto;
using System.Collections.Generic;
using Incasso.Users.Dto;
using incasso.Administrators.dto;

namespace Incasso.Upload.Dto
{
    public class UploadViewModel
    {
        public int PageSize { get; internal set; }
        public int RequestedPage { get; internal set; }
        public PagedResultDto<UploadDto>  Uploads{ get;  set; }
        public List<AdministratorDto>  Administrators{ get;  set; }
        public string Search { get; internal set; }
    }
}