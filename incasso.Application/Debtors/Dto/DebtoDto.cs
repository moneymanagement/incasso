using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using incasso.Administrators.dto;
using incasso.Debtors;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Debtor)), AutoMapFrom(typeof(Debtor))]
    public class DebtorDto : EntityDto
    {
        public AdministratorDto Administrator { get; set; }
        public string Number { get; set; }
        public string DossierNo { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Postal { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }
        public string Notes_mm { get; set; }
        public int AdministratorId { get; set; }
        public int Status { get; set; } = 0;
    }
}
