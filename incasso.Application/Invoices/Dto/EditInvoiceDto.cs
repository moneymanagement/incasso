using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Invoices;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Invoice))]
    public class EditInvoiceDto
    {
        public string Type { get; set; }
        public int Id{ get; set; }

        public string DossierNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Currency { get; set; }
        public string InvoiceDate { get; set; }
        public string ExpiredDate { get; set; }
        public string PaymentDate { get; set; }
        public float? Amount { get; set; }
        public float? Open { get; set; }
        public float? Paid { get; set; }
        public float? Paidmm { get; set; }
        public float? PaidClient { get; set; }
        public float? Interest { get; set; }
        public float? CollectionFee { get; set; }
        public float? AdminCosts { get; set; }
        public int Status { get; set; }
        public bool Closed { get; set; }
        public string DisputeAction { get; set; }
        public string Actiondate { get; set; }
        public string Action { get; set; }

        //public virtual User User { get; set; }
        //public virtual Debtor Debtor { get; set; }
        //public virtual Upload Upload { get; set; }

    }
}