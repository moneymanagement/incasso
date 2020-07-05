using Abp.AutoMapper;
using Incasso.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Users.Dto
{
    [AutoMapTo(typeof(UsersAdministrators))]
    public class UsersAdministratorsDto
    {
        public long User_Id { get; set; }
        public int Administrator_Id { get; set; }
    }
}
