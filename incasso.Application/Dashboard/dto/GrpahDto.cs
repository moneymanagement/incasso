using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incasso.Administrator;
using Incasso.MultiTenancy.Dto;
using Incasso.Upload;
using Incasso.Upload.Dto;

namespace incasso.Dashboard.dto
{
    public class GraphDto
    {
        public DebtorDto DebtorDetails{ get; set; }
        public List<UploadDto> Uploads{ get; set; }
        public float? TotalInvoiceAmmount { get; set; }
        public float? TotalPaidBeforeSubmission { get; set; }
        public float? TotalInterest { get; set; }
        public float? TotalCollectionCharges { get; set; }
        public float? TotalAdminCosts { get; set; }
        public float? TotalPaid { get; set; }
        public String Currency{ get; set; }
        public string Dosiers
        {
            get
            {
                //if (Uploads != null)
                //{
                //    return string.Join("/", Uploads.Select(x => x.Dosiers).ToArray());
                //}
                return string.Empty;
            }
        }
        public decimal TotalTotalOutStanding => 
            ((decimal) 
            (TotalInvoiceAmmount + TotalPaidBeforeSubmission + TotalInterest + TotalCollectionCharges + TotalAdminCosts - TotalPaid));


    }
}
