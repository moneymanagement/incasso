using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using incasso.Administrators.dto;
using incasso.Debtors;
using incasso.Helper;
using incasso.Invoices;
using Incasso.Administrators;
using Incasso.Authorization.Users;
using Incasso.Upload.Dto;
using Incasso.Users.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Invoice)), AutoMapFrom(typeof(Invoice))]
    public class InvoiceDto : EntityDto
    {
        public string DebtorNumber => Debtor?.Number;
        public string Type { get; set; }
        public string FileName { get; set; }
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
        public float? TotalOpen => (Open ?? 0) + (Interest ?? 0) +
            (CollectionFee ?? 0) + (AdminCosts??0)  ;

        public float? TotalOpenInc => (Open ??0) + (AdminCosts ?? 0) + (CollectionFee ?? 0) + (Interest ?? 0);
        public float? PaidBeforeSubmission => (Amount ?? 0) + (Open ?? 0);


        public int Status { get; set; }
        public string StatusFormatted => InvoiceStatusCatalog.ParseToString(Status);
        public bool Closed { get; set; }
        public string DisputeAction { get; set; }
        public string ActionDate { get; set; }
        public string Action { get; set; }
        public string StatusText { get; set; }
        public int UploadId { get; set; }
        public int DebtorId { get; set; }
        public int UserId { get; set; }
        public float? Total =>(Interest??0)+(CollectionFee??0) ;
        public List<InvoiceNoteDto> Notes { get; set; }

        public UserDto User { get; set; }
        public   AdministratorDto Administrator{ get; set; }
        public   DebtorDto Debtor { get; set; }
        public   UploadDto Upload { get; set; }

    }
}
