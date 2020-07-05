using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices.Dto
{
    [AutoMapTo(typeof(Invoice)), AutoMapFrom(typeof(Invoice))]
    public class InvoiceNotesDto : FullAuditedEntity
    {
        public DateTime? NoteDate { get; set; }
        public string Notes { get; set; }
        public string Added_By_Portal { get; set; }
        public int? ParentId { get; set; }
        public string Status { get; set; }
        public int InvoiceId { get; set; }
        public  Invoice Invoice { get; set; }
    }
}
