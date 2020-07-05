using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Incasso.Administrators;
using Incasso.Authorization.Users;
using Incasso.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Administrators.dto
{
    [AutoMapTo(typeof(Administrator))]
    public class UpdateAdministratorDto :  EntityDto
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
        public List<long>UsersList { get; set; }

    }
}
