using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Debtors;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Debtor))]
    public class EditDebtorDto
    {
        public string Number { get; set; }
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
        public int Id { get;   set; }
        public int AdministratorId { get;   set; }
    }
}