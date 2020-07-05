using Abp.IdentityFramework;
using Abp.UI;
using Abp.Web.Mvc.Controllers;
using Microsoft.AspNet.Identity;

namespace Incasso.Web.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class ControllerBase : AbpController
    {
        protected ControllerBase()
        {
            LocalizationSourceName = incassoConsts.LocalizationSourceName;
        }

        protected virtual void CheckModelState()
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException(L("FormIsNotValidMessage"));
            }
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}