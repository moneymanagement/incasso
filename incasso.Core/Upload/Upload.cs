using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using incasso.Debtors;
using Incasso.Administrators;
using Incasso.Authorization.Users;
using System;
using System.Collections.Generic;

namespace Incasso.Upload
{
    public class Upload : FullAuditedEntity, ISoftDelete
    {
        public string FileName { get; set; }
        public string PhysicalFilePath { get; set; }
        public string PhysicalFileName { get; set; }
        public string FileType { get; set; }
        public DateTime? Date { get; set; }
        public virtual ICollection<Administrator> Administrators { get; set; }
        public bool IsOverride { get; set; }
    }
}