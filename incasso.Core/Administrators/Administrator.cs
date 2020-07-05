using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using incasso.Debtors;
using Incasso.Authorization.Users;
using System.Collections.Generic;

namespace Incasso.Administrators
{
    public class Administrator : FullAuditedEntity, ISoftDelete
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public virtual ICollection<Debtor> Debtors{  get; set;  }
        public virtual ICollection<User> Users{  get; set;  }
        public virtual ICollection<Upload.Upload> Uploads { get; set; }
        public string AdminId { get; set; }
    }
}