using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Incasso.Authorization.Users;
using System.Linq;
using Incasso.Administrators;
using System.Collections.Generic;
using incasso.Administrators.dto;
using incasso.Users.Dto;
using Incasso.Authorization.Roles;

namespace Incasso.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }
        public bool OutSourcing { get; set; }
        public bool Incasso { get; set; }
        public   string[] ROLES { get; set; }
        public string Role => ROLES?.FirstOrDefault(); 

        public string UserRole { get;  set; } 
        public   ICollection<AdministratorDto> Administrators { get;  set; }
        public string AdministratorsList { get {
                if(Administrators!=null)
                return string.Join(", ", Administrators.Select(x => x.Name));
                return string.Empty;
            } }
    }
}
