using incasso.Invoices.Dto;

namespace Incasso.Administrator
{
    public class DebtorInvoiceGridDto
    {
        public int DebtorId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string DebtorNumber { get; set; }
        public string Country { get; set; }
        public bool IsClosed  => (Summary?.TotalTotalOutStanding??0) == 0|| !IsActive;
        public GraphDto Summary { get; set; }
    }
}
 