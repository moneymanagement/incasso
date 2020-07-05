using System;

namespace Incasso.MultiTenancy.Dto
{
    public class CriteriaInvoiceSearch
    {
        public int RequestedPage { get; set; }
        public int PageSize { get; set; }
        public int SkipCount { get; set; }
        public string Search { get; set; }
        public int MaxResultCount { get; set; }
        public int? AdminId { get; set; }
        public string InvoiceType { get; set; }
        public int DebtorId { get;   set; }
        public bool? Closed { get; set; } 
    }
}