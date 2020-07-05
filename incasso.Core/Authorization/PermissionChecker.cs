using Abp.Authorization;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;

namespace Incasso.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
