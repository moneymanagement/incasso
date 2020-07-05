using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Invoices;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Invoice))]
    public class CreateInvoiceDto
    {
        public int Type { get; set; }

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
        public string FileName { get; set; }
        public int Status { get; set; }
        public bool Closed { get; set; }
        public string DisputeAction { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Action { get; set; }
        public int UploadId { get; set; }
        public int DebtorId { get; set; }
        public int UserId { get; set; }

        //public virtual User User { get; set; }
        //public virtual Debtor Debtor { get; set; }
        //public virtual Upload Upload { get; set; }

    }
}