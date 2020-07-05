using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.InvoiceNotes
{
    public class InvoiceNote : FullAuditedEntity
    {
        public DateTime? NoteDate { get; set; }
        public string Notes { get; set; }
        public string Added_By_Portal { get; set; }
        public int? ParentId { get; set; }
        public string Status { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
        public virtual Invoices.Invoice Invoice{ get; set; }
        public bool IsEnterByUser { get; set; }
    }
}
