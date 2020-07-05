using Abp.Domain.Entities.Auditing;
using incasso.Invoices;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Incasso.MultiTenancy.Dto
{
    public class InvoiceNoteDto : FullAuditedEntity
    {
        public DateTime? NoteDate { get; set; }
        public string Notes { get; set; }
        public string Added_By_Portal { get; set; }
        public int? ParentId { get; set; }
        public string Status { get; set; }
        public int InvoiceId { get; set; }
        public   Invoice Invoice { get; set; }
    }
}
