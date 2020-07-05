using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using incasso.Administrators.dto;
using System;
using System.Collections.Generic;

namespace Incasso.Upload.Dto
{
    [AutoMapFrom(typeof(Upload))]
    public class UploadDto :  EntityDto
    {
        public string PhysicalFileName { get; set; }
        public string PhysicalFilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public bool IsOverride { get; set; }
        public DateTime? Date { get; set; }

    }
}
