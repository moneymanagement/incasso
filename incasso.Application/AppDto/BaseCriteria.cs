using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.AppDto
{
    public class BaseCriteria : PagedResultRequestDto
    {
        public int RequestedPage { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
    }
}
