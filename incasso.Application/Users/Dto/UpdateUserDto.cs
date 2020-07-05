using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Incasso.Authorization.Users;
using System.Collections.Generic;

namespace Incasso.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class UpdateUserDto: EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string Password { get; set; }
        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }
        public List<string> Admins { get; set; }
        public bool OutSourcing { get; set; }
        public bool Incasso { get; set; }
    }
}