using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incasso.Administrator;
using Incasso.Upload;

namespace incasso.Dashboard.dto
{
    public class DashboardDto
    {
        public int NoOfViewUser {get;set;}
        public int NoOfAdminUser { get;set;}
        public int NoOfAdministrator{ get;set;}
        public string FileName{ get;   set; }
        public string UploadedDate{ get;   set; }
        public int NoOfAdministrators { get; internal set; }

        public static implicit operator DashboardDto(List<DebtorInvoiceGridDto> v)
        {
            throw new NotImplementedException();
        }
    }
}
