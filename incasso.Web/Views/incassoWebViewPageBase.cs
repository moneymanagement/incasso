using Abp.Web.Mvc.Views;

namespace Incasso.Web.Views
{
    public abstract class incassoWebViewPageBase : incassoWebViewPageBase<dynamic>
    {

    }

    public abstract class incassoWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected incassoWebViewPageBase()
        {
            LocalizationSourceName = incassoConsts.LocalizationSourceName;
        }
    }
}