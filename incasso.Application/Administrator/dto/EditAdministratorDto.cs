using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using incasso.Debtors;
using System.Collections.Generic;
using Incasso.Authorization.Users;

namespace Incasso.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Administrators.Administrator))]
    public class EditAdministratorDto
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public virtual ICollection<Debtor> Debtors { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}