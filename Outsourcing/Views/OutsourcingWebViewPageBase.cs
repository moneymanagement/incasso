using Abp.Web.Mvc.Views;
using Incasso;

namespace Outsourcing.Web.Views
{
    public abstract class OutsourcingWebViewPageBase : OutsourcingWebViewPageBase<dynamic>
    {

    }

    public abstract class OutsourcingWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected OutsourcingWebViewPageBase()
        {
            LocalizationSourceName = incassoConsts.LocalizationSourceName;
        }
    }
}