using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Debtors;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Debtor))]
    public class CreateDebtorDto
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string DossierNo { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Postal { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }
        public string AdministratorId { get; set; }
        public string Notes_mm { get; set; }
      //  public int Status { get; set; }
    }
}