using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incasso.Administrator;
using Incasso.MultiTenancy.Dto;
using Incasso.Upload;

namespace incasso.Dashboard.dto
{
    public class CollectionDashboardDto
    {
        public CollectionDashboardDto()
        {
            DebtorList = new List<KeyValuePair<string, List<DebtorInvoiceGridDto>>>();
            ClosedDebtorList = new List<KeyValuePair<string, List<DebtorInvoiceGridDto>>>(); 
        }
        public List<KeyValuePair<string, List<DebtorInvoiceGridDto>>> DebtorList { get; set; }
        public List<KeyValuePair<string, List<DebtorInvoiceGridDto>>> ClosedDebtorList { get; set; }
        public List<InvoiceDto> InvoiceList { get; set; }
    }
}
