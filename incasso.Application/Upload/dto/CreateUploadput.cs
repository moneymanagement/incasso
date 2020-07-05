using Abp.AutoMapper;
using incasso.Administrators.dto;
using System;
using System.Collections.Generic;

namespace Incasso.Upload.Dto
{
    [AutoMapTo(typeof(Upload))]
    public class CreateUploadInput
    {
        public string PhysicalFileName { get; set; }
        public string FileName { get; set; }
        public List<int> Admins { get; set; }
        public string PhysicalFilePath { get; set; }
        public DateTime? Date { get; set; }
        public bool IsOverride { get; set; }
        public string FileType { get; set; }

    }
}
