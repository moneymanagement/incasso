using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices
{
   public class StatusCatalog : FullAuditedEntity
    {
        public string Catalog { get; set; }

    }
}
