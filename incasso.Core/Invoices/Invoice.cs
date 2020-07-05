using Abp.Domain.Entities.Auditing;
using incasso.Debtors;
using incasso.InvoiceNotes;
using Incasso.Authorization.Users;
using Incasso.Upload;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace incasso.Invoices
{
    public class Invoice : FullAuditedEntity, ICloneable
    {
        public string Type { get; set; }
        public string FileName { get; set; }
        public string DossierNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Currency { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public float? Amount { get; set; }
        public float? Open { get; set; }
        public float? Paid { get; set; }
        public float? Paidmm { get; set; }
        public float? PaidClient { get; set; }
        public float? Interest { get; set; }
        public float? CollectionFee { get; set; }
        public float? AdminCosts { get; set; }

        public float? TotalOpenInc => (Open ?? 0) + (AdminCosts ?? 0) + (CollectionFee ?? 0) + (Interest ?? 0) ;
        public float? PaidBeforeSubmission => (Amount ?? 0) + (Open?? 0);
        public bool Closed { get; set; }
        public string DisputeAction { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Action { get; set; }
        [ForeignKey("Upload")]
         public int UploadId { get; set; }
        [ForeignKey("Debtor")]
        public int DebtorId { get; set; }
        //[ForeignKey("User")]
       // public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual Debtor Debtor { get; set; }
        public virtual Upload Upload { get; set; }
        [ForeignKey("Administrator")]
        public int? AdministratorId { get; set; }
        public virtual Incasso.Administrators.Administrator Administrator { get; set; }
        public virtual  List<InvoiceNote> Notes { get; set; }
        public string Paractitioner { get; set; }
        [ForeignKey("StatusCatalog")]
        public int? Status { get; set; }
        public virtual StatusCatalog StatusCatalog { get; set; }

        public string StatusText => StatusCatalog?.Catalog;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
