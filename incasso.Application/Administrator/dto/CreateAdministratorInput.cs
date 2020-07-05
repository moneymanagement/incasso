using Abp.AutoMapper;
using Incasso.Authorization.Users;
using Incasso.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incasso.Administrators.dto
{
    [AutoMapTo(typeof(Administrator))]
    public class CreateAdministratorInput
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
        public virtual List<long> UsersList { get; set; }
    }
}
