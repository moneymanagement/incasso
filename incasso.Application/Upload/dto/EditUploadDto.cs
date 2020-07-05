using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Debtors;
using System.Collections.Generic;
using Incasso.Authorization.Users;
using incasso.Administrators.dto;
using System;

namespace Incasso.Upload.Dto
{
    [AutoMapTo(typeof(Upload))]
    public class EditUploadDto
    {
        public string PhysicalFileName { get; set; }
        public string FileName { get; set; }
        public virtual ICollection<AdministratorDto> Administrators { get; set; }
        public string FileType { get; set; }
        public bool IsOverride { get; set; }
        public DateTime? Date { get; set; }

    }
}