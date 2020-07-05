using System.Threading.Tasks;
using Abp.Application.Services;
using Incasso.Configuration.Dto;

namespace Incasso.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}