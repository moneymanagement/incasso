using Abp.AutoMapper;
using Incasso.Sessions.Dto;

namespace Outsourcing.Web.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}