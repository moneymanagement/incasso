using System.Collections.Generic;
using System.Linq;
using Incasso.Roles.Dto;
using Incasso.Users.Dto;

namespace Outsourcing.Web.Models.Users
{
    public class EditUserModalViewModel
    {
        public UserDto User { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }

        public bool UserIsInRole(RoleDto role)
        {
            return User.ROLES != null && User.ROLES.Any(r => r == role.Name);
        }
    }
}