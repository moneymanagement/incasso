using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Incasso.Users;
using Incasso.Authorization.Users;
using Incasso.MultiTenancy;
using Microsoft.AspNet.Identity;

namespace Incasso
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class IncassoAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }
        protected IncassoAppServiceBase()
        {
            LocalizationSourceName = incassoConsts.LocalizationSourceName;
        }
        protected async Task<long> GetCurrentUserId()
        {
            return   AbpSession.GetUserId();
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}