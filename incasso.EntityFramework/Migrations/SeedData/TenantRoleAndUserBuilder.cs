using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Incasso.Authorization;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.EntityFramework;

namespace Incasso.Migrations.SeedData
{
    public class TenantRoleAndUserBuilder
    {
        private readonly IncassoDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(IncassoDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            //Admin role
            var Viewer = _context.Roles.FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Viewer);
            if (Viewer == null)
            {
                Viewer = new Role(_tenantId, StaticRoleNames.Viewer, StaticRoleNames.Viewer)
                {
                    IsStatic = true
                };

                Viewer.SetNormalizedName();

                _context.Roles.Add(Viewer);
                _context.SaveChanges();
            }
                var Editor = _context.Roles.FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Editor);
                if (Editor == null)
                {
                    Editor = new Role(_tenantId, StaticRoleNames.Editor, StaticRoleNames.Editor)
                    {
                        IsStatic = true
                    };

                    Editor.SetNormalizedName();

                    _context.Roles.Add(Editor);
                    _context.SaveChanges();
                }
                    var adminRole = _context.Roles.FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin)
                {
                    IsStatic = true
                };

                adminRole.SetNormalizedName();

                _context.Roles.Add(adminRole);
                _context.SaveChanges();

                //Grant all permissions to admin role
                var permissions = PermissionFinder
                    .GetAllPermissions(new incassoAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant))
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            TenantId = _tenantId,
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRole.Id
                        });
                }

                _context.SaveChanges();
            }

            //admin user

            var adminUser = _context.Users.FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == User.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com", User.DefaultPassword);
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
        }
    }
}