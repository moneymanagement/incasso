using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices.Dto
{
    public class UpdateNotesInputDto
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public DateTime? Date { get; set; }
    }
    public class AddNotesInputDto
    {
        public List<int> Ids { get; set; }
        public string Notes { get; set; }
        public DateTime? Date{ get; set; }
    }
}
