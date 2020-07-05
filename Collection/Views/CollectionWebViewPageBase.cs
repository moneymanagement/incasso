using Abp.Web.Mvc.Views;
using Incasso;

namespace Collection.Web.Views
{
    public abstract class CollectionWebViewPageBase : CollectionWebViewPageBase<dynamic>
    {

    }

    public abstract class CollectionWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected CollectionWebViewPageBase()
        {
            LocalizationSourceName = incassoConsts.LocalizationSourceName;
        }
    }
}