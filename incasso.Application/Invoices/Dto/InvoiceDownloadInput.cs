using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices.Dto
{
    public class InvoiceDownloadInput
    {
        public int DebtorId { get; set; }
        public bool IsClosedInvoice { get; set; }
        public string Portal { get; set; }
    }
}
