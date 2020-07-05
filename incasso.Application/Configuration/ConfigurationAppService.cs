using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Incasso.Configuration.Dto;

namespace Incasso.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : IncassoAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
