using Incasso.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices.Dto
{
    public class GraphDto
    {
        public DebtorDto DebtorDetails { get; set; }
        
        public string Currency { get; set; }
        public float? TotalInvoiceAmmount { get; set; }
        public float? TotalOpen { get; set; }
        public float? TotalPaid { get; set; }
        public float? TotalInterest { get; set; }
        public float? TotalCollectionCharges { get; set; }
        public float? TotalAdminCosts { get; set; }
        public float? TotalTotalOutStanding { get; set; }
         
        public float TotalPaidBeforeSubmission { get; set; } = 0;
        public string DossierNo { get;   set; }
        public float? OutsourcingOpen { get; internal set; }
        public float? OutsourcingPaid { get; internal set; }
        public float? OutsourcingOutStanding { get; internal set; }
        public float? OutsourcingInvoiceAmount { get; internal set; }
    }
}
